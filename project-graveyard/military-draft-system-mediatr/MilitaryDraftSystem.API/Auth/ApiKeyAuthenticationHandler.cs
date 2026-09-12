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
                return Task.FromResult(AuthenticateResult.Fail(
                    $"Missing '{ApiKeyAuthenticationOptions.HeaderName}' header."));
            }

            var configuredKey = _configuration["Authentication:ApiKey"];

            if (string.IsNullOrWhiteSpace(configuredKey))
            {
                return Task.FromResult(AuthenticateResult.Fail(
                    "No API key is configured on the server."));
            }

            if (!string.Equals(providedKey.ToString(), configuredKey, StringComparison.Ordinal))
            {
                return Task.FromResult(AuthenticateResult.Fail("Invalid API key."));
            }

            var identity = new ClaimsIdentity(ApiKeyAuthenticationOptions.SchemeName);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, ApiKeyAuthenticationOptions.SchemeName);

            return Task.FromResult(AuthenticateResult.Success(ticket));
        }
    }
}
