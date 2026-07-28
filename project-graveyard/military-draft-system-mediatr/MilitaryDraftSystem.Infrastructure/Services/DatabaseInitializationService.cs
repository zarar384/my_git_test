using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MilitaryDraftSystem.Infrastructure.Persistence;
using MilitaryDraftSystem.Infrastructure.Persistence.Seeds;

namespace MilitaryDraftSystem.Infrastructure.Services
{
    public sealed class DatabaseInitializationService(
        IServiceProvider serviceProvider,
        IConfiguration configuration)
        : IHostedService
    {
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            using var scope = serviceProvider.CreateScope();

            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            // Apply all pending migrations.
            await db.Database.MigrateAsync(cancellationToken);

            // Seed database if enabled.
            if (configuration.GetValue<bool>("Database:Seed"))
            {
                await SeedData.SeedAsync(db);
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
