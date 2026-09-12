using MediatR;
using MilitaryDraftSystem.Domain.Enums;
using MilitaryDraftSystem.Domain.ValueObjects;

namespace MilitaryDraftSystem.Application.Draft.Events
{
    public record OfficerDiedEvent(Guid OfficerId, Death Death, WorkerEndReason Reason) : INotification;
}
