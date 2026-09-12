# Military Draft System

This is a small simulation built with .NET 10, ASP.NET Core, MediatR and EF Core (SQLite). It models a population of citizens who are born, age, die of old age or misfortune, and — if they meet certain conditions — get drafted into military service, either automatically by "recruitment agents" or manually by a human recruitment officer through the API. Recruitment officers themselves have a full life cycle too: a player can take one on leave, resign, retire, be fired, or have them die, and every citizen or officer death leaves a permanent historical trace even after the underlying row is eventually purged. There is no UI; the interesting part of the system runs entirely in the background as a set of hosted services that keep ticking for as long as the process is alive.

This document walks through the actual runtime flow, from process startup to a steady-state simulation loop, following the code as it exists today rather than describing an idealized design.

## Solution layout

The solution follows a fairly standard Clean Architecture split:

- **`MilitaryDraftSystem.Domain`** — entities (`Citizen`, `RecruitmentOfficer`, `Player`, `AutomaticRecruitmentAgent`, `God`, `Summons`, `CemeteryRecord`, `DeathStatistic`, `WorkerLifecycleStatistic`), value objects, enums, and domain events. No dependencies on anything else.
- **`MilitaryDraftSystem.Application`** — MediatR commands/queries, their handlers, pipeline behaviors, application-level events and their handlers, and the interfaces the application needs from the outside world (`IAppDbContext`, `IRandomProvider`, `IWorldNarrator`).
- **`MilitaryDraftSystem.Infrastructure`** — EF Core `DbContext`, migrations, seeders, the SaveChanges interceptor, and the hosted (background) services that drive the whole simulation.
- **`MilitaryDraftSystem.API`** — the composition root (`Program.cs`) and two controllers: one for drafting and officer lifecycle/stats, one for population-wide cemetery and historical statistics.
- **`MilitaryDraftSystem.Tests`** — xUnit tests covering the MediatR pipeline behaviors, domain entity behavior (citizen mortality, officer lifecycle), and the command/query handlers built on top of them.

There is no separate "worker" project — the API project hosts both the HTTP endpoints and the background simulation, since ASP.NET Core's `WebApplication` is also a generic host that can run `IHostedService` implementations alongside the web server.

## Startup

Everything is wired up in `MilitaryDraftSystem.API/Program.cs`. Reading it top to bottom tells you what actually exists in the system:

- MVC controllers are registered (`AddControllers()`), giving us `DraftController` and `PopulationController`.
- API key authentication and authorization are registered (`AddAuthentication` with the custom `ApiKeyAuthenticationHandler`, plus `AddAuthorization`). This is what protects the draft and population endpoints — see "Authentication" below.
- MediatR is registered and told to scan the `Application` assembly for handlers, via `AssemblyReference` — a marker class that exists purely so `typeof(AssemblyReference).Assembly` has something to point at.
- FluentValidation validators are registered from the same assembly.
- `IPopulationGenerationService`, `IRandomProvider` and `IWorldNarrator` are registered. `IRandomProvider` is a singleton — every random decision anywhere in the simulation (citizen generation, natural lifespan, death rolls, birthdays, officer life events) goes through this one instance, which matters if you ever want deterministic tests or replayable runs.
- EF Core is configured with SQLite, and a `DomainEventsInterceptor` is attached to the `DbContext` via `AddInterceptors`. This interceptor is how domain events raised inside entities eventually reach MediatR — more on this below.
- Three MediatR pipeline behaviors are registered as open generics: `ValidationBehavior<,>`, `LoggingBehavior<,>`, `TransactionBehavior<,>`. Order matters here, and it's explained in the pipeline section.
- Six hosted services are registered, also in a specific order: `DatabaseInitializationService`, `PopulationGenerationHostedService`, `AutomaticRecruitmentHostedService`, `PopulationSimulationHostedService`, `OfficerLifeSimulationHostedService`, `CitizenCleanupHostedService`.

When `app.Run()` is called, the generic host starts every registered `IHostedService` in registration order. `IHostedService.StartAsync` is awaited sequentially before the next one starts, which is why database initialization is listed first — the background loops that follow all assume the database already exists and is migrated.

## Database initialization and seeding

`DatabaseInitializationService.StartAsync` runs once at startup:

```csharp
await db.Database.MigrateAsync(cancellationToken);

if (configuration.GetValue<bool>("Database:Seed"))
{
	await SeedData.SeedAsync(db);
}
```

`MigrateAsync` applies any pending EF Core migrations against the SQLite file referenced by the `DefaultConnection` connection string. There is no "create if not exists" logic beyond what migrations already provide — if the database is missing, migrations create it from scratch.

Seeding is gated by the `Database:Seed` configuration flag (in `appsettings.json`), so it's opt-in per environment. `SeedData.SeedAsync` runs its seeders in order, each idempotent by checking whether its table already has rows:

- `CitizenSeeder` inserts a handful of hardcoded citizens in `WaitingForDraft` status, to have something eligible for drafting immediately.
- `GodSeeder` inserts a single `God` row — the entity that governs whether and how new citizens get generated.
- `PlayerSeeder` inserts the single `Player` the current single-player game is played as (a fixed, well-known `PlayerId`), so recruitment officers have a real owner from the very first seed.
- `RecruitmentOfficerSeeder` inserts recruitment officers used for manual drafting through the API, one of them owned by the seeded player and one left unowned as a legacy/NPC officer.

Because `StartAsync` for this hosted service is awaited before the host moves on to the next hosted service registration, the other background services are guaranteed to see a migrated, seeded database from their very first tick.

## The background loops

Once startup finishes, several `BackgroundService` loops run indefinitely, each with its own polling interval and each independent of the others. They all follow the same shape: create a DI scope, resolve `ISender` (MediatR), send a command, wait, repeat, and never let an exception kill the loop — everything is wrapped in try/catch with a log and a continue.

```
PopulationGenerationHostedService   -> RunPopulationGenerationCommand   every 30s
AutomaticRecruitmentHostedService   -> RunAutomaticRecruitmentCommand   every 1 min
PopulationSimulationHostedService   -> RunPopulationSimulationCommand   every 15s
OfficerLifeSimulationHostedService  -> RunOfficerLifeSimulationCommand  every tick interval
CitizenCleanupHostedService         -> PurgeDeceasedCitizensCommand     every tick interval
```

A new DI scope is created for every tick, which is important: `AppDbContext` is scoped, so each tick gets its own short-lived `DbContext` instance rather than one that lives for the whole process. This is what you'd want anyway with EF Core's change tracker.

These loops are the actual "engine" of the simulation — the API controllers are just a manual override/read mechanism layered on top of the same command pipeline.


## The MediatR pipeline

Every command sent through `ISender.Send` — whether triggered by a background loop or by the API controller — passes through the same three pipeline behaviors, in registration order:

```
ValidationBehavior -> LoggingBehavior -> TransactionBehavior -> actual handler
```

**`ValidationBehavior`** looks up all `IValidator<TRequest>` instances registered for the request type. If there are none (most commands here don't have a validator), it just calls `next()`. If validators exist, it runs them in parallel, collects every `ValidationFailure` from every validator, and throws a single `FluentValidation.ValidationException` if anything failed. Because this runs first, a validation failure never reaches logging, the transaction, or the handler.

**`LoggingBehavior`** wraps the call to `next()`, logs before and after, and returns whatever `next()` returned. There is nothing more to it — it's a thin diagnostic wrapper, not a source of side effects.

**`TransactionBehavior`** opens a database transaction (`IAppDbContext.BeginTransactionAsync`, backed by `EfTransaction` wrapping EF Core's own transaction), invokes `next()`, and commits on success or rolls back and rethrows on any exception. This is the last behavior before the actual handler, so the handler runs entirely inside this transaction.

The ordering is deliberate: you don't want to open a database transaction for a request that's going to fail validation anyway, and you do want logging to observe the full duration of the transactional unit of work, not just the inner handler call.

It's worth noting that the transaction here wraps the handler's `SaveChangesAsync` call, but in practice most handlers only call `SaveChangesAsync` once, near the end, so the "transaction" mostly protects that single batch of changes plus whatever `EfTransaction` disposal does. Domain events published *after* `SaveChangesAsync` (see below) are not inside this transaction's scope in a meaningful sense — they run after the data is already committed.

## Domain events

Domain events raised by entities (`Citizen.RaiseDomainEvent(...)`, which appends to the entity's in-memory `DomainEvents` collection) reach MediatR through a single mechanism: **`DomainEventsInterceptor`**, an EF Core `SaveChangesInterceptor` registered on the `DbContext`. Its `SavedChangesAsync` override runs automatically after every `SaveChangesAsync` call, scans the change tracker for any entity implementing `IHasDomainEvents` — not just `Citizen`, but also `RecruitmentOfficer`, `Player`, and the historical entities — with pending domain events, maps each one to a MediatR notification via `DomainEventMapper.Map`, publishes it, and clears the entity's `DomainEvents` collection.

Command handlers therefore don't publish anything themselves — they mutate entities, call `_db.SaveChangesAsync(cancellationToken)` once, and rely entirely on the interceptor to take it from there. This used to be duplicated (an earlier version of the handlers also looped over `DomainEvents` and published manually after `SaveChangesAsync`), but since the interceptor already drains the collection as part of that same call, the manual loop always found an empty collection and never did anything. That dead code has been removed — the interceptor is the one and only place domain events get published.

`DomainEventMapper.Map` is the single place that knows how to translate a domain event into an application-level MediatR notification:

```
SummonsCreatedDomainEvent      -> SummonsSentEvent
CitizenDraftedDomainEvent      -> CitizenDraftedEvent
CitizenBecameAdultDomainEvent  -> CitizenBecameAdultEvent
CitizenDiedDomainEvent         -> CitizenDiedEvent
OfficerDiedDomainEvent         -> OfficerDiedEvent
OfficerCareerEndedDomainEvent  -> OfficerCareerEndedEvent
```

Any domain event without a mapping throws `ArgumentException` — there's no silent fallback.

## Population generation

`PopulationGenerationHostedService` sends `RunPopulationGenerationCommand` every 30 seconds. The handler:

1. Loads the single `God` row. If it doesn't exist, or `God.Enabled` is false, the handler returns immediately — no citizens are created and nothing is logged beyond that early return. This is the master on/off switch for population growth.
2. Calls `IPopulationGenerationService.Generate(god)`, which decides, using `IRandomProvider`, how many citizens to create this tick (a random count between `God.MinCitizensPerGeneration` and `MaxCitizensPerGeneration`) and builds each one:
   - gender and matching first name pulled from static name lists, last name from a shared list,
   - age generated from a weighted distribution over four brackets (minor, young adult, middle-aged, elderly) defined in `SimulationProbabilities`, so younger citizens are far more common than elderly ones. The chosen bracket is then intersected with `God.MinAge`/`God.MaxAge`, so God's configured age range is the actual outer bound — if the weighted bracket falls entirely outside that range, generation falls back to God's own range directly,
   - a birth date derived from that age plus a small random day offset,
   - a natural lifespan sampled from a gender-specific Gaussian distribution (`SimulationProbabilities.Death.Lifespan`, via `IRandomProvider.NextGaussian`) and assigned once through `Citizen.AssignNaturalLifespan(...)` — this is the age at which the citizen is destined to die of old age if nothing else kills them first,
   - a medical category chosen from another weighted distribution (fit / limited fit / temporarily unfit / permanently unfit),
   - `IsStudent` only possible for ages 18–25, gated by `God.StudentChance`,
   - `HasCriminalRecord` gated by `God.CriminalRecordChance`,
   - and finally a `CitizenStatus` computed from all of the above: minors are `Registered`, citizens over 27 are `Retired`, draft-age citizens who are a student or have a criminal record get an extra roll for `Exempted` or `Deferred`, and everyone else lands in `WaitingForDraft` — the status that actually makes them eligible for drafting later.
3. Each generated citizen is added to the `DbContext` via `_db.AddCitizen`, the world narrator logs a birth line, `God.MarkExecuted(now)` stamps the timestamp, and `SaveChangesAsync` persists the whole batch in one round trip — which is also what triggers `DomainEventsInterceptor` to publish any domain events raised during generation.

## Ageing, birthdays and death

`PopulationSimulationHostedService` sends `RunPopulationSimulationCommand` every 15 seconds. The handler loads every living citizen (`Status != Deceased`, via `GetLivingCitizens`) and, for each one:

- rolls a small chance (`SimulationProbabilities.Birthday.OccursPercent`) that a birthday happens this tick. If it does, `citizen.HaveBirthday()` increments `Age`. Crossing from minor to `AdultAge` (18) flips status from whatever it was to `WaitingForDraft` and raises `CitizenBecameAdultDomainEvent`. Aging past 27 while still `WaitingForDraft` flips status to `Retired` instead — this is the domain-side aging-out of draft eligibility, independent of the exemption/deferment logic that happens at generation time.
- rolls for death via `TryDetermineDeathReason`, which checks, in order: old age (`citizen.HasReachedNaturalLifespan()` — true once `Age` reaches the citizen's own, individually assigned `NaturalLifespanYears` rather than a single fixed threshold for everyone), disease, accident, and — only for citizens currently `Drafted` — military death (friendly fire or combat), and finally a small constant suicide risk. The first roll that hits wins; there's no combination of causes in a single tick.
- if a reason is found, `citizen.Die(reason, now)` sets `Death` (a value object capturing reason and timestamp) and flips `Status` to `Deceased`, raising `CitizenDiedDomainEvent`. The citizen row itself is **not** deleted at this point — it stays in the table, now `Deceased`, until `CitizenCleanupHostedService` purges it later (see "Historical records and cleanup" below).

Only citizens who actually changed (birthday or death) are tracked in `affectedCitizens`, purely for logging/event-publishing purposes — the actual persistence is a single `SaveChangesAsync` call covering every tracked change in the `DbContext`, changed or not.

A citizen dying while `Drafted` and killed by a military-specific cause is what feeds into recruitment officer guilt, covered next.

## Manual drafting and recruitment officer consequences

The main user-facing entry point into the write side of the system is `DraftController.DraftCitizen`, which sends `DraftCitizenCommand(citizenId, recruitmentOfficerId)`. Its handler:

1. Loads the officer; throws if missing or if `!officer.IsActive` — an officer who is on leave, has resigned, retired, been fired, or died can no longer draft anyone. `IsActive` is `true` only while `Status == OfficerStatus.Active`.
2. Loads the citizen; throws if missing.
3. Calls `citizen.Draft(DraftSource.RecruitmentOfficer, officerId, null, now)`. `Citizen.Draft` re-validates eligibility itself (`IsEligibleForDraft`: alive, age 18–27, `MedicalCategory.Fit`, no criminal record, not a student, currently `WaitingForDraft`) and throws if the citizen doesn't qualify — the domain
4. `officer.RegisterDraftedCitizen()` increments the officer's personal draft count — this is what makes the officer responsible for this specific citizen going forward.
5. The summons is immediately marked delivered (`summons.MarkDelivered()` — delivery is treated as instantaneous in this simulation, there's no separate delivery step or delay), added to the context, everything is saved, and events are published by the interceptor during that same `SaveChangesAsync`.

The consequence loop closes later, asynchronously, when the drafted citizen eventually dies. `ApplyOfficerGuiltHandler` subscribes to `CitizenDiedEvent`. If the death reason is one of the military-specific causes, it looks up the `Summons` for that citizen, and if it was issued by a human officer (`RecruitmentOfficerId` is set — automatic-agent drafts have no officer and are skipped here by design, since "automatic recruitment agents have no psychology"), it calls `officer.ApplyGuilt(moraleLossPercent)`. This reduces `MoralePercent`, increments `GuiltIncidentsCount`, and if morale hits zero, retires the officer (`WorkerEndReason.MoraleCollapse`) through the same lifecycle machinery described next, permanently disqualifying them from further drafting via the check in step 1 above.

`GetRecruitmentOfficerStatsQuery`, exposed through `DraftController.GetRecruitmentOfficerStats`, is a read-only view into this state — draft count, morale, guilt incidents, `Status` and `HasEndedCareer` — for whoever is calling the API. `ListRecruitmentOfficersQuery` (`GET /draft/officers`) gives the same lifecycle view across every officer at once, active or not.

## Recruitment officer lifecycle and player ownership

A `RecruitmentOfficer` is not just a static drafting identity — it is a real person with a life cycle, owned by a `Player` (nullable, for legacy/NPC officers). `Player` and `RecruitmentOfficer` are intentionally separate entities even though the game is single-player today: `Player` is the durable, cross-game identity, and `RecruitmentOfficer` is one specific "character" a player is currently or previously playing as. This split is what lets the model support multiple simultaneous players later without any structural change.

The core rule enforced across the system is: **a player may have at most one officer whose career hasn't ended at any given time.**

- `RecruitmentOfficer.Status` (`OfficerStatus`) is `Active`, `OnLeave`, `Resigned`, `Retired`, `Fired`, or `Deceased`. `IsActive` is true only for `Active`. `HasEndedCareer` is true for `Resigned`, `Retired`, `Fired`, and `Deceased` — these are the terminal states.
- The domain methods `GoOnLeave`, `ReturnFromLeave`, `Resign`, `RetireVoluntarily`, `Fire`, `Die`, and `DieOnDuty` are the only ways `Status` changes, each guarded (for example, `GoOnLeave` throws unless the officer is currently `Active`) and each raising either `OfficerCareerEndedDomainEvent` (leave, resignation, retirement, dismissal) or `OfficerDiedDomainEvent` (death, on or off duty), carrying a `WorkerEndReason`.
- `StartOfficerCareerCommandHandler` (`POST /draft/players/{playerId}/officers`) is how a player creates a new officer — either their very first one, or a replacement after a previous one's career ended. Before creating the new `RecruitmentOfficer`, it calls `IAppDbContext.GetActiveRecruitmentOfficerByPlayer(playerId)`, which returns the player's officer if one exists in `Active` or `OnLeave` status (i.e., career not yet ended). If one is found, the handler throws `InvalidOperationException` rather than creating a second concurrent officer for that player — this is the one-active-character-per-player rule, enforced at the point of creation rather than left to callers to respect.
- The rest of the lifecycle commands (`GoOnLeaveCommand`, `ReturnFromLeaveCommand`, `ResignCommand`, `RetireCommand`, `FireOfficerCommand`), exposed through `DraftController` (`POST /draft/officers/{id}/leave|return-from-leave|resign|retire|fire`), just load the officer by id and call the corresponding domain method — the guard logic lives entirely in the domain entity, not duplicated in the handlers.
- `RunOfficerLifeSimulationCommandHandler`, driven every tick by `OfficerLifeSimulationHostedService`, rolls dice for every `Active` officer (`GetActiveRecruitmentOfficers`) for resignation, accidental death, suicide, and death on duty, using the same centralized `IRandomProvider` as the rest of the simulation and `SimulationProbabilities.Officer.*` for the odds. Only one outcome applies per officer per tick.
- Once an officer's career ends by any of these paths, the player is free to call `StartOfficerCareerCommand` again to start a new one — there is no limit on how many officers a single player can go through over the lifetime of the game, only on how many can be un-ended at once.

## Historical records and cleanup

Neither a dead citizen nor a dead/retired officer is meant to vanish without a trace, but the active population and officer tables are also not meant to grow forever with rows nobody needs anymore. The system resolves this with a permanent, append-only history layer that sits alongside — not inside — the active tables:

- `CemeteryRecord` is created the moment a citizen or officer dies (`RecordCitizenDeathHistoryHandler` / `RecordOfficerDeathHistoryHandler`, both subscribing to the respective died event), capturing `SubjectType`, `SubjectId`, `FullName`, `Reason`, and `DiedAt`. It is never deleted; it only gets `MarkOriginalRecordDeleted()` called on it later, once the underlying row is actually purged.
- `DeathStatistic` (grouped by `SubjectType` + `DeathReason`) and `WorkerLifecycleStatistic` (grouped by `WorkerEndReason`, covering leave/resignation/retirement/dismissal in addition to death) are ever-incrementing counters, updated by the same event handlers. They are never recomputed from the current population — a purge or a status change afterward can't retroactively change history that already happened.
- `CitizenCleanupHostedService` ticks periodically and sends `PurgeDeceasedCitizensCommand`. Its handler computes a cutoff from `SimulationRetention.DeceasedCitizenRetention`, loads deceased citizens who died at or before that cutoff (`GetDeceasedCitizensOlderThan`), marks their `CemeteryRecord` as `OriginalRecordDeleted`, and physically removes the `Citizen` row (`RemoveCitizen`) — the delay exists so a citizen's death is visible in the active data for a while before disappearing, while the cemetery and statistics rows persist forever regardless.
- `GET /population/cemetery` (`GetCemeteryRecordsQuery`) and `GET /population/lifecycle-statistics` (`GetLifecycleStatisticsQuery`), both on `PopulationController`, expose this historical layer read-only, independent of whatever the current state of the active tables happens to be.

## Automatic recruitment

`AutomaticRecruitmentHostedService` sends `RunAutomaticRecruitmentCommand` every minute. Unlike manual drafting, this path has no human officer and no morale consequence attached to it. The handler:

1. Loads every enabled `AutomaticRecruitmentAgent`. If there are none, it returns immediately — automatic recruitment is entirely opt-in per agent, similar to how `God.Enabled` gates population generation.
2. Loads every citizen currently `WaitingForDraft` (`GetCitizensEligibleForAutomaticDraft`) — note this is a coarser filter than `Citizen.IsEligibleForDraft`; it doesn't pre-check age, medical category, criminal record or student status at the query level.
3. Distributes eligible citizens across agents round-robin (`agents[i % agents.Count]`) — this is how "multiple agents operate independently" actually manifests: it's a simple load-balancing split, not independent agent-by-agent querying.
4. For each citizen, calls `citizen.Draft(DraftSource.AutomaticAgent, null, agentId, now)`. Because `Draft` internally calls `IsEligibleForDraft`, any citizen that slipped through the coarser query filter (e.g., a student who is still `WaitingForDraft` for some reason) would still fail here and throw — which would abort the whole loop's iteration at that point, since there's no per-citizen try/catch inside this handler. In practice, given how `GenerateStatus` assigns status at creation time, students and criminal-record citizens are shunted to `Exempted`/`Deferred` rather than `WaitingForDraft`, so this edge case is unlikely to trigger, but the code doesn't guard against it explicitly.
5. Each successful draft creates a `Summons` with `AutomaticRecruitmentAgentId` set and `RecruitmentOfficerId` null — which is exactly the flag `ApplyOfficerGuiltHandler` checks to skip morale consequences for automatically drafted citizens. The summons is marked delivered the same way as in the manual flow.
6. Every agent's `LastExecutionAt` is stamped via `MarkExecuted`, all changes are saved in one batch, and events are published per drafted citizen.

## Putting a full scenario together

To trace a citizen from birth to a consequence for someone else, here's a plausible sequence across several background ticks:

1. `PopulationGenerationHostedService` ticks. `God` is enabled, so `RunPopulationGenerationCommand` creates a batch of citizens, one of whom is generated as an 18-year-old, `Fit`, no criminal record, not a student, with a natural lifespan sampled around the low-to-mid 70s — status ends up `WaitingForDraft`. `CitizenBecameAdultDomainEvent` isn't raised here since the citizen is created already at 18, not aged into it.
2. Some time later, `PopulationSimulationHostedService` ticks and this citizen doesn't die or age past 27 yet — nothing happens to them this cycle, but other citizens might age, have birthdays, reach their own natural lifespan, or die of something else.
3. A recruitment officer calls `POST /draft/citizens/{citizenId}/officers/{officerId}` with a valid `X-Api-Key` header (or `AutomaticRecruitmentHostedService` picks the citizen up on its own one-minute cycle, whichever happens first). Either path calls `Citizen.Draft`, flips status to `Drafted`, creates a `Summons`, marks it delivered, and — if manual — increments the officer's draft count.
4. On a later `PopulationSimulationHostedService` tick, `TryDetermineDeathReason` rolls the military-death branch for this now-`Drafted` citizen and returns `KilledInCombat`. `citizen.Die(...)` sets `Deceased` and raises `CitizenDiedDomainEvent`. The citizen row stays in the table for now.
5. `DomainEventsInterceptor` publishes `CitizenDiedEvent` as part of that `SaveChangesAsync` call. Two independent handlers react: `ApplyOfficerGuiltHandler` finds the `Summons`, sees a `RecruitmentOfficerId`, loads that officer, and calls `ApplyGuilt` — if this pushes morale to zero, the officer transitions to `Retired` and can no longer draft anyone through the controller from that point on, but the player who controlled them can now call `StartOfficerCareerCommand` to begin again with a new officer. `RecordCitizenDeathHistoryHandler` independently writes a `CemeteryRecord` and increments the matching `DeathStatistic`, regardless of what happens to the officer.
6. Some time later, `CitizenCleanupHostedService` ticks, finds this citizen's death is older than `SimulationRetention.DeceasedCitizenRetention`, and physically removes the `Citizen` row — but the `CemeteryRecord` and `DeathStatistic` created in step 5 remain, so `GET /population/cemetery` still shows this citizen ever after.

Every step above happens through the same MediatR pipeline (validation, logging, transaction) and the same domain-event plumbing, whether it was triggered by a background timer or an HTTP request — there's no special-casing between the two.

## Authentication

The API is protected by a simple API key scheme (`ApiKeyAuthenticationHandler` in `MilitaryDraftSystem.API/Auth`). Every request to `DraftController` and `PopulationController` must include an `X-Api-Key` header matching the value configured under `Authentication:ApiKey` in `appsettings.json`. There's no notion of users or roles beyond this — it's a single shared secret that proves the caller is allowed to reach these endpoints at all, registered via `AddAuthentication`/`AddAuthorization` in `Program.cs` and enforced with `[Authorize]` on both controllers. Requests without the header, or with the wrong key, get a `401 Unauthorized` before MediatR ever sees them.

## Summons lifecycle

`Summons` now has a real lifecycle instead of sitting permanently in `Created`. The entity exposes `MarkDelivered()`, `MarkAttended()`, `MarkMissed()`, `Cancel()` and `Expire()`, each guarded so only valid transitions are allowed (for example, you can't attend a summons that was never delivered, and you can't cancel one that's already been attended or missed). Both drafting paths — `DraftCitizenCommandHandler` and `RunAutomaticRecruitmentCommandHandler` — call `MarkDelivered()` right after creating the summons, since delivery is treated as instantaneous in this simulation. Nothing currently drives a summons to `Attended`, `Missed`, `Cancelled` or `Expired` — those transitions exist on the entity and are ready to be wired into a future workflow (for example, a background check some time after issuance), but no such workflow exists yet.

## What's deliberately out of scope

A couple of things are still simplified by design, not because they were forgotten:

- `Summons` attendance/expiry is not driven by any timer yet — the lifecycle methods exist on the entity but nothing currently calls `MarkAttended`, `MarkMissed`, `Cancel` or `Expire`. If you need that, it would be a new background service similar to the existing ones.
- The API key is a single shared secret with no per-officer identity — `DraftCitizenCommand` still takes `recruitmentOfficerId` as a request parameter rather than deriving it from the caller's identity, so anyone with the key can still act as any officer.
- `Player` support is single-player in practice: the seed data creates exactly one `Player`, and nothing in the API currently authenticates a caller as a specific player — `StartOfficerCareerCommand` still takes `playerId` as a request parameter. The one-active-officer-per-player rule is already enforced in the domain/application layer, so the model is ready for multiple players, but nothing wires that up yet.
