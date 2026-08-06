using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MilitaryDraftSystem.Application.Population.Commands.RunPopulationSimulation;

namespace MilitaryDraftSystem.Infrastructure.BackgroundServices
{
    /// <summary>
    /// Periodically simulates the passage of time for the living population:
    /// aging, birthdays and the many ways a life may unexpectedly end.
    /// </summary>
    public sealed class PopulationSimulationHostedService(
        IServiceScopeFactory scopeFactory,
        ILogger<PopulationSimulationHostedService> logger)
        : BackgroundService
    {
        protected override async Task ExecuteAsync(
            CancellationToken stoppingToken)
        {
            logger.LogInformation("Population simulation service started.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    // Create a new scope for this execution.
                    using var scope = scopeFactory.CreateScope();

                    var sender = scope.ServiceProvider.GetRequiredService<ISender>();

                    // Execute one tick of the world simulation.
                    await sender.Send(
                        new RunPopulationSimulationCommand(),
                        stoppingToken);
                }
                catch (Exception ex)
                {
                    // Continue running even if a single execution fails.
                    logger.LogError(ex, "Population simulation execution failed.");
                }

                // Wait before simulating the next tick.
                await Task.Delay(TimeSpan.FromSeconds(15), stoppingToken);
            }

            logger.LogInformation("Population simulation service stopped.");
        }
    }
}
