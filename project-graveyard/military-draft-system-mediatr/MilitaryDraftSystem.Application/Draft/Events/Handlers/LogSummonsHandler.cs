using MediatR;
using Microsoft.Extensions.Logging;

namespace MilitaryDraftSystem.Application.Draft.Events.Handlers
{
    public class LogSummonsHandler : INotificationHandler<SummonsSentEvent>
    {
        private readonly ILogger<LogSummonsHandler> _logger;

        public LogSummonsHandler(ILogger<LogSummonsHandler> logger)
        {
            _logger = logger;
        }

        public Task Handle(SummonsSentEvent notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation(
                "Summons sent to citizen with ID: {CitizenId} for summons ID: {SummonsId}",
                notification.CitizenId,
                notification.SummonsId);

            return Task.CompletedTask;
        }
    }
}
