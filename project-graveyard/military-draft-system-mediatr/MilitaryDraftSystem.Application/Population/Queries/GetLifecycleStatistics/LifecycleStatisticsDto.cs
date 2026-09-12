namespace MilitaryDraftSystem.Application.Population.Queries.GetLifecycleStatistics
{
    public sealed record DeathStatisticDto(string SubjectType, string Reason, long Count);

    public sealed record WorkerLifecycleStatisticDto(string Reason, long Count);

    public sealed record LifecycleStatisticsDto(
        IReadOnlyList<DeathStatisticDto> DeathStatistics,
        IReadOnlyList<WorkerLifecycleStatisticDto> WorkerLifecycleStatistics);
}
