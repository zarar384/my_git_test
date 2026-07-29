using MilitaryDraftSystem.Domain.Interfaces;

namespace MilitaryDraftSystem.Domain.Events
{
    public sealed record CitizenDraftedDomainEvent(Guid CitizenId) : IDomainEvent;
}
