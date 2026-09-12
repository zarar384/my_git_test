using MediatR;
using MilitaryDraftSystem.Application.Common.Interfaces;

namespace MilitaryDraftSystem.Application.Population.Queries.GetLifecycleStatistics
{
    public sealed class GetLifecycleStatisticsHandler
        : IRequestHandler<GetLifecycleStatisticsQuery, LifecycleStatisticsDto>
    {
        private readonly IAppDbContext _db;

        public GetLifecycleStatisticsHandler(IAppDbContext db)
        {
            _db = db;
        }

        public async Task<LifecycleStatisticsDto> Handle(
            GetLifecycleStatisticsQuery request,
            CancellationToken cancellationToken)
        {
            var deathStatistics = await _db.GetDeathStatistics(cancellationToken);
            var workerStatistics = await _db.GetWorkerLifecycleStatistics(cancellationToken);

            return new LifecycleStatisticsDto(
                deathStatistics
                    .Select(s => new DeathStatisticDto(s.SubjectType.ToString(), s.Reason.ToString(), s.Count))
                    .ToList(),
                workerStatistics
                    .Select(s => new WorkerLifecycleStatisticDto(s.Reason.ToString(), s.Count))
                    .ToList());
        }
    }
}
