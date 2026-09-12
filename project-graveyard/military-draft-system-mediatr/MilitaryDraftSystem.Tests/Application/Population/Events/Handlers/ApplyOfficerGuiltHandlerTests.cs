using Microsoft.Extensions.Logging;
using Moq;
using MilitaryDraftSystem.Application.Common.Interfaces;
using MilitaryDraftSystem.Application.Population.Events;
using MilitaryDraftSystem.Application.Population.Events.Handlers;
using MilitaryDraftSystem.Domain.Entities;
using MilitaryDraftSystem.Domain.Enums;
using MilitaryDraftSystem.Domain.ValueObjects;

namespace MilitaryDraftSystem.Tests.Application.Population.Events.Handlers
{
    public class ApplyOfficerGuiltHandlerTests
    {
        [Fact]
        public async Task Handle_ShouldApplyGuilt_WhenCitizenDrafterByOfficerDiesInMilitaryService()
        {
            // Arrange
            var citizenId = Guid.NewGuid();
            var officer = new RecruitmentOfficer(Guid.NewGuid(), "Alan Turing", "Central Recruitment Office");
            var summons = Summons.Create(citizenId, DraftSource.RecruitmentOfficer, officer.Id, null, DateTimeOffset.UtcNow);
            var death = Death.Create(DeathReason.KilledInCombat, DateTimeOffset.UtcNow);

            var dbMock = new Mock<IAppDbContext>();
            dbMock.Setup(x => x.GetSummonsByCitizen(citizenId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(summons);
            dbMock.Setup(x => x.GetRecruitmentOfficer(officer.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(officer);

            var sut = new ApplyOfficerGuiltHandler(dbMock.Object, Mock.Of<ILogger<ApplyOfficerGuiltHandler>>());

            // Act
            await sut.Handle(new CitizenDiedEvent(citizenId, death), CancellationToken.None);

            // Assert
            Assert.Equal(85, officer.MoralePercent);
            Assert.Equal(1, officer.GuiltIncidentsCount);
            dbMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Theory]
        [InlineData(DeathReason.OldAge)]
        [InlineData(DeathReason.HeartAttack)]
        [InlineData(DeathReason.TrafficAccident)]
        public async Task Handle_ShouldDoNothing_WhenDeathReasonIsNotMilitary(DeathReason reason)
        {
            // Arrange
            var citizenId = Guid.NewGuid();
            var death = Death.Create(reason, DateTimeOffset.UtcNow);

            var dbMock = new Mock<IAppDbContext>();

            var sut = new ApplyOfficerGuiltHandler(dbMock.Object, Mock.Of<ILogger<ApplyOfficerGuiltHandler>>());

            // Act
            await sut.Handle(new CitizenDiedEvent(citizenId, death), CancellationToken.None);

            // Assert
            dbMock.Verify(x => x.GetSummonsByCitizen(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
            dbMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_ShouldDoNothing_WhenCitizenWasNotDraftedByAnOfficer()
        {
            // Arrange
            var citizenId = Guid.NewGuid();
            var agentId = Guid.NewGuid();
            var summons = Summons.Create(citizenId, DraftSource.AutomaticAgent, null, agentId, DateTimeOffset.UtcNow);
            var death = Death.Create(DeathReason.KilledInCombat, DateTimeOffset.UtcNow);

            var dbMock = new Mock<IAppDbContext>();
            dbMock.Setup(x => x.GetSummonsByCitizen(citizenId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(summons);

            var sut = new ApplyOfficerGuiltHandler(dbMock.Object, Mock.Of<ILogger<ApplyOfficerGuiltHandler>>());

            // Act
            await sut.Handle(new CitizenDiedEvent(citizenId, death), CancellationToken.None);

            // Assert
            dbMock.Verify(x => x.GetRecruitmentOfficer(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
            dbMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_ShouldDoNothing_WhenNoSummonsExistsForCitizen()
        {
            // Arrange
            var citizenId = Guid.NewGuid();
            var death = Death.Create(DeathReason.FriendlyFireDuringTraining, DateTimeOffset.UtcNow);

            var dbMock = new Mock<IAppDbContext>();
            dbMock.Setup(x => x.GetSummonsByCitizen(citizenId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Summons?)null);

            var sut = new ApplyOfficerGuiltHandler(dbMock.Object, Mock.Of<ILogger<ApplyOfficerGuiltHandler>>());

            // Act
            await sut.Handle(new CitizenDiedEvent(citizenId, death), CancellationToken.None);

            // Assert
            dbMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }
    }
}
