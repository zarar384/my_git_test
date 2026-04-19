using MediatR;

namespace MilitaryDraftSystem.Application.Draft.Commands.SendSummons
{
    // Command to send a summons to a citizen
    public record SendSummonsCommand(Guid CitizenId, DateTime Date) : IRequest;
}
