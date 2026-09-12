using MediatR;
using Microsoft.Extensions.Logging;
using MilitaryDraftSystem.Application.Common.Interfaces;
using MilitaryDraftSystem.Domain.Entities;
using MilitaryDraftSystem.Domain.Enums;

namespace MilitaryDraftSystem.Application.Population.Events.Handlers
{
    /// <summary>
    /// Preserves a citizen's death as permanent history the moment it occurs:
    /// a cemetery record and an incremented death statistic counter. This
    /// runs before the citizen's row is ever physically removed from the
    /// population table, so their footprint in history and statistics never
    /// depends on that row still existing.
    /// </summary>
    public sealed class RecordCitizenDeathHistoryHandler : INotificationHandler<CitizenDiedEvent>
    {
        private readonly IAppDbContext _db;
        private readonly ILogger<RecordCitizenDeathHistoryHandler> _logger;

        public RecordCitizenDeathHistoryHandler(IAppDbContext db, ILogger<RecordCitizenDeathHistoryHandler> logger)
        {
            _db = db;
            _logger = logger;
        }

        public async Task Handle(CitizenDiedEvent notification, CancellationToken cancellationToken)
        {
            var citizen = await _db.GetCitizen(notification.CitizenId, cancellationToken);

            if (citizen is not null)
            {
                var existingRecord = await _db.GetCemeteryRecord(SubjectType.Citizen, citizen.Id, cancellationToken);

                if (existingRecord is null)
                {
                    _db.AddCemeteryRecord(CemeteryRecord.Create(
                        SubjectType.Citizen,
                        citizen.Id,
                        citizen.FullName,
                        notification.Death.Reason,
                        notification.Death.OccurredAt));
                }
            }

            var statistic = await _db.GetDeathStatistic(SubjectType.Citizen, notification.Death.Reason, cancellationToken);

            if (statistic is null)
            {
                statistic = DeathStatistic.Create(SubjectType.Citizen, notification.Death.Reason);
                _db.AddDeathStatistic(statistic);
            }

            statistic.Increment();

            await _db.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                "Recorded death history for citizen {CitizenId}. Cause: {DeathReason}.",
                notification.CitizenId,
                notification.Death.Reason);
        }
    }
}
