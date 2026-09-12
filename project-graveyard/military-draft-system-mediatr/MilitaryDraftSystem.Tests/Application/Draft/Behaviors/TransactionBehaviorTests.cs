using MediatR;
using Moq;
using MilitaryDraftSystem.Application.Common.Interfaces;
using MilitaryDraftSystem.Application.Draft.Behaviors;

namespace MilitaryDraftSystem.Tests.Application.Draft.Behaviors
{
    public class TransactionBehaviorTests
    {
        public sealed record SampleRequest(string Name) : IRequest<string>;

        [Fact]
        public async Task Handle_ShouldCommitTransaction_WhenNextSucceeds()
        {
            // Arrange
            var transactionMock = new Mock<IAppTransaction>();
            var dbMock = new Mock<IAppDbContext>();
            dbMock
                .Setup(db => db.BeginTransactionAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(transactionMock.Object);

            var sut = new TransactionBehavior<SampleRequest, string>(dbMock.Object);

            RequestHandlerDelegate<string> next = (_) => Task.FromResult("ok");

            // Act
            var result = await sut.Handle(new SampleRequest("Alice"), next, CancellationToken.None);

            // Assert
            Assert.Equal("ok", result);
            transactionMock.Verify(t => t.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
            transactionMock.Verify(t => t.RollbackAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_ShouldRollbackTransaction_AndRethrow_WhenNextThrows()
        {
            // Arrange
            var transactionMock = new Mock<IAppTransaction>();
            var dbMock = new Mock<IAppDbContext>();
            dbMock
                .Setup(db => db.BeginTransactionAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(transactionMock.Object);

            var sut = new TransactionBehavior<SampleRequest, string>(dbMock.Object);

            RequestHandlerDelegate<string> next = (_) => throw new InvalidOperationException("boom");

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                sut.Handle(new SampleRequest("Alice"), next, CancellationToken.None));

            transactionMock.Verify(t => t.RollbackAsync(It.IsAny<CancellationToken>()), Times.Once);
            transactionMock.Verify(t => t.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_ShouldBeginTransaction_BeforeInvokingNext()
        {
            // Arrange
            var transactionMock = new Mock<IAppTransaction>();
            var dbMock = new Mock<IAppDbContext>();
            dbMock
                .Setup(db => db.BeginTransactionAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(transactionMock.Object);

            var sut = new TransactionBehavior<SampleRequest, string>(dbMock.Object);
            var nextCalled = false;

            RequestHandlerDelegate<string> next = (_) =>
            {
                nextCalled = true;
                return Task.FromResult("ok");
            };

            // Act
            await sut.Handle(new SampleRequest("Alice"), next, CancellationToken.None);

            // Assert
            dbMock.Verify(db => db.BeginTransactionAsync(It.IsAny<CancellationToken>()), Times.Once);
            Assert.True(nextCalled);
        }
    }
}
