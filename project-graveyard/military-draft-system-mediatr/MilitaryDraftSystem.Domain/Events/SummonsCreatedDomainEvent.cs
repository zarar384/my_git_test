using MilitaryDraftSystem.Domain.Interfaces;

namespace MilitaryDraftSystem.Domain.Events
{
    public sealed record SummonsCreatedDomainEvent(Guid SummonsId, Guid CitizenId) : IDomainEvent;
}
