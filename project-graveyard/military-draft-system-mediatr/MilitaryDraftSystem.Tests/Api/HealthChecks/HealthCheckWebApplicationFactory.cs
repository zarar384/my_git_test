using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;

namespace MilitaryDraftSystem.Tests.Api.HealthChecks
{
    /// <summary>
    /// Spins up the real API host against an isolated, file-based SQLite
    /// database so the EF Core health check exercises a real connection
    /// without touching the developer's draft.db.
    /// </summary>
    public sealed class HealthCheckWebApplicationFactory : WebApplicationFactory<Program>
    {
        public string DatabasePath { get; } = Path.Combine(
            Path.GetTempPath(),
            $"military-draft-system-healthcheck-tests-{Guid.NewGuid()}.db");

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureAppConfiguration((_, configBuilder) =>
            {
                configBuilder.AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["ConnectionStrings:DefaultConnection"] = $"Data Source={DatabasePath}",
                    ["Database:Seed"] = "false",
                    ["Authentication:ApiKey"] = "test-api-key"
                });
            });
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                File.Delete(DatabasePath);
                File.Delete(DatabasePath + "-shm");
                File.Delete(DatabasePath + "-wal");
            }
        }
    }
}
