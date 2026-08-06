using MilitaryDraftSystem.Domain.Interfaces;

namespace MilitaryDraftSystem.Domain.Events
{
    public sealed record CitizenBecameAdultDomainEvent(Guid CitizenId) : IDomainEvent;
}
