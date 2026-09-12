using MediatR;
using MilitaryDraftSystem.Domain.ValueObjects;

namespace MilitaryDraftSystem.Application.Population.Events
{
    public record CitizenDiedEvent(Guid CitizenId, Death Death) : INotification;
}
