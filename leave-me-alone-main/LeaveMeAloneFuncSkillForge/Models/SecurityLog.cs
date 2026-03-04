namespace LeaveMeAloneFuncSkillForge.Models
{
    public class SecurityLog
    {
        public int UserId { get; set; }

        public int Action { get; set; }

        public int ResourceId { get; set; }

        public DateTime Timestamp { get; set; }

        public string Payload { get; set; } = "";
    }
}
