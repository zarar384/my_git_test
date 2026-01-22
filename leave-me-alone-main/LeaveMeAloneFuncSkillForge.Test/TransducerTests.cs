using LeaveMeAloneFuncSkillForge.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeaveMeAloneFuncSkillForge.Test
{
    public class TransducerTests
    {
        [Fact]
        public void Transducer_Basic()
        {
            // Arrange
            var input = Enumerable.Range(1, 10);

            // Act
            var result = input
                .Select(x => x * 2)
                .Where(x => x % 3 == 0) 
                .ToList();

            // Assert
            var expected = new List<int> { 6, 12, 18 };
            Assert.Equal(expected, result);
        }

        [Fact]
        public void Transducer_ShouldFilterTransformAndAggregate()
        {
            // Arrange
            var numbers = new[] { 4, 8, 15, 16, 23, 42 };

            Func<IEnumerable<int>, IEnumerable<int>> transformer = xs =>
                xs.Select(x => x + 5)
                    .Select(x => x * 10)
                    .Where(x => x > 100);

            Func<IEnumerable<int>, string> aggregator = xs => string.Join(", ", xs);

            // Act
            var result = numbers.Transduce(transformer, aggregator);

            // Assert
            var expected = new List<int> { 130, 200, 210, 280, 470 }.ToFormattedString();
            Assert.Equal(expected, result);
        }

        [Fact]
        public void ToTransducer_ShouldCreateReusableFunction()
        {
            // Arrange
            var numbers = new[] { 1, 2, 3, 4, 5 };
            
            Func<IEnumerable<int>, IEnumerable<int>> transformer = xs =>
                xs.Select(x => x * x);

            Func<IEnumerable<int>, int> aggregator = xs => xs.Sum();

            // reusable trasnducer function
            var transducer = transformer.ToTransducer(aggregator);

            // Act
            var result1 = transducer(numbers); // 55
            var result2 = transducer(new int[] { 10, 20 }); // 100 + 400 = 500

            // Assert
            Assert.Equal(55, result1);
            Assert.Equal(500, result2);
        }

        [Fact]
        public void Transduce_ShouldWorkWithEmptyCollection()
        {
            // Arrange
            var empty = Array.Empty<int>();

            Func<IEnumerable<int>, IEnumerable<int>> transformer = xs =>
                xs.Where(x => x > 10);

            Func<IEnumerable<int>, int> aggregator = xs => xs.Count();

            // Act
            var result = empty.Transduce(transformer, aggregator);

            // Assert
            Assert.Equal(0, result);
        }
    }
}
