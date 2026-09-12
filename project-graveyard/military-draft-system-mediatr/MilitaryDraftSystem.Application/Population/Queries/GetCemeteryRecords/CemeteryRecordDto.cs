namespace MilitaryDraftSystem.Application.Population.Queries.GetCemeteryRecords
{
    public sealed record CemeteryRecordDto(
        Guid Id,
        string SubjectType,
        Guid SubjectId,
        string FullName,
        string Reason,
        DateTimeOffset DiedAt,
        bool OriginalRecordDeleted);
}
