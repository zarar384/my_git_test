using MediatR;
using Microsoft.Extensions.Logging;
using MilitaryDraftSystem.Application.Common.Configuration;
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
        private static readonly DeathReason[] DiseaseCauses =
        [
            DeathReason.HeartAttack,
            DeathReason.Cancer,
            DeathReason.UnknownIllness
        ];

        private static readonly DeathReason[] AccidentCauses =
        [
            DeathReason.TrafficAccident,
            DeathReason.Drowned,
            DeathReason.LightningStrike,
            DeathReason.FellFromStairs
        ];

        private static readonly DeathReason[] MilitaryCauses =
        [
            DeathReason.FriendlyFireDuringTraining,
            DeathReason.KilledInCombat
        ];

        private readonly IAppDbContext _db;
        private readonly IWorldNarrator _narrator;
        private readonly IRandomProvider _random;
        private readonly ILogger<RunPopulationSimulationCommandHandler> _logger;

        public RunPopulationSimulationCommandHandler(
            IAppDbContext db,
            IWorldNarrator narrator,
            IRandomProvider random,
            ILogger<RunPopulationSimulationCommandHandler> logger)
        {
            _db = db;
            _narrator = narrator;
            _random = random;
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
                if (_random.Chance(SimulationProbabilities.Birthday.OccursPercent))
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

            // Persist changes. Domain events raised above are published automatically
            // by DomainEventsInterceptor as part of this call.
            await _db.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                "Population simulation finished. {AffectedCount} citizen(s) changed.",
                affectedCitizens.Count);
        }

        /// <summary>
        /// Rolls the dice of fate for a single citizen and, if death occurs,
        /// returns the reason. Returns null if the citizen survives this tick.
        /// </summary>
        private DeathReason? TryDetermineDeathReason(Citizen citizen)
        {
            // Old age is triggered once the citizen reaches their individually
            // assigned natural lifespan, which already accounts for gender
            // differences in life expectancy and is never a fixed age.
            if (citizen.HasReachedNaturalLifespan() &&
                _random.Chance(
                    SimulationProbabilities.Death.OldAgeBasePercent +
                    ((citizen.Age - citizen.NaturalLifespanYears) * SimulationProbabilities.Death.OldAgePercentPerYearOver)))
            {
                return DeathReason.OldAge;
            }

            // Disease can strike at any age, but rarely.
            if (_random.Chance(SimulationProbabilities.Death.DiseasePercent))
            {
                return _random.Pick(DiseaseCauses);
            }

            // Accidents can happen to anyone.
            if (_random.Chance(SimulationProbabilities.Death.AccidentPercent))
            {
                return _random.Pick(AccidentCauses);
            }

            // Military service carries its own unique risks.
            if (citizen.Status == CitizenStatus.Drafted && _random.Chance(SimulationProbabilities.Death.MilitaryPercent))
            {
                return _random.Pick(MilitaryCauses);
            }

            // Life is not always kind.
            if (_random.Chance(SimulationProbabilities.Death.SuicidePercent))
            {
                return DeathReason.Suicide;
            }

            return null;
        }
    }
}
