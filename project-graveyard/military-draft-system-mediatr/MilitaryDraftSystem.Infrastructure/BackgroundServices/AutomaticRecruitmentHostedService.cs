using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MilitaryDraftSystem.Application.Draft.Commands.RunAutomaticRecruitment;

namespace MilitaryDraftSystem.Infrastructure.BackgroundServices
{
    /// <summary>
    /// Periodically executes the automatic recruitment process in the background.
    /// </summary>
    public sealed class AutomaticRecruitmentHostedService(
        IServiceScopeFactory scopeFactory,
        ILogger<AutomaticRecruitmentHostedService> logger)
        : BackgroundService
    {
        protected override async Task ExecuteAsync(
            CancellationToken stoppingToken)
        {
            logger.LogInformation("Automatic recruitment service started.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    // Create a new scope for this execution.
                    using var scope = scopeFactory.CreateScope();

                    var mediator = scope.ServiceProvider.GetRequiredService<ISender>();

                    // Execute the automatic recruitment workflow.
                    await mediator.Send(
                        new RunAutomaticRecruitmentCommand(),
                        stoppingToken);
                }
                catch (Exception ex)
                {
                    // Log the error and continue with the next scheduled execution.
                    logger.LogError(ex, "Automatic recruitment execution failed.");
                }

                // Wait before starting the next execution cycle.
                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }

            logger.LogInformation("Automatic recruitment service stopped.");
        }
    }
}
