using LeaveMeAloneFuncSkillForge.Domain;
using LeaveMeAloneFuncSkillForge.Utils;

namespace LeaveMeAloneFuncSkillForge.Test
{
    public class ReplaceExtensionsTests
    {
        private List<TaskData> SampleTasks() =>
            new()
            {
            new TaskData
            {
                EstimatedHours = 5,
                ComplexityLevel = 2,
                IsUrgent = false,
                AssignedDeveloper = "Alice",
                BackupDeveloper = "Bob",
                CreatedDate = DateTime.Now.AddDays(-1),
                DueDate = DateTime.Now.AddDays(5)
            },
            new TaskData
            {
                EstimatedHours = 8,
                ComplexityLevel = 5,
                IsUrgent = true,
                AssignedDeveloper = "Charlie",
                BackupDeveloper = "Dave",
                CreatedDate = DateTime.Now.AddDays(-2),
                DueDate = DateTime.Now.AddDays(1)
            }
            };

        [Fact]
        public void ReplaceAt_Index_ReplacesCorrectTask()
        {
            // Arrange
            var tasks = SampleTasks();

            // Act
            var updated = tasks.ReplaceAt(1, new TaskData
            {
                EstimatedHours = 10,
                ComplexityLevel = 5,
                IsUrgent = true,
                AssignedDeveloper = "Eve",
                BackupDeveloper = "Frank",
                CreatedDate = tasks[1].CreatedDate,
                DueDate = tasks[1].DueDate
            }).ToList();

            // Assert
            Assert.Equal("Alice", updated[0].AssignedDeveloper); // unchanged
            Assert.Equal("Eve", updated[1].AssignedDeveloper);   // replaced
        }

        [Fact]
        public void ReplaceAt_WithFunc_UpdatesCorrectly()
        {
            // Arrange
            var tasks = SampleTasks();

            // Act
            var updated = tasks.ReplaceAt(0, t => new TaskData
            {
                EstimatedHours = t.EstimatedHours + 2,
                ComplexityLevel = t.ComplexityLevel,
                IsUrgent = t.IsUrgent,
                AssignedDeveloper = t.AssignedDeveloper,
                BackupDeveloper = t.BackupDeveloper,
                CreatedDate = t.CreatedDate,
                DueDate = t.DueDate
            }).ToList();

            // Assert
            Assert.Equal(7, updated[0].EstimatedHours);
            Assert.Equal(8, updated[1].EstimatedHours); // unchanged
        }

        [Fact]
        public void ReplaceWhere_UpdatesUrgentTasks()
        {
            // Arrange
            var tasks = SampleTasks();

            // Act
            var updated = tasks.ReplaceWhere(
                t => t.IsUrgent,
                t => new TaskData
                {
                    EstimatedHours = t.EstimatedHours,
                    ComplexityLevel = t.ComplexityLevel,
                    IsUrgent = t.IsUrgent,
                    AssignedDeveloper = t.AssignedDeveloper,
                    BackupDeveloper = "UpdatedBackup",
                    CreatedDate = t.CreatedDate,
                    DueDate = t.DueDate
                }).ToList();

            // Assert
            Assert.Equal("Bob", updated[0].BackupDeveloper);        // unchanged
            Assert.Equal("UpdatedBackup", updated[1].BackupDeveloper); // updated
        }
    }
}
