using LeaveMeAloneFuncSkillForge.Common;
using LeaveMeAloneFuncSkillForge.Domain;
using LeaveMeAloneFuncSkillForge.Services;
using LeaveMeAloneFuncSkillForge.Utils;

namespace LeaveMeAloneFuncSkillForge.Test
{
    public class UnlessTests
    {
        [Fact]
        public void FullPipeline_ShouldSkipNullTasks_AndLogCorrectly()
        {
            // Arrange
            TaskData validTask = new TaskData
            {
                EstimatedHours = 8,
                ComplexityLevel = 5,
                IsUrgent = true,
                AssignedDeveloper = "Alice",
                BackupDeveloper = "Bob",
                CreatedDate = DateTime.Now,
                DueDate = DateTime.Now.AddDays(3)
            };

            TaskData nullTask = null; // This should be skipped

            string tapLog = "";
            Action<string> logAction = msg => tapLog += msg + "; ";

            // Act
            validTask.Unless(t => t == null, t =>
            {
                var summary = TaskPipeline.FullPipeline(t)
                    .Tap(x => logAction($"Processed valid task: {x}"));
            });

            nullTask.Unless(t => t == null, t =>
            {
                var summary = TaskPipeline.FullPipeline(t)
                    .Tap(x => logAction($"Processed null task: {x}"));
            });

            // Assert
            Assert.Contains("Processed valid task:", tapLog);
            Assert.DoesNotContain("Processed null task:", tapLog);
        }
    }
}
