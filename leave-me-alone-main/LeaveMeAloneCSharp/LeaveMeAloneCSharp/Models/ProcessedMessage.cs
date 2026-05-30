namespace LeaveMeAloneCSharp.Models
{
    public class ProcessedMessage
    {
        public int UserId { get; set; }
        public string Content { get; set; } = "";
        public int RiskScore { get; set; }
    }
}