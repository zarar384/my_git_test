using FluentValidation;

namespace MilitaryDraftSystem.Application.Draft.Commands.SendSummons
{
    public class SendSummonsValidator: AbstractValidator<SendSummonsCommand>
    {
        public SendSummonsValidator()
        {
            // citizen ID must not be empty
            RuleFor(x => x.CitizenId)
                .NotEmpty();

            // date must not be empty and must be in the future
            RuleFor(x => x.Date)
                .NotEmpty()
                .GreaterThan(DateTime.UtcNow);
        }
    }
}
