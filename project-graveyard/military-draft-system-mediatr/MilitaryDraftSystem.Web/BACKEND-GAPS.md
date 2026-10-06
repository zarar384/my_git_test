# Backend gaps affecting the Angular client

This document lists real limitations discovered while auditing
`MilitaryDraftSystem.API` / `MilitaryDraftSystem.Application` before building
`MilitaryDraftSystem.Web`. The frontend does **not** work around these gaps
with invented endpoints or client-side business logic — it shows explicit
"not available yet" states instead, and this file is the single place that
explains why.

## 1. No way to list/search/paginate Citizens

`PopulationController` only exposes:
- `GET /population/cemetery`
- `GET /population/lifecycle-statistics`

There is no `GET /population/citizens` (or equivalent) endpoint. `IAppDbContext`
has `GetLivingCitizens`, `GetCitizen(id)`, `GetCitizensEligibleForAutomaticDraft`,
and `GetDeceasedCitizensOlderThan` internally, but none of these are exposed
through the API.

**Impact:** `DraftCitizenCommand` (`POST /draft/citizens/{citizenId}/officers/{recruitmentOfficerId}`)
requires a `citizenId` that the frontend has no legitimate way to discover.
The Population and Draft screens in the client show a placeholder explaining
this gap instead of simulating a citizen list.

**To unblock:** add a paginated/filterable query endpoint, e.g.
`GET /population/citizens?status=&search=&page=&pageSize=`, returning a
`CitizenSummaryDto` with the fields already on the `Citizen` entity (name,
gender, age, medicalCategory, status, isStudent, hasCriminalRecord, isAlive).

## 2. No way to discover or identify a Player

There is no `GET /players` or `GET /players/{id}` endpoint. `IAppDbContext.GetPlayer(id)`
exists only internally. The only known Player today is the single seeded one
(`PlayerSeeder.DefaultPlayerId = 11111111-1111-1111-1111-111111111111`),
which the frontend uses as a temporary stand-in (`environment.defaultPlayerId`),
isolated in one place so it can be replaced once real player identity exists.

**Impact:** The client cannot support real multiplayer, cannot let a user "be"
a specific player through authentication, and cannot list a player's own
historical officers without client-side filtering of `GET /draft/officers`
by `playerId`.

**To unblock:** add a `GET /players/{id}` (and ideally `GET /players`) endpoint,
plus real per-caller identity (see gap #3) instead of a request-supplied `playerId`.

## 3. API key is not real authentication / has no player identity

`ApiKeyAuthenticationHandler` validates a single shared secret
(`Authentication:ApiKey` in `appsettings.json`) via the `X-Api-Key` header.
It is explicitly a development-only mechanism:
- There is no notion of a signed-in user.
- `StartOfficerCareerCommand` and `DraftCitizenCommand` both take identifiers
  (`playerId`, `recruitmentOfficerId`) as plain request parameters rather than
  deriving them from the caller's identity, so anyone with the key can act as
  any player or any officer.

**Impact:** The frontend does not treat the API key as a secret or as player
identity. It is committed in `environment.ts` only because the backend's own
`appsettings.json` already discloses the same dev-local value.

**To unblock (future multiplayer):** introduce real authentication (e.g.
cookie/JWT based per-player login) and derive `playerId` server-side from the
authenticated principal instead of trusting a client-supplied value.

## Not a gap, but worth noting: no CORS policy configured

`Program.cs` does not configure `AddCors`/`UseCors`. This does not block the
Angular app because the dev server proxies `/api/*` to the backend
(`proxy.conf.json`), avoiding cross-origin requests entirely during
development. A production deployment serving the Angular build from a
different origin than the API would need a CORS policy added to the backend.
