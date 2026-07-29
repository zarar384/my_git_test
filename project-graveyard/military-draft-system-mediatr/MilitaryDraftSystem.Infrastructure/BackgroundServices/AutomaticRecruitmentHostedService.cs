using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MilitaryDraftSystem.Application.Draft.Commands.RunAutomaticRecruitment;

namespace MilitaryDraftSystem.Infrastructure.BackgroundServices
{
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
                    using var scope = scopeFactory.CreateScope();

                    var mediator =
                        scope.ServiceProvider.GetRequiredService<ISender>();

                    await mediator.Send(
                        new RunAutomaticRecruitmentCommand(),
                        stoppingToken);
                }
                catch (Exception ex)
                {
                    logger.LogError(
                        ex,
                        "Automatic recruitment execution failed.");
                }

                await Task.Delay(
                    TimeSpan.FromMinutes(1),
                    stoppingToken);
            }

            logger.LogInformation("Automatic recruitment service stopped.");
        }
    }
}
