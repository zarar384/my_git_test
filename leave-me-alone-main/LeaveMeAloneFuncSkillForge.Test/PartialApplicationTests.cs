using LeaveMeAloneFuncSkillForge.Common;

namespace LeaveMeAloneFuncSkillForge.Test
{
    public class PartialApplicationTests
    {
        [Fact]
        public void Partial_4ParamsTo1_ShouldFixFirstThreeParams()
        {
            // arrange
            Func<int, int, int, int, int> sum = (a, b, c, d) => a + b + c + d;

            var partial = sum.Partial(1, 2, 3);

            // act
            var result = partial(4);

            // assert
            Assert.Equal(10, result); // 1 + 2 + 3 + 4
        }

        [Fact]
        public void Partial_4ParamsTo2_ShouldFixFirstTwoParams()
        {
            // arrange
            Func<int, int, int, int, int> sum = (a, b, c, d) => a + b + c + d;

            var partial = sum.Partial(1, 2);

            // act
            var result = partial(3, 4);

            // assert
            Assert.Equal(10, result); // 1 + 2 + 3 + 4
        }

        [Fact]
        public void Partial_2ParamsTo1_ShouldFixFirstParam()
        {
            // arrange
            Func<int, int, int> multiply = (a, b) => a * b;

            var partial = multiply.Partial(10);

            // act
            var result = partial(5);

            // assert
            Assert.Equal(50, result);
        }

        [Fact]
        public void Partial_ChangeResultWhenSkipHeaderFlagChanges()
        {
            // Arrange
            var csv = OnePieceFunc.ParseOnePieceCharacters;

            var skipHeaderParser = csv.Partial(true, Environment.NewLine, ",");
            var noHeaderParser = csv.Partial(false, Environment.NewLine, ",");

            // Act
            var skipHeaderResult = skipHeaderParser("OnePieceCharacters.csv").ToList();
            var headerResult = noHeaderParser("OnePieceCharacters.csv").ToList();

            // Assert
            Assert.True(headerResult.Count > skipHeaderResult.Count);
        }
    }
}
