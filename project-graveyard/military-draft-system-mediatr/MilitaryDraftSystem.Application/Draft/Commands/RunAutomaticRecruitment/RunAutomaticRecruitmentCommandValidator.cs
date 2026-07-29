using FluentValidation;

namespace MilitaryDraftSystem.Application.Draft.Commands.RunAutomaticRecruitment
{
    // This command does not contain any input to validate.
    public sealed class RunAutomaticRecruitmentCommandValidator
        : AbstractValidator<RunAutomaticRecruitmentCommand>
    {
    }
}