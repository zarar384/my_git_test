using MilitaryDraftSystem.Domain.Enums;
using MilitaryDraftSystem.Domain.Interfaces;
using MilitaryDraftSystem.Domain.ValueObjects;

namespace MilitaryDraftSystem.Domain.Events
{
    public sealed record OfficerDiedDomainEvent(Guid OfficerId, Death Death, WorkerEndReason Reason) : IDomainEvent;
}
