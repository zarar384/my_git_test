using MediatR;
using Microsoft.Extensions.Logging;
using MilitaryDraftSystem.Application.Common.Configuration;
using MilitaryDraftSystem.Application.Common.Interfaces;
using MilitaryDraftSystem.Domain.Enums;

namespace MilitaryDraftSystem.Application.Population.Events.Handlers
{
    /// <summary>
    /// Applies a psychological toll to the recruitment officer who personally
    /// drafted a citizen, whenever that citizen dies in military service.
    /// Automatic recruitment agents have no psychology, so only human-drafted
    /// citizens can trigger this consequence.
    /// </summary>
    public class ApplyOfficerGuiltHandler : INotificationHandler<CitizenDiedEvent>
    {
        private static readonly DeathReason[] MilitaryCauses =
        [
            DeathReason.FriendlyFireDuringTraining,
            DeathReason.KilledInCombat
        ];

        private readonly IAppDbContext _db;
        private readonly ILogger<ApplyOfficerGuiltHandler> _logger;

        public ApplyOfficerGuiltHandler(IAppDbContext db, ILogger<ApplyOfficerGuiltHandler> logger)
        {
            _db = db;
            _logger = logger;
        }

        public async Task Handle(CitizenDiedEvent notification, CancellationToken cancellationToken)
        {
            if (!MilitaryCauses.Contains(notification.Death.Reason))
                return;

            var summons = await _db.GetSummonsByCitizen(notification.CitizenId, cancellationToken);

            if (summons?.RecruitmentOfficerId is not Guid officerId)
                return;

            var officer = await _db.GetRecruitmentOfficer(officerId, cancellationToken);

            if (officer is null)
                return;

            officer.ApplyGuilt(SimulationProbabilities.OfficerConsequences.MoraleLossPerMilitaryDeath);

            await _db.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                "Recruitment officer {OfficerId} bears guilt for the death of citizen {CitizenId} they drafted. Morale is now {MoralePercent}%.",
                officer.Id,
                notification.CitizenId,
                officer.MoralePercent);
        }
    }
}
