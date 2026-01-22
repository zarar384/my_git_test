
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
                    (source.DueDate - DateTime.Now).TotalDays < 2,
                IsQualityTask = source.IsValid()
            };

        /// <summary>
        /// Produces an overall project evaluation summary.
        /// </summary>
        public static Func<IEnumerable<TaskData>, ProjectEvaluationSummary> EvaluateProjectTasks = tasks =>
            tasks.Aggregate(
                new
                {
                    TotalEffortScore = 0.0,
                    UrgentTaskCount = 0,
                    MinTimeRemaining = TimeSpan.MaxValue,
                    MaxTimeRemaining = TimeSpan.MinValue,
                    ResponsiblePersons = new HashSet<string>(),
                    TotalTasks = 0
                },
                (acc, task) =>
                {
                    var evaluation = MakeObject(task);

                    acc.ResponsiblePersons.Add(evaluation.ResponsiblePerson);

                    return new
                    {
                        TotalEffortScore = acc.TotalEffortScore + evaluation.TotalEffortScore,
                        UrgentTaskCount = acc.UrgentTaskCount + (evaluation.NeedsImmediateAttention ? 1 : 0),
                        MinTimeRemaining = evaluation.TimeRemaining < acc.MinTimeRemaining ? evaluation.TimeRemaining : acc.MinTimeRemaining,
                        MaxTimeRemaining = evaluation.TimeRemaining > acc.MaxTimeRemaining ? evaluation.TimeRemaining : acc.MaxTimeRemaining,
                        ResponsiblePersons = acc.ResponsiblePersons,
                        TotalTasks = acc.TotalTasks + 1
                    };
                })
            is var result ? new ProjectEvaluationSummary
            {
                AverageEffortScore = result.TotalTasks == 0 ? 0 : result.TotalEffortScore / result.TotalTasks,
                UrgentTaskCount = result.UrgentTaskCount,
                UrgentTaskRatio = result.TotalTasks == 0 ? 0 : (double)result.UrgentTaskCount / result.TotalTasks,
                MinTimeRemaining = result.MinTimeRemaining,
                MaxTimeRemaining = result.MaxTimeRemaining,
                ResponsiblePersons = result.ResponsiblePersons,
                TotalTasks = result.TotalTasks
            } : null;

        /// <summary>
        /// Recursively finds the index of the first overdue task (by TimeRemaining <= 0)
        /// </summary>
        public static int? GetFirstOverdueTaskIndex(TaskEvaluationResult[] evaluations, int currentIndex = 0) =>
            currentIndex >= evaluations.Length
            ? (int?)null
            : evaluations[currentIndex].TimeRemaining <= TimeSpan.Zero
                ? currentIndex
                : GetFirstOverdueTaskIndex(evaluations, currentIndex + 1);

        /// <summary>
        /// Task validation rules
        /// </summary>
        public static readonly Func<TaskData, bool>[] QualityRules =
        {
            x => x.EstimatedHours > 0,
            x => x.ComplexityLevel >= 1 && x.ComplexityLevel <= 10,
            x => x.DueDate > x.CreatedDate,
            x => !string.IsNullOrWhiteSpace(x.AssignedDeveloper),
            x => !string.IsNullOrWhiteSpace(x.BackupDeveloper)
        };

        public static bool IsInvalid(this TaskData task) =>
            !QualityRules.Any(rule => rule(task));

        public static bool IsValid(this TaskData task) =>
            !task.IsInvalid();
    }
}
