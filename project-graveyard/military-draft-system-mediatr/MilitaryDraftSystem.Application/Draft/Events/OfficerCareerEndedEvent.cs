using MediatR;
using MilitaryDraftSystem.Domain.Enums;

namespace MilitaryDraftSystem.Application.Draft.Events
{
    public record OfficerCareerEndedEvent(Guid OfficerId, WorkerEndReason Reason) : INotification;
}
