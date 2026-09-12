using MediatR;
using MilitaryDraftSystem.Application.Common.Interfaces;

namespace MilitaryDraftSystem.Application.Draft.Queries.ListRecruitmentOfficers
{
    public sealed class ListRecruitmentOfficersHandler
        : IRequestHandler<ListRecruitmentOfficersQuery, IReadOnlyList<RecruitmentOfficerSummaryDto>>
    {
        private readonly IAppDbContext _db;

        public ListRecruitmentOfficersHandler(IAppDbContext db)
        {
            _db = db;
        }

        public async Task<IReadOnlyList<RecruitmentOfficerSummaryDto>> Handle(
            ListRecruitmentOfficersQuery request,
            CancellationToken cancellationToken)
        {
            var officers = await _db.GetRecruitmentOfficers(cancellationToken);

            return officers
                .Select(o => new RecruitmentOfficerSummaryDto(
                    o.Id,
                    o.FullName,
                    o.Department,
                    o.PlayerId,
                    o.Status.ToString(),
                    o.IsActive,
                    o.HasEndedCareer))
                .ToList();
        }
    }
}
