using LeaveMeAloneFuncSkillForge.Domain;

namespace LeaveMeAloneFuncSkillForge.Functional
{
    public static class TaskTransformations
    {
        /// <summary>
        /// Calculates total effort score, time remaining, assigns responsible person,
        /// and determines if the task needs immediate attention.
        /// </summary>
        public static TaskEvaluationResult MakeObject(TaskData source) =>
            new TaskEvaluationResult
            {
                TotalEffortScore = source.EstimatedHours * source.ComplexityLevel,
                TimeRemaining = source.DueDate - DateTime.Now,
                ResponsiblePerson = source.IsUrgent
                    ? source.AssignedDeveloper
                    : source.BackupDeveloper,
                NeedsImmediateAttention = source.IsUrgent ||
                    (source.DueDate - DateTime.Now).TotalDays < 2
            };
    }
}
