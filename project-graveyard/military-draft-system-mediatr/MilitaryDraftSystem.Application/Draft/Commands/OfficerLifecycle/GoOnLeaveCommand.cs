using MediatR;

namespace MilitaryDraftSystem.Application.Draft.Commands.OfficerLifecycle
{
    /// <summary>
    /// Sends a recruitment officer on temporary leave. They stop being able to
    /// draft citizens but may return to active duty later.
    /// </summary>
    public sealed record GoOnLeaveCommand(Guid RecruitmentOfficerId) : IRequest;
}
