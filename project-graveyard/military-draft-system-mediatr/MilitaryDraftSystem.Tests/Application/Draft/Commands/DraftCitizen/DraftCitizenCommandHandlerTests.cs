using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using MilitaryDraftSystem.Application.Common.Interfaces;
using MilitaryDraftSystem.Application.Draft.Commands.DraftCitizen;
using MilitaryDraftSystem.Domain.Entities;
using MilitaryDraftSystem.Domain.Enums;

namespace MilitaryDraftSystem.Tests.Application.Draft.Commands.DraftCitizen
{
    public class DraftCitizenCommandHandlerTests
    {
        private static Citizen CreateEligibleCitizen()
        {
            return new Citizen(
                firstName: "Alice",
                lastName: "Williams",
                age: 22,
                medicalCategory: MedicalCategory.Fit,
                status: CitizenStatus.WaitingForDraft,
                hasCriminalRecord: false,
                isStudent: false);
        }

        [Fact]
        public async Task Handle_ShouldDraftCitizenAndIncrementOfficerCounter_WhenCitizenIsEligible()
        {
            // Arrange
            var citizen = CreateEligibleCitizen();
            var officer = new RecruitmentOfficer(Guid.NewGuid(), "Grace Hopper", "Central Recruitment Office");

            var dbMock = new Mock<IAppDbContext>();
            dbMock.Setup(x => x.GetRecruitmentOfficer(officer.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(officer);
            dbMock.Setup(x => x.GetCitizen(citizen.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(citizen);

            var narratorMock = new Mock<IWorldNarrator>();
            var loggerMock = new Mock<ILogger<DraftCitizenCommandHandler>>();

            var sut = new DraftCitizenCommandHandler(
                dbMock.Object,
                narratorMock.Object,
                loggerMock.Object);

            // Act
            await sut.Handle(new DraftCitizenCommand(citizen.Id, officer.Id), CancellationToken.None);

            // Assert
            Assert.Equal(CitizenStatus.Drafted, citizen.Status);
            Assert.Equal(1, officer.DraftedCitizensCount);
            dbMock.Verify(x => x.AddSummons(It.Is<Summons>(s =>
                s.CitizenId == citizen.Id &&
                s.RecruitmentOfficerId == officer.Id &&
                s.Source == DraftSource.RecruitmentOfficer)), Times.Once);
            dbMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldThrow_WhenRecruitmentOfficerDoesNotExist()
        {
            // Arrange
            var citizen = CreateEligibleCitizen();
            var officerId = Guid.NewGuid();

            var dbMock = new Mock<IAppDbContext>();
            dbMock.Setup(x => x.GetRecruitmentOfficer(officerId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((RecruitmentOfficer?)null);

            var sut = new DraftCitizenCommandHandler(
                dbMock.Object,
                Mock.Of<IWorldNarrator>(),
                Mock.Of<ILogger<DraftCitizenCommandHandler>>());

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                sut.Handle(new DraftCitizenCommand(citizen.Id, officerId), CancellationToken.None));
        }

        [Fact]
        public async Task Handle_ShouldThrow_WhenCitizenDoesNotExist()
        {
            // Arrange
            var officer = new RecruitmentOfficer(Guid.NewGuid(), "Grace Hopper", "Central Recruitment Office");
            var citizenId = Guid.NewGuid();

            var dbMock = new Mock<IAppDbContext>();
            dbMock.Setup(x => x.GetRecruitmentOfficer(officer.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(officer);
            dbMock.Setup(x => x.GetCitizen(citizenId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Citizen?)null);

            var sut = new DraftCitizenCommandHandler(
                dbMock.Object,
                Mock.Of<IWorldNarrator>(),
                Mock.Of<ILogger<DraftCitizenCommandHandler>>());

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                sut.Handle(new DraftCitizenCommand(citizenId, officer.Id), CancellationToken.None));
        }

        [Fact]
        public async Task Handle_ShouldThrow_WhenOfficerIsRetired()
        {
            // Arrange
            var citizen = CreateEligibleCitizen();
            var officer = new RecruitmentOfficer(Guid.NewGuid(), "Grace Hopper", "Central Recruitment Office");
            officer.ApplyGuilt(100);

            var dbMock = new Mock<IAppDbContext>();
            dbMock.Setup(x => x.GetRecruitmentOfficer(officer.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(officer);

            var sut = new DraftCitizenCommandHandler(
                dbMock.Object,
                Mock.Of<IWorldNarrator>(),
                Mock.Of<ILogger<DraftCitizenCommandHandler>>());

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                sut.Handle(new DraftCitizenCommand(citizen.Id, officer.Id), CancellationToken.None));

            dbMock.Verify(x => x.GetCitizen(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
        }
    }
}
