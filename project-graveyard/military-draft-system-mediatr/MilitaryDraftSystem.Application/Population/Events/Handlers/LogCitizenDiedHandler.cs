using MediatR;
using Microsoft.Extensions.Logging;

namespace MilitaryDraftSystem.Application.Population.Events.Handlers
{
    public class LogCitizenDiedHandler : INotificationHandler<CitizenDiedEvent>
    {
        private readonly ILogger<LogCitizenDiedHandler> _logger;

        public LogCitizenDiedHandler(ILogger<LogCitizenDiedHandler> logger)
        {
            _logger = logger;
        }

        public Task Handle(CitizenDiedEvent notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation(
                "Citizen {CitizenId} died. Cause: {DeathReason}.",
                notification.CitizenId,
                notification.Death.Reason);

            return Task.CompletedTask;
        }
    }
}
