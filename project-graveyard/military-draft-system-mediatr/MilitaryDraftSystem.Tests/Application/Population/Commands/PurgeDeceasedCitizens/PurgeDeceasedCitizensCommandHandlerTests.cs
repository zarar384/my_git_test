using Microsoft.Extensions.Logging;
using Moq;
using MilitaryDraftSystem.Application.Common.Interfaces;
using MilitaryDraftSystem.Application.Population.Commands.PurgeDeceasedCitizens;
using MilitaryDraftSystem.Domain.Entities;
using MilitaryDraftSystem.Domain.Enums;

namespace MilitaryDraftSystem.Tests.Application.Population.Commands.PurgeDeceasedCitizens
{
    public class PurgeDeceasedCitizensCommandHandlerTests
    {
        private static Citizen CreateDeceasedCitizen()
        {
            var citizen = new Citizen(
                firstName: "John",
                lastName: "Doe",
                age: 90,
                medicalCategory: MedicalCategory.Fit,
                status: CitizenStatus.WaitingForDraft,
                hasCriminalRecord: false,
                isStudent: false);

            citizen.Die(DeathReason.OldAge, DateTimeOffset.UtcNow.AddDays(-60));

            return citizen;
        }

        [Fact]
        public async Task Handle_ShouldRemoveCitizenAndMarkCemeteryRecordDeleted()
        {
            // Arrange
            var citizen = CreateDeceasedCitizen();
            var cemeteryRecord = CemeteryRecord.Create(
                SubjectType.Citizen,
                citizen.Id,
                citizen.FullName,
                DeathReason.OldAge,
                DateTimeOffset.UtcNow.AddDays(-60));

            var dbMock = new Mock<IAppDbContext>();
            dbMock.Setup(x => x.GetDeceasedCitizensOlderThan(It.IsAny<DateTimeOffset>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync([citizen]);
            dbMock.Setup(x => x.GetCemeteryRecord(SubjectType.Citizen, citizen.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(cemeteryRecord);

            var sut = new PurgeDeceasedCitizensCommandHandler(
                dbMock.Object,
                Mock.Of<ILogger<PurgeDeceasedCitizensCommandHandler>>());

            // Act
            await sut.Handle(new PurgeDeceasedCitizensCommand(), CancellationToken.None);

            // Assert
            dbMock.Verify(x => x.RemoveCitizen(citizen), Times.Once);
            Assert.True(cemeteryRecord.OriginalRecordDeleted);
            dbMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldNotSave_WhenNoCitizensToPurge()
        {
            // Arrange
            var dbMock = new Mock<IAppDbContext>();
            dbMock.Setup(x => x.GetDeceasedCitizensOlderThan(It.IsAny<DateTimeOffset>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync([]);

            var sut = new PurgeDeceasedCitizensCommandHandler(
                dbMock.Object,
                Mock.Of<ILogger<PurgeDeceasedCitizensCommandHandler>>());

            // Act
            await sut.Handle(new PurgeDeceasedCitizensCommand(), CancellationToken.None);

            // Assert
            dbMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }
    }
}
