namespace LeaveMeAloneFuncSkillForge.Domain
{
    public class TaskData
    {
        public int EstimatedHours { get; set; }
        public int ComplexityLevel { get; set; } // 1–10
        public bool IsUrgent { get; set; }
        public string AssignedDeveloper { get; set; }
        public string BackupDeveloper { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime CreatedDate { get; set; }       
    }

}
