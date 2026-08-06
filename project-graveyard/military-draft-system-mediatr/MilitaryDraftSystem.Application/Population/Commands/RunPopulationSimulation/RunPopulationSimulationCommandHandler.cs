using MediatR;
using Microsoft.Extensions.Logging;
using MilitaryDraftSystem.Application.Common.Interfaces;
using MilitaryDraftSystem.Domain.Entities;
using MilitaryDraftSystem.Domain.Enums;

namespace MilitaryDraftSystem.Application.Population.Commands.RunPopulationSimulation
{
    /// <summary>
    /// Simulates the passage of time for the living population: birthdays, aging
    /// and the many ways a citizen's life may unexpectedly come to an end.
    /// </summary>
    public sealed class RunPopulationSimulationCommandHandler
        : IRequestHandler<RunPopulationSimulationCommand>
    {
        private static readonly Random Random = Random.Shared;

        private readonly IAppDbContext _db;
        private readonly IMediator _mediator;
        private readonly IWorldNarrator _narrator;
        private readonly ILogger<RunPopulationSimulationCommandHandler> _logger;

        public RunPopulationSimulationCommandHandler(
            IAppDbContext db,
            IMediator mediator,
            IWorldNarrator narrator,
            ILogger<RunPopulationSimulationCommandHandler> logger)
        {
            _db = db;
            _mediator = mediator;
            _narrator = narrator;
            _logger = logger;
        }

        public async Task Handle(
            RunPopulationSimulationCommand request,
            CancellationToken cancellationToken)
        {
            var citizens = await _db.GetLivingCitizens(cancellationToken);

            if (citizens.Count == 0)
            {
                return;
            }

            _logger.LogInformation("Population simulation started for {CitizenCount} living citizen(s).", citizens.Count);

            var now = DateTimeOffset.UtcNow;
            var affectedCitizens = new List<Citizen>();

            foreach (var citizen in citizens)
            {
                var changed = false;

                // Small chance of a birthday happening during this simulation tick.
                if (Chance(5))
                {
                    citizen.HaveBirthday();
                    changed = true;

                    if (citizen.Age == Citizen.AdultAge)
                    {
                        _logger.LogInformation("Citizen {CitizenId} became an adult.", citizen.Id);
                        _narrator.CitizenBecameAdult(citizen);
                    }
                }

                // Roll for death. Older citizens and drafted soldiers face higher risk.
                var reason = TryDetermineDeathReason(citizen);

                if (reason is not null)
                {
                    citizen.Die(reason.Value, now);
                    changed = true;

                    _logger.LogInformation(
                        "Citizen {CitizenId} died. Cause: {DeathReason}.",
                        citizen.Id,
                        reason.Value);

                    _narrator.CitizenDied(citizen, citizen.Death!);
                }

                if (changed)
                {
                    affectedCitizens.Add(citizen);
                }
            }

            await _db.SaveChangesAsync(cancellationToken);

            foreach (var citizen in affectedCitizens)
            {
                foreach (var domainEvent in citizen.DomainEvents)
                {
                    await _mediator.Publish(domainEvent, cancellationToken);
                }

                citizen.ClearDomainEvents();
            }

            _logger.LogInformation(
                "Population simulation finished. {AffectedCount} citizen(s) changed.",
                affectedCitizens.Count);
        }

        /// <summary>
        /// Rolls the dice of fate for a single citizen and, if death occurs,
        /// returns the reason. Returns null if the citizen survives this tick.
        /// </summary>
        private static DeathReason? TryDetermineDeathReason(Citizen citizen)
        {
            // Old age becomes an increasing risk past 70.
            if (citizen.Age > 70 && Chance(1 + (citizen.Age - 70)))
            {
                return DeathReason.OldAge;
            }

            // Disease can strike at any age, but rarely.
            if (Chance(1))
            {
                return Pick(DeathReason.HeartAttack, DeathReason.Cancer, DeathReason.UnknownIllness);
            }

            // Accidents can happen to anyone.
            if (Chance(1))
            {
                return Pick(
                    DeathReason.TrafficAccident,
                    DeathReason.Drowned,
                    DeathReason.LightningStrike,
                    DeathReason.FellFromStairs);
            }

            // Military service carries its own unique risks.
            if (citizen.Status == CitizenStatus.Drafted && Chance(2))
            {
                return Pick(DeathReason.FriendlyFireDuringTraining, DeathReason.KilledInCombat);
            }

            // Life is not always kind.
            if (Chance(1))
            {
                return DeathReason.Suicide;
            }

            return null;
        }

        private static bool Chance(int percent)
        {
            return Random.Next(100) < percent;
        }

        private static DeathReason Pick(params DeathReason[] reasons)
        {
            return reasons[Random.Next(reasons.Length)];
        }
    }
}
