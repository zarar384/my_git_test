using MediatR;

namespace MilitaryDraftSystem.Application.Draft.Commands.OfficerLifecycle
{
    /// <summary>
    /// The recruitment officer is dismissed from duty. Their career ends.
    /// </summary>
    public sealed record FireOfficerCommand(Guid RecruitmentOfficerId) : IRequest;
}
