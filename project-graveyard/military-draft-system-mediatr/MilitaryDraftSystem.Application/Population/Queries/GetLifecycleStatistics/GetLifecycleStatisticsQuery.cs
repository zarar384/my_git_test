using MediatR;

namespace MilitaryDraftSystem.Application.Population.Queries.GetLifecycleStatistics
{
    /// <summary>
    /// Aggregates the historical death and worker-lifecycle counters so the
    /// game can display "how did people die" and "why did officers leave"
    /// reports without ever touching the active population/officer tables.
    /// </summary>
    public sealed record GetLifecycleStatisticsQuery : IRequest<LifecycleStatisticsDto>;
}
