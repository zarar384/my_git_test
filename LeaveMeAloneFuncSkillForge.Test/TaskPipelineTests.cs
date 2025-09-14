using LeaveMeAloneFuncSkillForge.Domain;
using LeaveMeAloneFuncSkillForge.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeaveMeAloneFuncSkillForge.Test
{
    public class TaskPipelineTests
    {
        [Fact]
        public void EvaluateAll_ShouldReturnArray()
        {
            var tasks = new[]
            {
                new TaskData
                {
                    EstimatedHours = 5, ComplexityLevel = 2,
                    IsUrgent = false, AssignedDeveloper = "Alice", BackupDeveloper = "Bob",
                    CreatedDate = DateTime.Now.AddDays(-1), DueDate = DateTime.Now.AddDays(5)
                },
                new TaskData
                {
                    EstimatedHours = 8, ComplexityLevel = 3,
                    IsUrgent = true, AssignedDeveloper = "Charlie", BackupDeveloper = "Dana",
                    CreatedDate = DateTime.Now.AddDays(-2), DueDate = DateTime.Now.AddDays(1)
                }
            };

            var results = TaskPipeline.EvaluateAll(tasks);

            Assert.Equal(2, results.Length);
            Assert.Contains(results, r => r.ResponsiblePerson == "Charlie");
        }

        [Fact]
        public void Summarize_ShouldCalculateSummary()
        {
            var tasks = Enumerable.Range(1, 3).Select(i => new TaskData
            {
                EstimatedHours = i * 2,
                ComplexityLevel = 3,
                IsUrgent = i % 2 == 0,
                AssignedDeveloper = "Dev" + i,
                BackupDeveloper = "Backup" + i,
                CreatedDate = DateTime.Now.AddDays(-i),
                DueDate = DateTime.Now.AddDays(i + 1)
            });

            var summary = TaskPipeline.Summarize(tasks);

            Assert.Equal(3, summary.TotalTasks);
            Assert.True(summary.AverageEffortScore > 0);
            Assert.True(summary.ResponsiblePersons.Count >= 2);
        }
    }
}
