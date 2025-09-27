using LeaveMeAloneFuncSkillForge.Common;

namespace LeaveMeAloneFuncSkillForge.Functional
{
    public static class TaskFuncs
    {
        public static string GetResponsible(this TaskData task) =>
            task.Alt(
                t => !string.IsNullOrWhiteSpace(t.AssignedDeveloper) ? t.AssignedDeveloper : null,
                t => !string.IsNullOrWhiteSpace(t.BackupDeveloper) ? t.BackupDeveloper : null,
                _ => "Unassigned"
            );

        public static Func<TaskData, int> CalcEffort = task =>
            task.EstimatedHours * task.ComplexityLevel;

        public static Func<TaskData, TaskRiskResult> CalcRisk = task =>
        {
            var effort = CalcEffort(task);
            var days = (task.DueDate - DateTime.Now).TotalDays;
            var risk = effort / Math.Max(days, 1);

            return new TaskRiskResult
            {
                RiskScore = risk,
                RiskCategory = risk > 100 ? "High" :
                               risk > 50 ? "Medium" : "Low"
            };
        };

        public static Func<TaskData, bool> CheckQuality = 
            task => task.IsValid();

        public static Func<(int Effort, TaskRiskResult Risk, bool Quality), string> FormatSummary =
             x => $"Effort={x.Effort}, Risk={x.Risk.RiskCategory} ({x.Risk.RiskScore:F1}), Quality={x.Quality}";

        public static Func<TaskData, (int Effort, TaskRiskResult Risk, bool Quality)> Aggregate =
            task => (CalcEffort(task), CalcRisk(task), CheckQuality(task));

        public static Func<TaskData, string> FullSummary =
            Aggregate.Compose(FormatSummary);
    }
}
