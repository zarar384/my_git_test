using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MilitaryDraftSystem.Application.Draft.Commands.RunOfficerLifeSimulation;

namespace MilitaryDraftSystem.Infrastructure.BackgroundServices
{
    /// <summary>
    /// Periodically simulates the passage of time for every active recruitment
    /// officer: resignation, accidental death, suicide, or death on duty.
    /// </summary>
    public sealed class OfficerLifeSimulationHostedService(
        IServiceScopeFactory scopeFactory,
        ILogger<OfficerLifeSimulationHostedService> logger)
        : BackgroundService
    {
        protected override async Task ExecuteAsync(
            CancellationToken stoppingToken)
        {
            logger.LogInformation("Officer life simulation service started.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = scopeFactory.CreateScope();

                    var sender = scope.ServiceProvider.GetRequiredService<ISender>();

                    await sender.Send(
                        new RunOfficerLifeSimulationCommand(),
                        stoppingToken);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Officer life simulation execution failed.");
                }

                await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
            }

            logger.LogInformation("Officer life simulation service stopped.");
        }
    }
}
