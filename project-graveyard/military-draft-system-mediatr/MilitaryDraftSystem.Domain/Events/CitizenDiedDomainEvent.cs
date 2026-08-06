using MilitaryDraftSystem.Domain.Interfaces;
using MilitaryDraftSystem.Domain.ValueObjects;

namespace MilitaryDraftSystem.Domain.Events
{
    public sealed record CitizenDiedDomainEvent(Guid CitizenId, Death Death) : IDomainEvent;
}
