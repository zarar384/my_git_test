using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MilitaryDraftSystem.Infrastructure.Persistence;
using MilitaryDraftSystem.Infrastructure.Persistence.Seeds;

namespace MilitaryDraftSystem.Infrastructure.Services
{
    public sealed class DatabaseInitializationService(
        IServiceProvider serviceProvider,
        IConfiguration configuration,
        ILogger<DatabaseInitializationService> logger)
        : IHostedService
    {
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            using var scope = serviceProvider.CreateScope();

            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            try
            {
                logger.LogInformation("Applying pending database migrations.");

                // Apply all pending migrations.
                await db.Database.MigrateAsync(cancellationToken);

                logger.LogInformation("Database migrations applied successfully.");

                // Seed database if enabled.
                if (configuration.GetValue<bool>("Database:Seed"))
                {
                    logger.LogInformation("Seeding database with initial data.");

                    await SeedData.SeedAsync(db);

                    logger.LogInformation("Database seeding completed.");
                }
            }
            catch (Exception ex)
            {
                // Startup must not silently continue with an un-migrated
                // database: log with full context and rethrow so the host
                // fails fast.
                logger.LogCritical(
                    ex,
                    "Database initialization failed. The application cannot start.");

                throw;
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
