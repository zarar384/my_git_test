namespace LeaveMeAloneFuncSkillForge.Models
{
    internal class ProcessedLog : Task<ProcessedLog>
    {
        public int UserId { get; set; }
        public string Action { get; set; }
        public int RiskScore { get; set; }
    }
}