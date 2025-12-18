using LeaveMeAloneFuncSkillForge.Common;

namespace LeaveMeAloneFuncSkillForge.Test
{
    public class CurryingTests
    {
        [Fact]
        public void Add_WithTwoSteps_ReturnsCorrectSum()
        {
            // Arrange
            var add100 = Extensions.Add(100);

            // Act
            var result = add100(200);

            // Assert
            Assert.Equal(300m, result);
        }

        [Fact]
        public void Add_CanBeReused_WithDifferentValues()
        {
            // Arrange
            var add50 = Extensions.Add(50);

            // Act
            var result1 = add50(10);
            var result2 = add50(0);
            var result3 = add50(100);

            // Assert
            Assert.Equal(60m, result1);
            Assert.Equal(50m, result2);
            Assert.Equal(150m, result3);
        }

        [Fact]
        public void Add_ReturnsFunction_WhenOnlyFirstArgumentProvided()
        {
            // Act
            var result = Extensions.Add(10);

            // Assert
            Assert.IsType<Func<decimal, decimal>>(result);
        }
    }
}
