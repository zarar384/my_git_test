using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace MilitaryDraftSystem.API.Middleware
{
    /// <summary>
    /// Renders health check results as a small, safe JSON payload: overall
    /// status plus the status of each check by name. Never includes
    /// exception details, descriptions, or other diagnostic data that could
    /// leak internal implementation details.
    /// </summary>
    public static class HealthCheckResponseWriter
    {
        private static readonly JsonSerializerOptions SerializerOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        public static Task WriteAsync(HttpContext context, HealthReport report)
        {
            context.Response.ContentType = "application/json";

            var payload = new
            {
                status = report.Status.ToString(),
                checks = report.Entries.ToDictionary(
                    entry => entry.Key,
                    entry => entry.Value.Status.ToString())
            };

            return context.Response.WriteAsync(JsonSerializer.Serialize(payload, SerializerOptions));
        }
    }
}
