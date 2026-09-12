using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MilitaryDraftSystem.Application.Population.Commands.PurgeDeceasedCitizens;

namespace MilitaryDraftSystem.Infrastructure.BackgroundServices
{
    /// <summary>
    /// Periodically purges deceased citizens from the population table once
    /// their retention window has elapsed, keeping the living-population table
    /// lean without ever losing their history: cemetery records and death
    /// statistics are recorded separately, at the moment of death.
    /// </summary>
    public sealed class CitizenCleanupHostedService(
        IServiceScopeFactory scopeFactory,
        ILogger<CitizenCleanupHostedService> logger)
        : BackgroundService
    {
        protected override async Task ExecuteAsync(
            CancellationToken stoppingToken)
        {
            logger.LogInformation("Citizen cleanup service started.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = scopeFactory.CreateScope();

                    var sender = scope.ServiceProvider.GetRequiredService<ISender>();

                    await sender.Send(
                        new PurgeDeceasedCitizensCommand(),
                        stoppingToken);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Citizen cleanup execution failed.");
                }

                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }

            logger.LogInformation("Citizen cleanup service stopped.");
        }
    }
}
