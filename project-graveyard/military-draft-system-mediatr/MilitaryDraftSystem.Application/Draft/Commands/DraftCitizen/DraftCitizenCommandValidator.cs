using FluentValidation;

namespace MilitaryDraftSystem.Application.Draft.Commands.DraftCitizen
{
    public sealed class DraftCitizenCommandValidator : AbstractValidator<DraftCitizenCommand>
    {
        public DraftCitizenCommandValidator()
        {
            RuleFor(x => x.CitizenId)
                .NotEmpty();

            RuleFor(x => x.RecruitmentOfficerId)
                .NotEmpty();
        }
    }
}
