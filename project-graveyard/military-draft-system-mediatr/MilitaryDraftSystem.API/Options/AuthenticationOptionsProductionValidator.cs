using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace MilitaryDraftSystem.API.Options
{
    /// <summary>
    /// Rejects the well-known development API key when running in
    /// Production. Data annotations alone can't express this because the
    /// rule depends on the hosting environment, not just the value itself.
    /// </summary>
    public sealed class AuthenticationOptionsProductionValidator(IHostEnvironment environment)
        : IValidateOptions<AuthenticationOptions>
    {
        public ValidateOptionsResult Validate(string? name, AuthenticationOptions options)
        {
            if (environment.IsProduction() &&
                string.Equals(options.ApiKey, AuthenticationOptions.DevelopmentApiKey, StringComparison.Ordinal))
            {
                return ValidateOptionsResult.Fail(
                    "Authentication:ApiKey is set to the development default value, which is not allowed in Production. " +
                    "Provide a real secret via an environment variable or another external configuration source.");
            }

            return ValidateOptionsResult.Success;
        }
    }
}
