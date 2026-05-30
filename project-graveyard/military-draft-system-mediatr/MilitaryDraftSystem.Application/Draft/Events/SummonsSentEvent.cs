using MediatR;

namespace MilitaryDraftSystem.Application.Draft.Events
{
    public record SummonsSentEvent(Guid CitizenId, Guid SummonsId): INotification;
}
