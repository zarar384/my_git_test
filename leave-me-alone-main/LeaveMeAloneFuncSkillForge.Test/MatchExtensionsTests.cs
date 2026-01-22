using LeaveMeAloneFuncSkillForge.Common;

namespace LeaveMeAloneFuncSkillForge.Test
{
    public class MatchExtensionsTests
    {
        [Fact]
        public void Match_ReturnsMatchedValue_WhenPredicateIsTrue()
        {
            // Arrange
            int input = 10;

            // Act
            var result = input.Match(
                (x => x == 10, x => "10"),
                (x => x == 20, x => "20")
            );

            // Assert
            Assert.Equal("10", result.Value);
            Assert.True(result.IsMatched);
        }

        [Fact]
        public void Match_ReturnsDefaultValue_WhenNoPredicateMatches()
        {
            // Arrange
            int input = 30;

            // Act
            var result = input.Match(
                (x => x == 10, x => "10"),
                (x => x == 20, x => "20")
            );

            // Assert. Fallback to default
            var fallback = result.DefaultMatch(_ => "Default");
            Assert.Equal("Default", fallback);
        }

        [Fact]
        public void Match_DefaultMatch_UsesOriginalValue()
        {
            // Arrange
            int input = 123;

            // Act
            var result = input.Match<int, string>(
                (x => false, x => "bredik") // nothing matches
            );

            // Assert
            string fallback = result.DefaultMatch(x => $"input={x}");
            Assert.Equal("input=123", fallback);
        }

        [Fact]
        public void Match_ReturnsFirstMatch_WhenMultiplePredicatesMatch()
        {
            // Arrange
            int input = 10;

            // Act
            var result = input.Match(
                (x => x >= 10, x => "1"),
                (x => x == 10, x => "2")
            );

            // Assert. Should return first match
            Assert.Equal("1", result.DefaultMatch(_ => "Default"));
        }
    }
}
