using LeaveMeAloneFuncSkillForge.Utils;

namespace LeaveMeAloneFuncSkillForge.Test
{
    public class CommonFunctionTests
    {
        [Fact]
        public void GenerateGridCoordinates_Forward_5x5_ReturnsExpectedCoordinates()
        {
            // Arrange

            //Act
            var result = FunctionExtensions.GenerateGridCoord(5, 5, reverse: false);

            //Assert
            var expectedReverseCoordinates = new List<(int X, int Y)>
            {
                (1, 1), (1, 2), (1, 3), (1, 4), (1, 5),
                (2, 1), (2, 2), (2, 3), (2, 4), (2, 5),
                (3, 1), (3, 2), (3, 3), (3, 4), (3, 5),
                (4, 1), (4, 2), (4, 3), (4, 4), (4, 5),
                (5, 1), (5, 2), (5, 3), (5, 4), (5, 5)
            };

            Assert.Equal(expectedReverseCoordinates, result);
        }
    }
}
