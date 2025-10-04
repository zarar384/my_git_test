using LeaveMeAloneFuncSkillForge.Domain.DiscriminatedUnions;
using LeaveMeAloneFuncSkillForge.Functional;

namespace LeaveMeAloneFuncSkillForge.Test
{
    public class ShapeTests
    {
        [Fact]
        public void Area_ShouldCalculateCorrectly()
        {
            // Arrange
            var circle = new Shape.Circle(5);
            var square = new Shape.Square(4);
            var rectangle = new Shape.Rectangle(3, 6);
            var triangle = new Shape.Triangle(4, 5);

            // Act
            var circleArea = circle.Area();
            var squareArea = square.Area();
            var rectangleArea = rectangle.Area();
            var triangleArea = triangle.Area();

            // Assert
            Assert.Equal(Math.PI * 25, circleArea, 5);  // Area of circle: pir^2
            Assert.Equal(16, squareArea);               // Area of square: side^2
            Assert.Equal(18, rectangleArea);            // Area of rectangle: width * height
            Assert.Equal(10, triangleArea);             // Area of triangle: 0.5 * base * height
        }
    }
}
