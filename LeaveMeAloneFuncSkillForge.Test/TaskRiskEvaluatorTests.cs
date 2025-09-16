using LeaveMeAloneFuncSkillForge.Domain;
using LeaveMeAloneFuncSkillForge.Functional;

namespace LeaveMeAloneFuncSkillForge.Test
{
    public class TaskRiskEvaluatorTests
    {
        [Fact]
        public void EvaluateRisk_ShouldReturnHighRisk_WhenEffortIsHugeAndDeadlineIsClose()
        {
            // Arrange
            var task = new TaskData
            {
                EstimatedHours = 100,
                ComplexityLevel = 10,
                DueDate = DateTime.Now.AddDays(1),
                CreatedDate = DateTime.Now
            };

            // Act
            var result = TaskRiskEvaluator.EvaluateRisk(task);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.RiskScore > 100);
            Assert.Equal("High", result.RiskCategory);
        }

        [Fact]
        public void EvaluateRisk_ShouldReturnMediumRisk_WhenEffortModerateAndDeadlineTight()
        {
            // Arrange
            var task = new TaskData
            {
                EstimatedHours = 20,
                ComplexityLevel = 5,
                DueDate = DateTime.Now.AddDays(2),
                CreatedDate = DateTime.Now
            };

            // Act
            var result = TaskRiskEvaluator.EvaluateRisk(task);

            // Assert
            Assert.NotNull(result);
            Assert.InRange(result.RiskScore, 50, 100);
            Assert.Equal("Medium", result.RiskCategory);
        }

        [Fact]
        public void EvaluateRisk_ShouldReturnLowRisk_WhenEffortLowAndDeadlineFar()
        {
            // Arrange
            var task = new TaskData
            {
                EstimatedHours = 5,
                ComplexityLevel = 2,
                DueDate = DateTime.Now.AddDays(30),
                CreatedDate = DateTime.Now
            };

            // Act
            var result = TaskRiskEvaluator.EvaluateRisk(task);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.RiskScore < 50);
            Assert.Equal("Low", result.RiskCategory);
        }
    }
}