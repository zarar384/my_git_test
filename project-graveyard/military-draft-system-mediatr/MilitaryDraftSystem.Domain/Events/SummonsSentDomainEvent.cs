using MilitaryDraftSystem.Domain.Interfaces;

namespace MilitaryDraftSystem.Domain.Events
{
    public record SummonsSentDomainEvent(Guid CitizenId, Guid SummonsId) : IDomainEvent;
}
