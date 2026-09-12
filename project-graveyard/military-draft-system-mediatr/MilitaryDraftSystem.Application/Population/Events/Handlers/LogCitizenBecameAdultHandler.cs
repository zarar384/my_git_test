using MediatR;
using Microsoft.Extensions.Logging;

namespace MilitaryDraftSystem.Application.Population.Events.Handlers
{
    public class LogCitizenBecameAdultHandler : INotificationHandler<CitizenBecameAdultEvent>
    {
        private readonly ILogger<LogCitizenBecameAdultHandler> _logger;

        public LogCitizenBecameAdultHandler(ILogger<LogCitizenBecameAdultHandler> logger)
        {
            _logger = logger;
        }

        public Task Handle(CitizenBecameAdultEvent notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Citizen {CitizenId} became an adult.", notification.CitizenId);

            return Task.CompletedTask;
        }
    }
}
