using LeaveMeAloneFuncSkillForge.Domain;
using LeaveMeAloneFuncSkillForge.DTOs;
using LeaveMeAloneFuncSkillForge.Functional;

namespace LeaveMeAloneFuncSkillForge.Test
{
    public class TaskEvaluationTests
    {
        [Fact]
        public void MakeObject_ShouldCalculateCorrectProperites()
        {
            // Arrange
            var now = DateTime.Now;
            var task = new TaskData
            {
                EstimatedHours = 10,
                ComplexityLevel = 3,
                DueDate = now.AddDays(1), // less than 2 days before deadline
                IsUrgent = false,
                AssignedDeveloper = "Alice",
                BackupDeveloper = "Bob"
            };

            // Act
            var result = TaskTransformations.MakeObject(task);

            // Assert
            Assert.Equal(30, result.TotalEffortScore); // 10 * 3
            Assert.True(result.TimeRemaining.TotalMinutes <= (task.DueDate - now).TotalMinutes + 1); // with a small tolerance for timing differences
            Assert.Equal("Bob", result.ResponsiblePerson); // IsUrgent=false -> BackupDeveloper
            Assert.True(result.NeedsImmediateAttention); // term < 2 days
        }

        [Fact]
        public void EvaluateProjectTasks_ReturnsCorrectSummary()
        {
            // Arrange
            var now = DateTime.Now;
            var tasks = new List<TaskData>
            {
                new TaskData
                {
                    EstimatedHours = 10,
                    ComplexityLevel = 5,
                    IsUrgent = true,
                    AssignedDeveloper = "Dev A",
                    BackupDeveloper = "Backup A",
                    CreatedDate = now.AddDays(-5),
                    DueDate = now.AddDays(1)
                },
                new TaskData
                {
                    EstimatedHours = 20,
                    ComplexityLevel = 3,
                    IsUrgent = false,
                    AssignedDeveloper = "Dev B",
                    BackupDeveloper = "Backup B",
                    CreatedDate = now.AddDays(-10),
                    DueDate = now.AddDays(10)
                },
                new TaskData
                {
                    EstimatedHours = 5,
                    ComplexityLevel = 8,
                    IsUrgent = false,
                    AssignedDeveloper = "Dev A",
                    BackupDeveloper = "Backup A",
                    CreatedDate = now.AddDays(-2),
                    DueDate = now.AddHours(20)
                }
            };

            // Act
            var summary = TaskTransformations.EvaluateProjectTasks(tasks);

            // Assert
            Assert.Equal(3, summary.TotalTasks);

            // 10*5 + 20*3 + 5*8 = 50 + 60 + 40 = 150 total effort
            Assert.Equal(150, summary.AverageEffortScore * 3);

            // Task 1 is urgent (IsUrgent == true)
            // Task 3 has less than 2 days remaining - IsUrgent
            // Total urgent tasks = 2
            Assert.Equal(2, summary.UrgentTaskCount);

            // Task 1 responsible: "Dev A" (urgent)
            // Task 2 responsible: "Backup B" (not urgent)
            // Task 3 responsible: "Backup A" (not urgent)
            // Total unique responsible persons = 3
            Assert.Equal(3, summary.ResponsiblePersons.Count);

            // 5 sec
            var tolerance = TimeSpan.FromSeconds(5);

            // Minimum time remaining among all tasks
            var minExpected = tasks.Select(t=> TaskTransformations.MakeObject(t).TimeRemaining).Min();
            Assert.True((summary.MinTimeRemaining - minExpected).Duration() <= tolerance,
                $"Exp: {minExpected}, Act: {summary.MinTimeRemaining}");

            // Maximum time remaining among all tasks
            var maxExpected = tasks.Select(t => TaskTransformations.MakeObject(t).TimeRemaining).Max();
            Assert.True((summary.MaxTimeRemaining - maxExpected).Duration() <= tolerance,
                $"Exp: {maxExpected}, Act: {summary.MaxTimeRemaining}");

            // Urgent task ratio (urgent tasks / total tasks)
            Assert.Equal(2.0 / 3, summary.UrgentTaskRatio, 5);
        }

        [Fact]
        public void GetFirstOverdueTaskIndex_ReturnsCorrectIndex()
        {
            // Arrange
            var evaluations = new[]
            {
                new TaskEvaluationResult { TimeRemaining = TimeSpan.FromDays(5) },
                new TaskEvaluationResult { TimeRemaining = TimeSpan.FromHours(1) },
                new TaskEvaluationResult { TimeRemaining = TimeSpan.Zero },         // overdue
                new TaskEvaluationResult { TimeRemaining = TimeSpan.FromDays(-1) }, // overdue
            };

            // Act
            var result = TaskTransformations.GetFirstOverdueTaskIndex(evaluations);

            // Assert
            Assert.Equal(2, result);
        }

        [Fact]
        public void Task_IsValid_When_AllQualityRulesPass()
        {
            // Arrange 
            var task = new TaskData
            {
                EstimatedHours = 5,
                ComplexityLevel = 5,
                IsUrgent = false,
                AssignedDeveloper = "Alice",
                BackupDeveloper = "Bob",
                DueDate = DateTime.Now.AddDays(5),
                CreatedDate = DateTime.Now.AddDays(-1)
            };

            // Act
            var isValid = task.IsValid();

            // Assert
            Assert.True(isValid);
        }
    }
}
