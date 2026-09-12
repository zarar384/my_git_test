using MediatR;
using Microsoft.Extensions.Logging;
using MilitaryDraftSystem.Application.Common.Interfaces;
using MilitaryDraftSystem.Application.Draft.Events;
using MilitaryDraftSystem.Domain.Entities;
using MilitaryDraftSystem.Domain.Enums;

namespace MilitaryDraftSystem.Application.Draft.Events.Handlers
{
    /// <summary>
    /// Preserves a recruitment officer's death as permanent history: a
    /// cemetery record, a death statistic counter, and a worker life-cycle
    /// statistic counter. Deactivating or later removing the officer never
    /// erases this footprint.
    /// </summary>
    public sealed class RecordOfficerDeathHistoryHandler : INotificationHandler<OfficerDiedEvent>
    {
        private readonly IAppDbContext _db;
        private readonly ILogger<RecordOfficerDeathHistoryHandler> _logger;

        public RecordOfficerDeathHistoryHandler(IAppDbContext db, ILogger<RecordOfficerDeathHistoryHandler> logger)
        {
            _db = db;
            _logger = logger;
        }

        public async Task Handle(OfficerDiedEvent notification, CancellationToken cancellationToken)
        {
            var officer = await _db.GetRecruitmentOfficer(notification.OfficerId, cancellationToken);

            if (officer is not null)
            {
                var existingRecord = await _db.GetCemeteryRecord(SubjectType.RecruitmentOfficer, officer.Id, cancellationToken);

                if (existingRecord is null)
                {
                    _db.AddCemeteryRecord(CemeteryRecord.Create(
                        SubjectType.RecruitmentOfficer,
                        officer.Id,
                        officer.FullName,
                        notification.Death.Reason,
                        notification.Death.OccurredAt));
                }
            }

            var deathStatistic = await _db.GetDeathStatistic(SubjectType.RecruitmentOfficer, notification.Death.Reason, cancellationToken);

            if (deathStatistic is null)
            {
                deathStatistic = DeathStatistic.Create(SubjectType.RecruitmentOfficer, notification.Death.Reason);
                _db.AddDeathStatistic(deathStatistic);
            }

            deathStatistic.Increment();

            var lifecycleStatistic = await _db.GetWorkerLifecycleStatistic(notification.Reason, cancellationToken);

            if (lifecycleStatistic is null)
            {
                lifecycleStatistic = WorkerLifecycleStatistic.Create(notification.Reason);
                _db.AddWorkerLifecycleStatistic(lifecycleStatistic);
            }

            lifecycleStatistic.Increment();

            await _db.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                "Recorded death history for recruitment officer {OfficerId}. Cause: {DeathReason}.",
                notification.OfficerId,
                notification.Death.Reason);
        }
    }

    /// <summary>
    /// Preserves the end (or pause) of a recruitment officer's career as
    /// permanent history via the worker life-cycle statistics. Deaths are
    /// handled separately by <see cref="RecordOfficerDeathHistoryHandler"/>.
    /// </summary>
    public sealed class RecordOfficerCareerEndedHistoryHandler : INotificationHandler<OfficerCareerEndedEvent>
    {
        private readonly IAppDbContext _db;
        private readonly ILogger<RecordOfficerCareerEndedHistoryHandler> _logger;

        public RecordOfficerCareerEndedHistoryHandler(IAppDbContext db, ILogger<RecordOfficerCareerEndedHistoryHandler> logger)
        {
            _db = db;
            _logger = logger;
        }

        public async Task Handle(OfficerCareerEndedEvent notification, CancellationToken cancellationToken)
        {
            var statistic = await _db.GetWorkerLifecycleStatistic(notification.Reason, cancellationToken);

            if (statistic is null)
            {
                statistic = WorkerLifecycleStatistic.Create(notification.Reason);
                _db.AddWorkerLifecycleStatistic(statistic);
            }

            statistic.Increment();

            await _db.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                "Recruitment officer {OfficerId} career transition recorded. Reason: {Reason}.",
                notification.OfficerId,
                notification.Reason);
        }
    }
}
