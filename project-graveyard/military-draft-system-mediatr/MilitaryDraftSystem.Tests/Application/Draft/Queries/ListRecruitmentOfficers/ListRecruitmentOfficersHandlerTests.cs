using Moq;
using MilitaryDraftSystem.Application.Common.Interfaces;
using MilitaryDraftSystem.Application.Draft.Queries.ListRecruitmentOfficers;
using MilitaryDraftSystem.Domain.Entities;

namespace MilitaryDraftSystem.Tests.Application.Draft.Queries.ListRecruitmentOfficers
{
    public class ListRecruitmentOfficersHandlerTests
    {
        [Fact]
        public async Task Handle_ShouldReturnSummaries_ForAllOfficers()
        {
            var playerId = Guid.NewGuid();
            var activeOfficer = new RecruitmentOfficer(Guid.NewGuid(), "Alan Turing", "Central Recruitment Office", playerId);
            var resignedOfficer = new RecruitmentOfficer(Guid.NewGuid(), "Grace Hopper", "Central Recruitment Office");
            resignedOfficer.Resign(DateTimeOffset.UtcNow);

            var dbMock = new Mock<IAppDbContext>();
            dbMock.Setup(x => x.GetRecruitmentOfficers(It.IsAny<CancellationToken>()))
                .ReturnsAsync([activeOfficer, resignedOfficer]);

            var sut = new ListRecruitmentOfficersHandler(dbMock.Object);

            var result = await sut.Handle(new ListRecruitmentOfficersQuery(), CancellationToken.None);

            Assert.Equal(2, result.Count);
            var activeSummary = Assert.Single(result, r => r.Id == activeOfficer.Id);
            Assert.Equal(playerId, activeSummary.PlayerId);
            Assert.True(activeSummary.IsActive);
            Assert.False(activeSummary.HasEndedCareer);

            var resignedSummary = Assert.Single(result, r => r.Id == resignedOfficer.Id);
            Assert.False(resignedSummary.IsActive);
            Assert.True(resignedSummary.HasEndedCareer);
        }
    }
}
