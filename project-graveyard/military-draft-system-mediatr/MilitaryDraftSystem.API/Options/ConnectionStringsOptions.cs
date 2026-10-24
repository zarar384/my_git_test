using System.ComponentModel.DataAnnotations;

namespace MilitaryDraftSystem.API.Options
{
    /// <summary>
    /// Database connectivity configuration. The application cannot run at
    /// all without a connection string, so this is validated eagerly at
    /// startup (<c>ValidateOnStart</c>) rather than discovered later inside
    /// a hosted service or the first database operation.
    /// </summary>
    public sealed class ConnectionStringsOptions
    {
        public const string SectionName = "ConnectionStrings";

        [Required(AllowEmptyStrings = false, ErrorMessage = "ConnectionStrings:DefaultConnection is missing or empty.")]
        public string DefaultConnection { get; set; } = string.Empty;
    }
}
