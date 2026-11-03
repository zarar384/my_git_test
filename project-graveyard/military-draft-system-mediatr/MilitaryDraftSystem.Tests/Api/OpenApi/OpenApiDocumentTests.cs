using System.Net;
using MilitaryDraftSystem.Tests.Api.HealthChecks;

namespace MilitaryDraftSystem.Tests.Api.OpenApi
{
    /// <summary>
    /// Verifies the built-in ASP.NET Core OpenAPI document is published when
    /// running in Development (the environment used by
    /// <see cref="HealthCheckWebApplicationFactory"/>), reflects the real API
    /// shape, and documents the "X-Api-Key" authentication scheme used by the
    /// protected controllers.
    /// </summary>
    [Collection(MilitaryDraftSystem.Tests.Api.WebApplicationFactoryCollection.Name)]
    public class OpenApiDocumentTests : IClassFixture<HealthCheckWebApplicationFactory>
    {
        private readonly HealthCheckWebApplicationFactory _factory;

        public OpenApiDocumentTests(HealthCheckWebApplicationFactory factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task OpenApiDocument_ShouldBeAvailable_InDevelopment()
        {
            // Arrange
            var client = _factory.CreateClient();

            // Act
            var response = await client.GetAsync("/openapi/v1.json");
            var body = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Contains("\"/draft/officers\"", body);
            Assert.Contains("\"/population/cemetery\"", body);
            Assert.Contains(ApiKeySecuritySchemeId, body);
        }

        private const string ApiKeySecuritySchemeId = "ApiKey";
    }
}
