using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeaveMeAloneFuncSkillForge.Services
{
    public class TaskPipeline
    {
        /// <summary>
        /// One task -> chain of transformations -> TaskEvaluationResult
        /// </summary>
        public static TaskEvaluationResult Evaluate(TaskData task) =>
            task
                .Map(x => new
                {
                    Effort = x.EstimatedHours * x.ComplexityLevel,
                    Remaining = x.DueDate - DateTime.Now,
                    Responsible = x.IsUrgent ? x.AssignedDeveloper : x.BackupDeveloper,
                    IsUrgent = x.IsUrgent,
                    IsQuality = x.IsValid()
                })
                .Map(x => new TaskEvaluationResult
                {
                    TotalEffortScore = x.Effort,
                    TimeRemaining = x.Remaining,
                    ResponsiblePerson = x.Responsible,
                    NeedsImmediateAttention = x.IsUrgent || x.Remaining.TotalDays < 2,
                    IsQualityTask = x.IsQuality
                });

        public static TaskEvaluationResult[] EvaluateAll(IEnumerable<TaskData> tasks) =>
            tasks.Select(Evaluate).ToArray();

        public static ProjectEvaluationSummary Summarize(IEnumerable<TaskData> tasks) =>
            tasks.Map(TaskTransformations.EvaluateProjectTasks);
    }
}
