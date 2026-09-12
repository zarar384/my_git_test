using MediatR;

namespace MilitaryDraftSystem.Application.Draft.Commands.OfficerLifecycle
{
    /// <summary>
    /// Returns a recruitment officer to active duty from leave.
    /// </summary>
    public sealed record ReturnFromLeaveCommand(Guid RecruitmentOfficerId) : IRequest;
}
