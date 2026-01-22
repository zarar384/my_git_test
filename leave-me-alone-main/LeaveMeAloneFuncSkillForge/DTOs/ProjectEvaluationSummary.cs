namespace LeaveMeAloneFuncSkillForge.DTOs
{
    public class ProjectEvaluationSummary
    {
        public double AverageEffortScore { get; set; }
        public int UrgentTaskCount { get; set; }
        public double UrgentTaskRatio { get; set; }
        public TimeSpan MinTimeRemaining { get; set; }
        public TimeSpan MaxTimeRemaining { get; set; }
        public HashSet<string> ResponsiblePersons { get; set; } = new();
        public int TotalTasks { get; set; }
    }
}
