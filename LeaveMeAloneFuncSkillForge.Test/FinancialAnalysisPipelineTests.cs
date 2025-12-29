using LeaveMeAloneFuncSkillForge.DiscriminatedUnions;
using LeaveMeAloneFuncSkillForge.Domain;
using LeaveMeAloneFuncSkillForge.Services;

namespace LeaveMeAloneFuncSkillForge.Test
{
    public class FinancialAnalysisPipelineTests
    {
        [Fact]
        public void Analyze_ReturnsReportWhenInputIsValid()
        {
            // Arrange
            var transactions = new List<Transaction>
            {
                new Transaction
                {
                    Id = 1,
                    Amount = 100m,
                    Time = new DateTime(2024, 1, 1, 8, 0, 0) // +10%
                },
                new Transaction
                {
                    Id = 2,
                    Amount = 200m,
                    Time = new DateTime(2024, 1, 1, 10, 0, 0)
                }
            };

            var tasks = new List<TaskData>
            {
                new TaskData { EstimatedHours = 10 },
                new TaskData { EstimatedHours = 5 }
            };

            var films = new List<Film>
            {
                new Film { BoxOfficeRevenue = 50 },
                new Film { BoxOfficeRevenue = 25 }
            };

            // Act
            var result = FinancialAnalysisPipeline.Analyze(
                transactions,
                tasks,
                films);

            // Assert
            Assert.IsType<Something<IReadOnlyList<string>>>(result.CurrentValue);

            var report = ((Something<IReadOnlyList<string>>)result.CurrentValue).Value;

            // RiskAdjustedAmount:
            // 100 * 1.1 = 110
            // 200 * 1.0 = 200
            // Total = 310
            // Minus tasks: 310 - 15 = 295
            // Plus films: 295 + 75 = 370

            Assert.Contains("Final score: 370,0", report);
            Assert.Contains("Transactions: 2", report);
            Assert.Contains("Tasks: 2", report);
            Assert.Contains("Films: 2", report);
        }

        [Fact]
        public void Analyze_ReturnsNothingWhenTransactionsAreEmpty()
        {
            // Arrange
            var transactions = Enumerable.Empty<Transaction>();
            var tasks = new List<TaskData>
            {
                new TaskData { EstimatedHours = 10 }
            };

            var films = new List<Film>
            {
                new Film { BoxOfficeRevenue = 100 }
            };

            // Act
            var result = FinancialAnalysisPipeline.Analyze(
                transactions,
                tasks,
                films);

            // Assert
            Assert.IsType<Nothing<IReadOnlyList<string>>>(result.CurrentValue);
        }
    }
}
