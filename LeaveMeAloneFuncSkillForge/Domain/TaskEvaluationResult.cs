namespace LeaveMeAloneFuncSkillForge.Domain
{
    public class TaskEvaluationResult
    {
        public int TotalEffortScore { get; set; }
        public TimeSpan TimeRemaining { get; set; }
        public string ResponsiblePerson { get; set; }
        public bool NeedsImmediateAttention { get; set; }
    }
}
