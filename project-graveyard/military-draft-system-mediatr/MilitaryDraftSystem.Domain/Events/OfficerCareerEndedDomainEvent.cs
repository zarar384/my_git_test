using MilitaryDraftSystem.Domain.Enums;
using MilitaryDraftSystem.Domain.Interfaces;

namespace MilitaryDraftSystem.Domain.Events
{
    /// <summary>
    /// Raised whenever a recruitment officer's active career ends or pauses
    /// (resignation, retirement, dismissal, leave), for reasons other than death,
    /// which is covered separately by <see cref="OfficerDiedDomainEvent"/>.
    /// </summary>
    public sealed record OfficerCareerEndedDomainEvent(Guid OfficerId, WorkerEndReason Reason) : IDomainEvent;
}
