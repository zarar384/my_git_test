using LeaveMeAloneFuncSkillForge.Common;
using LeaveMeAloneFuncSkillForge.Domain;

namespace LeaveMeAloneFuncSkillForge.Test
{
    public class AdjacentFunctionsTests
    {
        private List<Transaction> GetSampleTransactions() => new List<Transaction>
        {
            new Transaction { Id = 1, Time = DateTime.Today.AddHours(9), Amount = 100m },
            new Transaction { Id = 2, Time = DateTime.Today.AddHours(10), Amount = 150m },
            new Transaction { Id = 3, Time = DateTime.Today.AddHours(11), Amount = 200m },
            new Transaction { Id = 4, Time = DateTime.Today.AddHours(12), Amount = 400m },
            new Transaction { Id = 5, Time = DateTime.Today.AddHours(13), Amount = 450m }
        };

        [Fact]
        public void AllAdjacent_ShouldReturnTrue_ForNonDecreasingAmounts()
        {
            // Arrange
            var transactions = GetSampleTransactions();

            // Act
            bool result = transactions.AllAdjacent((prev, next) => next.Amount >= prev.Amount);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void AnyAdjacent_ShouldReturnTrue_ForSuddenIncrease()
        {
            // Arrange
            var transactions = GetSampleTransactions();

            // Act
            bool result = transactions.AnyAdjacent((prev, next) => next.Amount > prev.Amount * 1.5m);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void AllAdjacent_ShouldReturnFalse_WhenDecreaseExists()
        {
            // Arrange
            var transactions = GetSampleTransactions();
            transactions[2].Amount = 100m; // insert a decrease

            // Act
            bool result = transactions.AllAdjacent((prev, next) => next.Amount >= prev.Amount);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void AnyAdjacent_ShouldReturnFalse_WhenNoSuddenIncrease()
        {
            // Arrange
            var transactions = GetSampleTransactions();
            // remove sharp rise 1.5×
            transactions[1].Amount = 120m;
            transactions[2].Amount = 140m;
            transactions[3].Amount = 160m;
            transactions[4].Amount = 180m; 

            // Act
            bool result = transactions.AnyAdjacent((prev, next) => next.Amount > prev.Amount * 1.5m);

            // Assert
            Assert.False(result);
        }
    }
}
