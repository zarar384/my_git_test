using MediatR;

namespace MilitaryDraftSystem.Application.Draft.Commands.RunAutomaticRecruitment
{
    // Executes the automatic recruitment process.
    public sealed record RunAutomaticRecruitmentCommand : IRequest;
}
