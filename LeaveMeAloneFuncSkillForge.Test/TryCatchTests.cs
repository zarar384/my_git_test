using LeaveMeAloneFuncSkillForge.Common;
using LeaveMeAloneFuncSkillForge.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeaveMeAloneFuncSkillForge.Test
{
    public class TryCatchTests
    {
        [Fact]
        public void MapWithTryCatch_ShouldReturnResult_WhenNoException()
        {
            // Arrange
            int input = 5;

            // Act
            var result = input.MapWithTryCatch(i => i * 2);

            // Assert
            Assert.Null(result.Error);
            Assert.Equal(10, result.Result);
        }

        [Fact]
        public void MapWithTryCatch_ShouldReturnError_WhenExceptionThrown()
        {
            // Arrange
            int input = 5;

            // Act
            var result = input.MapWithTryCatch<int, int>(i => throw new InvalidOperationException("Test exception"));

            // Assert
            Assert.NotNull(result.Error);
            Assert.IsType<InvalidOperationException>(result.Error);
            Assert.Equal("Test exception", result.Error.Message);
        }

        [Fact]
        public void OnError_ShouldExecuteAction_WhenErrorExists()
        {
            // Arrange
            var execResult = new ExecutionResult<int>
            {
                Error = new InvalidOperationException("Test error")
            };
            var actionExecuted = false;

            // Act
            var result = execResult.OnError(ex => actionExecuted = true);

            // Assert
            Assert.True(actionExecuted);
            Assert.Equal(0, result); // Default value since error occurred
        }

        [Fact]
        public void Pipeline_Compose_Transduce_Tap_MapWithTryCatch_WorksAsExpected()
        {
            // Arrange
            var numbers = new[] { 1, 2, 3, 4, 5 };

            // +1 => *10 => filter divisible by 3 => risky divide (10 / (x-30))
            Func<IEnumerable<int>, IEnumerable<int>> transformer = nums => nums
                .Select(x => x + 1)
                .Select(x => x * 10)
                .Where(x => x % 3 == 0)
                .Select(x => 10 / (x - 30)); // will throw when x=30 (x=2)

            Func<IEnumerable<int>, string> aggregator = nums => string.Join(",", nums);

            var transduced = transformer.ToTransducer(aggregator);

            var pipeline = new Func<int[], int[]>(nums=> nums)
                .Compose(nums => nums.Tap(n => Console.WriteLine($"Input: {string.Join(", ", n)}")))
                .Compose(nums => nums.MapWithTryCatch(transduced).OnError(e =>
                    Console.WriteLine("Caught error: " + e.Message)))
                .Compose(result => result.Tap(n => Console.WriteLine($"Output: {n}")));

            var consoleOutput = new StringWriter();
            Console.SetOut(consoleOutput);

            // Act
            var result = pipeline(numbers);

            // Assert
            Assert.Null(result); // one element will give division by zero (null (default string))

            var output = consoleOutput.ToString();
            Assert.Contains("Input: 1, 2, 3, 4, 5", output);
            Assert.Contains("Caught error: Attempted to divide by zero.", output);
            Assert.Contains("Output: ", output);
        }
    }
}
