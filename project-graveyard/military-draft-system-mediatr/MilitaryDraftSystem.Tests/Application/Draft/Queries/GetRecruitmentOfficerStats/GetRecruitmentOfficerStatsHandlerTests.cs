using Moq;
using MilitaryDraftSystem.Application.Common.Interfaces;
using MilitaryDraftSystem.Application.Draft.Queries.GetRecruitmentOfficerStats;
using MilitaryDraftSystem.Domain.Entities;

namespace MilitaryDraftSystem.Tests.Application.Draft.Queries.GetRecruitmentOfficerStats
{
    public class GetRecruitmentOfficerStatsHandlerTests
    {
        [Fact]
        public async Task Handle_ShouldReturnStats_WhenOfficerExists()
        {
            // Arrange
            var officer = new RecruitmentOfficer(Guid.NewGuid(), "Alan Turing", "Central Recruitment Office");
            officer.RegisterDraftedCitizen();
            officer.ApplyGuilt(15);

            var dbMock = new Mock<IAppDbContext>();
            dbMock.Setup(x => x.GetRecruitmentOfficer(officer.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(officer);

            var sut = new GetRecruitmentOfficerStatsHandler(dbMock.Object);

            // Act
            var result = await sut.Handle(new GetRecruitmentOfficerStatsQuery(officer.Id), CancellationToken.None);

            // Assert
            Assert.Equal(officer.Id, result.Id);
            Assert.Equal("Alan Turing", result.FullName);
            Assert.Equal(1, result.DraftedCitizensCount);
            Assert.Equal(85, result.MoralePercent);
            Assert.Equal(1, result.GuiltIncidentsCount);
            Assert.False(result.IsRetired);
        }

        [Fact]
        public async Task Handle_ShouldThrow_WhenOfficerDoesNotExist()
        {
            // Arrange
            var officerId = Guid.NewGuid();

            var dbMock = new Mock<IAppDbContext>();
            dbMock.Setup(x => x.GetRecruitmentOfficer(officerId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((RecruitmentOfficer?)null);

            var sut = new GetRecruitmentOfficerStatsHandler(dbMock.Object);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                sut.Handle(new GetRecruitmentOfficerStatsQuery(officerId), CancellationToken.None));
        }
    }
}
