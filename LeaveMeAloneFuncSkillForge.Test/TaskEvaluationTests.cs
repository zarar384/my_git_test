using LeaveMeAloneFuncSkillForge.Domain;
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
    }
}
