namespace LeaveMeAloneFuncSkillForge.Services.Reports.Environment
{
    public sealed class ReportEnvironment
    {
        public IEnumerable<Film> Films { get; init; }
        public string Currency { get; init; }
        public int TopGenres { get; init; }
    }
}
