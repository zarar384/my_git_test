using LeaveMeAloneFuncSkillForge.Common;
using LeaveMeAloneFuncSkillForge.Domain;
using LeaveMeAloneFuncSkillForge.Functional;
using LeaveMeAloneFuncSkillForge.Repositories;

namespace LeaveMeAloneFuncSkillForge.Test
{
    public class IndefiniteLoopsTests
    {
        private readonly TaskRepository _repository = new(20);

        // Imperative indefinite loop (while)
        [Fact]
        public void Imperative_WhileLoop_UntilAllUrgentTasksAreResolved()
        {
            var tasks = _repository.GetTasks().ToList();

            // classic imperative indefinite loop
            while (tasks.Any(t => t.IsUrgent))
            {
                tasks = tasks.Select(t => t.IsUrgent
                            ? t with { IsUrgent = false }
                            : t)
                    .ToList();
            }

            Assert.All(tasks, t => Assert.False(t.IsUrgent));
        }

        // Recursive indefinite loop (dangerous in C#)
        [Fact]
        public void Recursive_Function_Until_NoUrgentTasksRemain()
        {
            var tasks = _repository.GetTasks().ToArray();

            IEnumerable<TaskData> Resolve(IEnumerable<TaskData> current) =>
                !current.Any(t => t.IsUrgent)
                    ? current
                    : Resolve(current.Select(t => t.IsUrgent
                                ? t with { IsUrgent = false }
                                : t));

            var result = Resolve(tasks);

            Assert.All(result, t => Assert.False(t.IsUrgent));
        }

        // Trampolining (hidden while loop)
        [Fact]
        public void Trampolining_IterateUntil_NoUrgentTasksRemain()
        {
            var tasks = _repository.GetTasks().ToList();

            var result =
                tasks.IterateUntil(updateFunction: current =>
                    current.Select(t => t.IsUrgent
                                ? t with { IsUrgent = false }
                                : t).ToList(),

                    endCondition: current =>
                        !current.Any(t => t.IsUrgent)
                );

            Assert.All(result, t => Assert.False(t.IsUrgent));
        }

        // Aggregate as an indefinite loop abstraction
        [Fact]
        public void Aggregate_ProcessesAllTasks_WithoutExplicitLoop()
        {
            var tasks = _repository.GetTasks();

            var summary = TaskTransformations.EvaluateProjectTasks(tasks);

            Assert.True(summary.TotalTasks > 0);
            Assert.True(summary.AverageEffortScore >= 0);
            Assert.True(summary.ResponsiblePersons.Count > 0);
        }

        // Recursive search (first overdue task)
        [Fact]
        public void RecursiveSearch_FindsFirstOverdueTask()
        {
            var evaluations = _repository.GetTasks()
                    .Select(TaskTransformations.MakeObject)
                    .ToArray();

            var index =
                TaskTransformations.GetFirstOverdueTaskIndex(evaluations);

            if (index is not null)
            {
                Assert.True(evaluations[index.Value].TimeRemaining <= TimeSpan.Zero);
            }
            else
            {
                Assert.All(evaluations, e => Assert.True(e.TimeRemaining > TimeSpan.Zero));
            }
        }

        // Indefinite evaluation through LINQ (Last)
        [Fact]
        public void Linq_Last_Forces_IndefiniteIterator_ToCompletion()
        {
            var tasks = _repository.GetTasks();

            var finalSummary = tasks.Select(TaskTransformations.MakeObject).Last();

            Assert.NotNull(finalSummary);
        }

        // Functional risk evaluation (no loops)
        [Fact]
        public void RiskEvaluation_Is_Computed_Functionally()
        {
            var risks = _repository.GetTasks().Select(TaskRiskEvaluator.EvaluateRisk).ToList();

            Assert.All(risks, r => Assert.Contains(r.RiskCategory, new[] { "Low", "Medium", "High" }));
        }

        // Functional pipeline replacing control flow
        [Fact]
        public void Functional_Pipeline_Replaces_While_Logic()
        {
            var tasks = _repository.GetTasks();

            var result = tasks
                    .Where(TaskTransformations.IsValid)
                    .Select(TaskTransformations.MakeObject)
                    .Where(x => x.NeedsImmediateAttention)
                    .OrderByDescending(x => x.TotalEffortScore)
                    .ToList();

            Assert.All(result, r => Assert.True(r.NeedsImmediateAttention));
        }
    }
}
