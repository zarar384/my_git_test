namespace LeaveMeAloneFuncSkillForge.Models
{
    // Infrastructure model for API configuration settings
    public sealed class ApiConfiguration
    {
        public string BaseUrl { get; init; } = string.Empty;
        public string ApiKey { get; init; } = string.Empty;
        public int TimeoutSeconds { get; init; } = 10;
    }
}
