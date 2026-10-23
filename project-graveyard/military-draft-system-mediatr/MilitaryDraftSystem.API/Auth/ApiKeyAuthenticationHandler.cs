using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace MilitaryDraftSystem.API.Auth
{
    /// <summary>
    /// Simple API key authentication: the caller must present the configured
    /// key in the "X-Api-Key" header. There is no notion of users, roles or
    /// tokens beyond this - it only proves the caller is allowed to reach the
    /// draft endpoints at all.
    /// </summary>
    public sealed class ApiKeyAuthenticationOptions : AuthenticationSchemeOptions
    {
        public const string SchemeName = "ApiKey";

        public const string HeaderName = "X-Api-Key";
    }

    public sealed class ApiKeyAuthenticationHandler : AuthenticationHandler<ApiKeyAuthenticationOptions>
    {
        private readonly IConfiguration _configuration;

        public ApiKeyAuthenticationHandler(
            IOptionsMonitor<ApiKeyAuthenticationOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            IConfiguration configuration)
            : base(options, logger, encoder)
        {
            _configuration = configuration;
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Request.Headers.TryGetValue(ApiKeyAuthenticationOptions.HeaderName, out var providedKey))
            {
                // ASP.NET Core authenticates the default scheme for every
                // request, even anonymous ones (health checks, etc.), so a
                // missing header here is routine, not a security event.
                Logger.LogDebug(
                    "Authentication skipped for {Path}: missing '{HeaderName}' header.",
                    Request.Path,
                    ApiKeyAuthenticationOptions.HeaderName);

                return Task.FromResult(AuthenticateResult.Fail(
                    $"Missing '{ApiKeyAuthenticationOptions.HeaderName}' header."));
            }

            var configuredKey = _configuration["Authentication:ApiKey"];

            if (string.IsNullOrWhiteSpace(configuredKey))
            {
                Logger.LogError(
                    "Authentication is misconfigured: no API key is configured on the server.");

                return Task.FromResult(AuthenticateResult.Fail(
                    "No API key is configured on the server."));
            }

            // Never log the provided or configured key values themselves.
            if (!string.Equals(providedKey.ToString(), configuredKey, StringComparison.Ordinal))
            {
                Logger.LogWarning(
                    "Authentication failed for {Path}: invalid API key provided.",
                    Request.Path);

                return Task.FromResult(AuthenticateResult.Fail("Invalid API key."));
            }

            var identity = new ClaimsIdentity(ApiKeyAuthenticationOptions.SchemeName);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, ApiKeyAuthenticationOptions.SchemeName);

            return Task.FromResult(AuthenticateResult.Success(ticket));
        }
    }
}
