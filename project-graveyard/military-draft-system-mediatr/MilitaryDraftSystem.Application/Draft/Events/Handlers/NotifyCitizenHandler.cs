using MediatR;

namespace MilitaryDraftSystem.Application.Draft.Events.Handlers
{
    public class NotifyCitizenHandler : INotificationHandler<SummonsSentEvent>
    {
        public Task Handle(SummonsSentEvent notification, CancellationToken cancellationToken)
        {
            // Simulate notification logic (e.g., send email, SMS, etc.)
            Console.WriteLine($"Notification sent to citizen with ID: {notification.CitizenId} for summons ID: {notification.SummonsId}");

            return Task.CompletedTask;
        }
    }
}
