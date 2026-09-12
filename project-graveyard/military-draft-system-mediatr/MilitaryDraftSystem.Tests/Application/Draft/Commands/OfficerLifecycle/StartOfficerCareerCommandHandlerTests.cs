using Microsoft.Extensions.Logging;
using Moq;
using MilitaryDraftSystem.Application.Common.Interfaces;
using MilitaryDraftSystem.Application.Draft.Commands.OfficerLifecycle;
using MilitaryDraftSystem.Domain.Entities;
using MilitaryDraftSystem.Domain.Enums;

namespace MilitaryDraftSystem.Tests.Application.Draft.Commands.OfficerLifecycle
{
    public class StartOfficerCareerCommandHandlerTests
    {
        private static Player CreatePlayer()
        {
            return new Player(Guid.NewGuid(), "Player One", DateTimeOffset.UtcNow);
        }

        [Fact]
        public async Task Handle_ShouldCreateOfficer_WhenPlayerHasNoActiveOfficer()
        {
            // Arrange
            var player = CreatePlayer();

            var dbMock = new Mock<IAppDbContext>();
            dbMock.Setup(x => x.GetPlayer(player.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(player);
            dbMock.Setup(x => x.GetActiveRecruitmentOfficerByPlayer(player.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync((RecruitmentOfficer?)null);

            var sut = new StartOfficerCareerCommandHandler(
                dbMock.Object,
                Mock.Of<ILogger<StartOfficerCareerCommandHandler>>());

            // Act
            var officerId = await sut.Handle(
                new StartOfficerCareerCommand(player.Id, "Alan Turing", "Central Recruitment Office"),
                CancellationToken.None);

            // Assert
            Assert.NotEqual(Guid.Empty, officerId);
            dbMock.Verify(x => x.AddRecruitmentOfficer(It.Is<RecruitmentOfficer>(o =>
                o.Id == officerId &&
                o.PlayerId == player.Id &&
                o.FullName == "Alan Turing" &&
                o.IsActive)), Times.Once);
            dbMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldThrow_WhenPlayerDoesNotExist()
        {
            // Arrange
            var playerId = Guid.NewGuid();

            var dbMock = new Mock<IAppDbContext>();
            dbMock.Setup(x => x.GetPlayer(playerId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Player?)null);

            var sut = new StartOfficerCareerCommandHandler(
                dbMock.Object,
                Mock.Of<ILogger<StartOfficerCareerCommandHandler>>());

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                sut.Handle(
                    new StartOfficerCareerCommand(playerId, "Alan Turing", "Central Recruitment Office"),
                    CancellationToken.None));
        }

        [Fact]
        public async Task Handle_ShouldThrow_WhenPlayerAlreadyHasActiveOfficer()
        {
            // Arrange
            var player = CreatePlayer();
            var existingOfficer = new RecruitmentOfficer(Guid.NewGuid(), "Grace Hopper", "Central Recruitment Office", player.Id);

            var dbMock = new Mock<IAppDbContext>();
            dbMock.Setup(x => x.GetPlayer(player.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(player);
            dbMock.Setup(x => x.GetActiveRecruitmentOfficerByPlayer(player.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingOfficer);

            var sut = new StartOfficerCareerCommandHandler(
                dbMock.Object,
                Mock.Of<ILogger<StartOfficerCareerCommandHandler>>());

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                sut.Handle(
                    new StartOfficerCareerCommand(player.Id, "Alan Turing", "Central Recruitment Office"),
                    CancellationToken.None));

            dbMock.Verify(x => x.AddRecruitmentOfficer(It.IsAny<RecruitmentOfficer>()), Times.Never);
            dbMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Theory]
        [InlineData(true)] // Resign
        [InlineData(false)] // Die
        public async Task Handle_ShouldAllowNewOfficer_WhenPreviousOfficerCareerHasEnded(bool resign)
        {
            // Arrange
            var player = CreatePlayer();
            var previousOfficer = new RecruitmentOfficer(Guid.NewGuid(), "Grace Hopper", "Central Recruitment Office", player.Id);

            if (resign)
                previousOfficer.Resign(DateTimeOffset.UtcNow);
            else
                previousOfficer.Die(DeathReason.HeartAttack, DateTimeOffset.UtcNow);

            var dbMock = new Mock<IAppDbContext>();
            dbMock.Setup(x => x.GetPlayer(player.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(player);
            // The previous officer's career has ended, so no active officer is returned.
            dbMock.Setup(x => x.GetActiveRecruitmentOfficerByPlayer(player.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync((RecruitmentOfficer?)null);

            var sut = new StartOfficerCareerCommandHandler(
                dbMock.Object,
                Mock.Of<ILogger<StartOfficerCareerCommandHandler>>());

            // Act
            var officerId = await sut.Handle(
                new StartOfficerCareerCommand(player.Id, "Alan Turing", "Central Recruitment Office"),
                CancellationToken.None);

            // Assert
            Assert.NotEqual(Guid.Empty, officerId);
            Assert.NotEqual(previousOfficer.Id, officerId);
            dbMock.Verify(x => x.AddRecruitmentOfficer(It.IsAny<RecruitmentOfficer>()), Times.Once);
        }
    }
}
