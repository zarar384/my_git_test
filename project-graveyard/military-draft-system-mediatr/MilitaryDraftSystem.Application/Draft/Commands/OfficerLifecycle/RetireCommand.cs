using MediatR;

namespace MilitaryDraftSystem.Application.Draft.Commands.OfficerLifecycle
{
    /// <summary>
    /// The recruitment officer voluntarily retires. Their career ends.
    /// </summary>
    public sealed record RetireCommand(Guid RecruitmentOfficerId) : IRequest;
}
