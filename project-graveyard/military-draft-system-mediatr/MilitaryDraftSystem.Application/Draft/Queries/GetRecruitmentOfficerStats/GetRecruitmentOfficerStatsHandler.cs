using MediatR;
using MilitaryDraftSystem.Application.Common.Interfaces;

namespace MilitaryDraftSystem.Application.Draft.Queries.GetRecruitmentOfficerStats
{
    public sealed class GetRecruitmentOfficerStatsHandler
        : IRequestHandler<GetRecruitmentOfficerStatsQuery, RecruitmentOfficerStatsDto>
    {
        private readonly IAppDbContext _db;

        public GetRecruitmentOfficerStatsHandler(IAppDbContext db)
        {
            _db = db;
        }

        public async Task<RecruitmentOfficerStatsDto> Handle(
            GetRecruitmentOfficerStatsQuery request,
            CancellationToken cancellationToken)
        {
            var officer = await _db.GetRecruitmentOfficer(request.RecruitmentOfficerId, cancellationToken)
                ?? throw new InvalidOperationException($"Recruitment officer {request.RecruitmentOfficerId} was not found.");

            return new RecruitmentOfficerStatsDto(
                officer.Id,
                officer.FullName,
                officer.Department,
                officer.DraftedCitizensCount,
                officer.MoralePercent,
                officer.GuiltIncidentsCount,
                officer.IsRetired,
                officer.Status.ToString(),
                officer.HasEndedCareer);
        }
    }
}
