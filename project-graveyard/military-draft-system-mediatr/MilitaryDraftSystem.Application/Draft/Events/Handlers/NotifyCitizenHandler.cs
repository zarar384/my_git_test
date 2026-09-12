using MediatR;
using Microsoft.Extensions.Logging;

namespace MilitaryDraftSystem.Application.Draft.Events.Handlers
{
    public class NotifyCitizenHandler : INotificationHandler<SummonsSentEvent>
    {
        private readonly ILogger<NotifyCitizenHandler> _logger;

        public NotifyCitizenHandler(ILogger<NotifyCitizenHandler> logger)
        {
            _logger = logger;
        }

        public Task Handle(SummonsSentEvent notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation(
                "Notification sent to citizen with ID: {CitizenId} for summons ID: {SummonsId}",
                notification.CitizenId,
                notification.SummonsId);

            return Task.CompletedTask;
        }
    }
}
