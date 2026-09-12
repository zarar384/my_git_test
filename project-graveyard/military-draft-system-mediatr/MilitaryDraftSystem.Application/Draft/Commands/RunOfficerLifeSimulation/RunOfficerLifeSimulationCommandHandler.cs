using MediatR;
using Microsoft.Extensions.Logging;
using MilitaryDraftSystem.Application.Common.Configuration;
using MilitaryDraftSystem.Application.Common.Interfaces;
using MilitaryDraftSystem.Domain.Enums;

namespace MilitaryDraftSystem.Application.Draft.Commands.RunOfficerLifeSimulation
{
    /// <summary>
    /// Rolls the dice of fate for every active recruitment officer, each tick:
    /// they may resign, die accidentally, take their own life, or die on duty.
    /// Only one outcome may occur per officer per tick, and every outcome is
    /// recorded as part of the officer's permanent history.
    /// </summary>
    public sealed class RunOfficerLifeSimulationCommandHandler
        : IRequestHandler<RunOfficerLifeSimulationCommand>
    {
        private static readonly DeathReason[] AccidentalCauses =
        [
            DeathReason.HeartAttack,
            DeathReason.TrafficAccident,
            DeathReason.UnknownIllness
        ];

        private static readonly DeathReason[] OnDutyCauses =
        [
            DeathReason.HeartAttack,
            DeathReason.FellFromStairs
        ];

        private readonly IAppDbContext _db;
        private readonly IRandomProvider _random;
        private readonly ILogger<RunOfficerLifeSimulationCommandHandler> _logger;

        public RunOfficerLifeSimulationCommandHandler(
            IAppDbContext db,
            IRandomProvider random,
            ILogger<RunOfficerLifeSimulationCommandHandler> logger)
        {
            _db = db;
            _random = random;
            _logger = logger;
        }

        public async Task Handle(
            RunOfficerLifeSimulationCommand request,
            CancellationToken cancellationToken)
        {
            var officers = await _db.GetActiveRecruitmentOfficers(cancellationToken);

            if (officers.Count == 0)
            {
                return;
            }

            var now = DateTimeOffset.UtcNow;
            var affectedCount = 0;

            foreach (var officer in officers)
            {
                if (_random.Chance(SimulationProbabilities.Officer.ResignationPercent))
                {
                    officer.Resign(now);
                    affectedCount++;
                    _logger.LogInformation("Recruitment officer {OfficerId} resigned.", officer.Id);
                    continue;
                }

                if (_random.Chance(SimulationProbabilities.Officer.AccidentalDeathPercent))
                {
                    officer.Die(_random.Pick(AccidentalCauses), now);
                    affectedCount++;
                    _logger.LogInformation("Recruitment officer {OfficerId} died accidentally.", officer.Id);
                    continue;
                }

                if (_random.Chance(SimulationProbabilities.Officer.SuicidePercent))
                {
                    officer.Die(DeathReason.Suicide, now);
                    affectedCount++;
                    _logger.LogInformation("Recruitment officer {OfficerId} took their own life.", officer.Id);
                    continue;
                }

                if (_random.Chance(SimulationProbabilities.Officer.DiedOnDutyPercent))
                {
                    officer.DieOnDuty(_random.Pick(OnDutyCauses), now);
                    affectedCount++;
                    _logger.LogInformation("Recruitment officer {OfficerId} died on duty.", officer.Id);
                }
            }

            if (affectedCount > 0)
            {
                await _db.SaveChangesAsync(cancellationToken);
            }

            _logger.LogInformation(
                "Officer life simulation finished. {AffectedCount} officer(s) changed.",
                affectedCount);
        }
    }
}
