using Moq;
using MilitaryDraftSystem.Application.Common.Interfaces;
using MilitaryDraftSystem.Application.Population.Queries.GetLifecycleStatistics;
using MilitaryDraftSystem.Domain.Entities;
using MilitaryDraftSystem.Domain.Enums;

namespace MilitaryDraftSystem.Tests.Application.Population.Queries.GetLifecycleStatistics
{
    public class GetLifecycleStatisticsHandlerTests
    {
        [Fact]
        public async Task Handle_ShouldAggregateDeathAndWorkerStatistics()
        {
            var deathStatistic = DeathStatistic.Create(SubjectType.Citizen, DeathReason.OldAge);
            deathStatistic.Increment();
            deathStatistic.Increment();

            var workerStatistic = WorkerLifecycleStatistic.Create(WorkerEndReason.Resignation);
            workerStatistic.Increment();

            var dbMock = new Mock<IAppDbContext>();
            dbMock.Setup(x => x.GetDeathStatistics(It.IsAny<CancellationToken>()))
                .ReturnsAsync([deathStatistic]);
            dbMock.Setup(x => x.GetWorkerLifecycleStatistics(It.IsAny<CancellationToken>()))
                .ReturnsAsync([workerStatistic]);

            var sut = new GetLifecycleStatisticsHandler(dbMock.Object);

            var result = await sut.Handle(new GetLifecycleStatisticsQuery(), CancellationToken.None);

            var death = Assert.Single(result.DeathStatistics);
            Assert.Equal("Citizen", death.SubjectType);
            Assert.Equal("OldAge", death.Reason);
            Assert.Equal(2, death.Count);

            var worker = Assert.Single(result.WorkerLifecycleStatistics);
            Assert.Equal("Resignation", worker.Reason);
            Assert.Equal(1, worker.Count);
        }
    }
}
