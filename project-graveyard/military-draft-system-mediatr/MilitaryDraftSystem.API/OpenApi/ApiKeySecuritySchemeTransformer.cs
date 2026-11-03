using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;
using MilitaryDraftSystem.API.Auth;

namespace MilitaryDraftSystem.API.OpenApi
{
    /// <summary>
    /// Registers the "X-Api-Key" header authentication scheme used by the
    /// draft/population endpoints in the generated OpenAPI document, so
    /// consumers of the specification know how to authenticate instead of
    /// discovering it by trial and error.
    /// </summary>
    public sealed class ApiKeySecuritySchemeTransformer(IAuthenticationSchemeProvider schemeProvider) : IOpenApiDocumentTransformer
    {
        private const string SecuritySchemeId = ApiKeyAuthenticationOptions.SchemeName;

        public async Task TransformAsync(
            OpenApiDocument document,
            OpenApiDocumentTransformerContext context,
            CancellationToken cancellationToken)
        {
            var schemes = await schemeProvider.GetAllSchemesAsync();

            if (schemes.All(scheme => scheme.Name != ApiKeyAuthenticationOptions.SchemeName))
            {
                return;
            }

            var securityScheme = new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.ApiKey,
                In = ParameterLocation.Header,
                Name = ApiKeyAuthenticationOptions.HeaderName,
                Description = "API key required for protected draft and population endpoints. Provide it via the 'X-Api-Key' header."
            };

            document.AddComponent(SecuritySchemeId, securityScheme);

            var securitySchemeReference = new OpenApiSecuritySchemeReference(SecuritySchemeId, document);

            var securityRequirement = new OpenApiSecurityRequirement
            {
                [securitySchemeReference] = []
            };

            // Only the controllers guarded by [Authorize] require the key;
            // the unauthenticated health endpoints are mapped directly
            // (not via controllers) and are not part of this document.
            foreach (var pathItem in document.Paths.Values)
            {
                if (pathItem.Operations is null)
                {
                    continue;
                }

                foreach (var operation in pathItem.Operations.Values)
                {
                    operation.Security ??= new List<OpenApiSecurityRequirement>();
                    operation.Security.Add(securityRequirement);
                }
            }
        }
    }
}
