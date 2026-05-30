using System.Collections.ObjectModel;

namespace LeaveMeAloneCSharp.Models
{
    public struct LogEntry 
    {
        public int UserId { get; set; }
        public string Action { get; set; }
        public DateTime Timestamp { get; set; }
    }
}