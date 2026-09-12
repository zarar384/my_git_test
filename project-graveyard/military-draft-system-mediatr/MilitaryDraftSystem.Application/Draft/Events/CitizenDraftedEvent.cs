using MediatR;

namespace MilitaryDraftSystem.Application.Draft.Events
{
    public record CitizenDraftedEvent(Guid CitizenId) : INotification;
}
