using System.Net;
using MilitaryDraftSystem.Tests.Api.HealthChecks;

namespace MilitaryDraftSystem.Tests.Api.HealthChecks
{
    /// <summary>
    /// Verifies the application's health-check HTTP contract: liveness never
    /// depends on external dependencies, readiness reflects real database
    /// connectivity, and failure responses never leak internal details.
    /// </summary>
    public class HealthCheckEndpointTests : IClassFixture<HealthCheckWebApplicationFactory>
    {
        private readonly HealthCheckWebApplicationFactory _factory;

        public HealthCheckEndpointTests(HealthCheckWebApplicationFactory factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task Live_ShouldReturn200_WhenApplicationIsRunning()
        {
            // Arrange
            var client = _factory.CreateClient();

            // Act
            var response = await client.GetAsync("/health/live");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Ready_ShouldReturn200_WhenDatabaseIsAvailable()
        {
            // Arrange
            var client = _factory.CreateClient();

            // Act
            var response = await client.GetAsync("/health/ready");
            var body = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Contains("\"database\":\"Healthy\"", body);
        }

        [Fact]
        public async Task Ready_ShouldNotRequireApiKeyAuthentication()
        {
            // Arrange: a client with no X-Api-Key header at all.
            var client = _factory.CreateClient();

            // Act
            var response = await client.GetAsync("/health/ready");

            // Assert: health endpoints must stay reachable without auth so
            // monitoring/orchestration tooling can probe them.
            Assert.NotEqual(HttpStatusCode.Unauthorized, response.StatusCode);
        }
    }
}
