using MediatR;
using MilitaryDraftSystem.Application.Common.Interfaces;

namespace MilitaryDraftSystem.Application.Population.Queries.GetCemeteryRecords
{
    public sealed class GetCemeteryRecordsHandler
        : IRequestHandler<GetCemeteryRecordsQuery, IReadOnlyList<CemeteryRecordDto>>
    {
        private readonly IAppDbContext _db;

        public GetCemeteryRecordsHandler(IAppDbContext db)
        {
            _db = db;
        }

        public async Task<IReadOnlyList<CemeteryRecordDto>> Handle(
            GetCemeteryRecordsQuery request,
            CancellationToken cancellationToken)
        {
            var records = await _db.GetCemeteryRecords(cancellationToken);

            return records
                .Select(r => new CemeteryRecordDto(
                    r.Id,
                    r.SubjectType.ToString(),
                    r.SubjectId,
                    r.FullName,
                    r.Reason.ToString(),
                    r.DiedAt,
                    r.OriginalRecordDeleted))
                .ToList();
        }
    }
}
