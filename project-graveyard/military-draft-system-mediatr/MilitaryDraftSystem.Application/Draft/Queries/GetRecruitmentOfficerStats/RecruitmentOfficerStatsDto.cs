namespace MilitaryDraftSystem.Application.Draft.Queries.GetRecruitmentOfficerStats
{
    /// <summary>
    /// A snapshot of a recruitment officer's draft history and psychological state.
    /// </summary>
    public sealed record RecruitmentOfficerStatsDto(
        Guid Id,
        string FullName,
        string Department,
        int DraftedCitizensCount,
        int MoralePercent,
        int GuiltIncidentsCount,
        bool IsRetired,
        string Status,
        bool HasEndedCareer);
}
