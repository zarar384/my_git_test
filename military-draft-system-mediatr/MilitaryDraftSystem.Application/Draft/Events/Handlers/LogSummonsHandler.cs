using MediatR;

namespace MilitaryDraftSystem.Application.Draft.Events.Handlers
{
    public class LogSummonsHandler : INotificationHandler<SummonsSentEvent>
    {
        public Task Handle(SummonsSentEvent notification, CancellationToken cancellationToken)
        {
            // Simulate logging logic (e.g., write to a log file, database, etc.)
            Console.WriteLine($"Summons sent to citizen with ID: {notification.CitizenId} for summons ID: {notification.SummonsId}");

            return Task.CompletedTask;
        }
    }
}
