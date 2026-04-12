using LeaveMeAloneCSharp.Playground;

namespace LeaveMeAloneCSharp.Test
{
    public class DataFlowTests
    {

        [Fact]
        public async Task ErrorPropagationDemo_ShouldThrowInvalidOperationException()
        {
            // Act
            var ex = await Assert.ThrowsAsync<AggregateException>(async () =>
            {
                await DataflowL.ErrorPropagationDemo(false);
            });

            // Assert
            // flatten() will return an AggregateException that contains all the exceptions thrown by the tasks in the dataflow blocks.
            var innerException = ex.Flatten().InnerException;

            Assert.IsType<InvalidOperationException>(innerException);
        }
    }
}
