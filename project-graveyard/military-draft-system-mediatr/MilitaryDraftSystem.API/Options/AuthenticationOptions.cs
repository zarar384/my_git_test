using System.ComponentModel.DataAnnotations;

namespace MilitaryDraftSystem.API.Options
{
    /// <summary>
    /// API key authentication configuration. A missing or empty key would
    /// leave every draft endpoint either inaccessible or (worse) silently
    /// unauthenticated, so this is validated at startup rather than the
    /// first time a request is authenticated.
    /// </summary>
    public sealed class AuthenticationOptions
    {
        public const string SectionName = "Authentication";

        /// <summary>
        /// Well-known development-only value. Production must never fall
        /// back to this default, whether because it was left in
        /// appsettings.json or simply never overridden.
        /// </summary>
        public const string DevelopmentApiKey = "dev-local-api-key";

        [Required(AllowEmptyStrings = false, ErrorMessage = "Authentication:ApiKey is missing or empty.")]
        public string ApiKey { get; set; } = string.Empty;
    }
}
