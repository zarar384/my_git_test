using MediatR;

namespace MilitaryDraftSystem.Application.Population.Events
{
    public record CitizenBecameAdultEvent(Guid CitizenId) : INotification;
}
