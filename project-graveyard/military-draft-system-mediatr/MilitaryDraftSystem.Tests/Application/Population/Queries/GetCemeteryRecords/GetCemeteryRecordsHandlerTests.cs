using Moq;
using MilitaryDraftSystem.Application.Common.Interfaces;
using MilitaryDraftSystem.Application.Population.Queries.GetCemeteryRecords;
using MilitaryDraftSystem.Domain.Entities;
using MilitaryDraftSystem.Domain.Enums;

namespace MilitaryDraftSystem.Tests.Application.Population.Queries.GetCemeteryRecords
{
    public class GetCemeteryRecordsHandlerTests
    {
        [Fact]
        public async Task Handle_ShouldMapCemeteryRecordsToDtos()
        {
            var record = CemeteryRecord.Create(
                SubjectType.Citizen,
                Guid.NewGuid(),
                "John Doe",
                DeathReason.OldAge,
                DateTimeOffset.UtcNow);

            var dbMock = new Mock<IAppDbContext>();
            dbMock.Setup(x => x.GetCemeteryRecords(It.IsAny<CancellationToken>()))
                .ReturnsAsync([record]);

            var sut = new GetCemeteryRecordsHandler(dbMock.Object);

            var result = await sut.Handle(new GetCemeteryRecordsQuery(), CancellationToken.None);

            var dto = Assert.Single(result);
            Assert.Equal(record.Id, dto.Id);
            Assert.Equal("Citizen", dto.SubjectType);
            Assert.Equal("John Doe", dto.FullName);
            Assert.Equal("OldAge", dto.Reason);
            Assert.False(dto.OriginalRecordDeleted);
        }
    }
}
