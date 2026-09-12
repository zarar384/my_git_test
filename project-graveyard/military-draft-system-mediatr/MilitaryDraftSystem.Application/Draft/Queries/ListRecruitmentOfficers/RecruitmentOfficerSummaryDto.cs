namespace MilitaryDraftSystem.Application.Draft.Queries.ListRecruitmentOfficers
{
    public sealed record RecruitmentOfficerSummaryDto(
        Guid Id,
        string FullName,
        string Department,
        Guid? PlayerId,
        string Status,
        bool IsActive,
        bool HasEndedCareer);
}
