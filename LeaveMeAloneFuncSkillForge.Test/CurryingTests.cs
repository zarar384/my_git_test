using LeaveMeAloneFuncSkillForge.Common;
using LeaveMeAloneFuncSkillForge.Domain;
using LeaveMeAloneFuncSkillForge.DTOs;

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

        [Fact]
        public void Currying_Builds_Specific_Parser_Step_By_Step()
        {
            // Arrange
            var curried = OnePieceFunc.ParseOnePieceCharacters.Curry();

            var skipHeader = curried(true);
            var windowsLines = skipHeader(Environment.NewLine);
            var commaSeparated = windowsLines(",");

            // Act
            var result = commaSeparated("OnePieceCharacters.csv").ToList();

            // Assert
            Assert.NotEmpty(result);
            Assert.False(string.IsNullOrWhiteSpace(result[0].Name));
        }

        [Fact]
        public void Curried_Parser_Can_Be_Reused_For_Multiple_Files()
        {
            // Arrange
            var parser = OnePieceFunc.ParseOnePieceCharacters
                    .Curry()(true)(Environment.NewLine)(",");

            // Act
            var file1 = parser("OnePieceCharacters.csv").ToList();
            var file2 = parser("OnePieceCharacters.csv").ToList();

            // Assert
            Assert.Equal(file1.Count, file2.Count);
        }

        [Fact]
        public void Currying_Allows_Changing_Only_One_Aspect_Of_Parsing()
        {
            // Arrange
            var curried = OnePieceFunc.ParseOnePieceCharacters.Curry();

            var skipHeaderParser =
                curried(true)(Environment.NewLine)(",");

            var noHeaderParser =
                curried(false)(Environment.NewLine)(",");

            // Act
            var skipHeaderResult = skipHeaderParser("OnePieceCharacters.csv").ToList();
            var noHeaderResult = noHeaderParser("OnePieceCharacters.csv").ToList();

            // Assert
            Assert.True(noHeaderResult.Count > skipHeaderResult.Count);
        }

        [Fact]
        public void Curried_Function_Remains_Function_Until_Last_Argument()
        {
            // Arrange
            var curried = OnePieceFunc.ParseOnePieceCharacters.Curry();

            // Act
            var step1 = curried(true);
            var step2 = step1(Environment.NewLine);
            var step3 = step2(",");

            // Assert
            Assert.IsType<Func<string, IEnumerable<OnePieceCharacterDto>>>(step3);
        }
    }
}
