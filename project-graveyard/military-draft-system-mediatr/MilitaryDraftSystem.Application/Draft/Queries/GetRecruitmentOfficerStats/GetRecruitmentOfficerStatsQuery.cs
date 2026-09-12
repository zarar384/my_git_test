using MediatR;

namespace MilitaryDraftSystem.Application.Draft.Queries.GetRecruitmentOfficerStats
{
    /// <summary>
    /// Requests the current standing (draft history, morale, retirement status)
    /// of a recruitment officer.
    /// </summary>
    public sealed record GetRecruitmentOfficerStatsQuery(Guid RecruitmentOfficerId)
        : IRequest<RecruitmentOfficerStatsDto>;
}
