using MediatR;

namespace MilitaryDraftSystem.Application.Draft.Commands.OfficerLifecycle
{
    /// <summary>
    /// The recruitment officer voluntarily quits their job. Their career ends
    /// and the player controlling them may start a new game with a new officer.
    /// </summary>
    public sealed record ResignCommand(Guid RecruitmentOfficerId) : IRequest;
}
