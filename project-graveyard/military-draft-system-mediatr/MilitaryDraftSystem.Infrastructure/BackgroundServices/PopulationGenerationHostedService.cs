using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MilitaryDraftSystem.Application.Population.Commands.RunPopulationGeneration;

namespace MilitaryDraftSystem.Infrastructure.BackgroundServices
{
    /// <summary>
    /// Periodically checks whether a new population generation should be executed.
    /// </summary>
    public sealed class PopulationGenerationHostedService(
        IServiceScopeFactory scopeFactory,
        ILogger<PopulationGenerationHostedService> logger)
        : BackgroundService
    {
        protected override async Task ExecuteAsync(
            CancellationToken stoppingToken)
        {
            logger.LogInformation("Population generation service started.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    // Create a new scope for this execution.
                    using var scope = scopeFactory.CreateScope();

                    var sender =
                        scope.ServiceProvider.GetRequiredService<ISender>();

                    // Execute the population generation workflow.
                    await sender.Send(
                        new RunPopulationGenerationCommand(),
                        stoppingToken);
                }
                catch (Exception ex)
                {
                    // Continue running even if a single execution fails.
                    logger.LogError(ex, "Population generation failed.");
                }

                // Wait before checking the generator again.
                await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
            }

            logger.LogInformation("Population generation service stopped.");
        }
    }
}