using Microsoft.Extensions.Logging;
using Moq;
using MilitaryDraftSystem.Application.Common.Interfaces;
using MilitaryDraftSystem.Application.Draft.Commands.RunOfficerLifeSimulation;
using MilitaryDraftSystem.Domain.Entities;
using MilitaryDraftSystem.Domain.Enums;
using MilitaryDraftSystem.Tests.Common;

namespace MilitaryDraftSystem.Tests.Application.Draft.Commands.RunOfficerLifeSimulation
{
    public class RunOfficerLifeSimulationCommandHandlerTests
    {
        [Fact]
        public async Task Handle_ShouldResignOfficer_WhenResignationChanceSucceeds()
        {
            // Arrange
            var officer = new RecruitmentOfficer(Guid.NewGuid(), "Alan Turing", "Central Recruitment Office");

            var dbMock = new Mock<IAppDbContext>();
            dbMock.Setup(x => x.GetActiveRecruitmentOfficers(It.IsAny<CancellationToken>()))
                .ReturnsAsync([officer]);

            var random = new FakeRandomProvider();
            random.EnqueueDouble(0.0); // resignation succeeds (first check)

            var sut = new RunOfficerLifeSimulationCommandHandler(
                dbMock.Object,
                random,
                Mock.Of<ILogger<RunOfficerLifeSimulationCommandHandler>>());

            // Act
            await sut.Handle(new RunOfficerLifeSimulationCommand(), CancellationToken.None);

            // Assert
            Assert.Equal(OfficerStatus.Resigned, officer.Status);
            dbMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldNotSave_WhenNoActiveOfficers()
        {
            // Arrange
            var dbMock = new Mock<IAppDbContext>();
            dbMock.Setup(x => x.GetActiveRecruitmentOfficers(It.IsAny<CancellationToken>()))
                .ReturnsAsync([]);

            var sut = new RunOfficerLifeSimulationCommandHandler(
                dbMock.Object,
                new FakeRandomProvider(),
                Mock.Of<ILogger<RunOfficerLifeSimulationCommandHandler>>());

            // Act
            await sut.Handle(new RunOfficerLifeSimulationCommand(), CancellationToken.None);

            // Assert
            dbMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_ShouldLeaveOfficerActive_WhenAllChancesFail()
        {
            // Arrange
            var officer = new RecruitmentOfficer(Guid.NewGuid(), "Alan Turing", "Central Recruitment Office");

            var dbMock = new Mock<IAppDbContext>();
            dbMock.Setup(x => x.GetActiveRecruitmentOfficers(It.IsAny<CancellationToken>()))
                .ReturnsAsync([officer]);

            var random = new FakeRandomProvider { DefaultDouble = 1.0 }; // all Chance(...) checks fail

            var sut = new RunOfficerLifeSimulationCommandHandler(
                dbMock.Object,
                random,
                Mock.Of<ILogger<RunOfficerLifeSimulationCommandHandler>>());

            // Act
            await sut.Handle(new RunOfficerLifeSimulationCommand(), CancellationToken.None);

            // Assert
            Assert.Equal(OfficerStatus.Active, officer.Status);
            dbMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }
    }
}
