namespace MilitaryDraftSystem.Domain.Events
{
    public record SummonsSentDomainEvent(Guid CitizenId, Guid SummonsId);
}
