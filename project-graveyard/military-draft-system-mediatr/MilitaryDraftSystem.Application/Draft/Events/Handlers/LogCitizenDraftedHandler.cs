using MediatR;
using Microsoft.Extensions.Logging;

namespace MilitaryDraftSystem.Application.Draft.Events.Handlers
{
    public class LogCitizenDraftedHandler : INotificationHandler<CitizenDraftedEvent>
    {
        private readonly ILogger<LogCitizenDraftedHandler> _logger;

        public LogCitizenDraftedHandler(ILogger<LogCitizenDraftedHandler> logger)
        {
            _logger = logger;
        }

        public Task Handle(CitizenDraftedEvent notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Citizen {CitizenId} was drafted.", notification.CitizenId);

            return Task.CompletedTask;
        }
    }
}
