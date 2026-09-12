using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using MilitaryDraftSystem.Application.Draft.Behaviors;

namespace MilitaryDraftSystem.Tests.Application.Draft.Behaviors
{
    public class LoggingBehaviorTests
    {
        public sealed record SampleRequest(string Name) : IRequest<string>;

        [Fact]
        public async Task Handle_ShouldInvokeNextExactlyOnce_AndReturnItsResult()
        {
            // Arrange
            var sut = new LoggingBehavior<SampleRequest, string>(Mock.Of<ILogger<LoggingBehavior<SampleRequest, string>>>());
            var callCount = 0;

            RequestHandlerDelegate<string> next = (_) =>
            {
                callCount++;
                return Task.FromResult("handled");
            };

            // Act
            var result = await sut.Handle(new SampleRequest("Alice"), next, CancellationToken.None);

            // Assert
            Assert.Equal(1, callCount);
            Assert.Equal("handled", result);
        }

        [Fact]
        public async Task Handle_ShouldPropagateException_WhenNextThrows()
        {
            // Arrange
            var sut = new LoggingBehavior<SampleRequest, string>(Mock.Of<ILogger<LoggingBehavior<SampleRequest, string>>>());

            RequestHandlerDelegate<string> next = (_) => throw new InvalidOperationException("boom");

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                sut.Handle(new SampleRequest("Alice"), next, CancellationToken.None));
        }
    }
}
