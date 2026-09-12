using MilitaryDraftSystem.Domain.Common;
using MilitaryDraftSystem.Domain.Enums;

namespace MilitaryDraftSystem.Domain.Entities
{
    /// <summary>
    /// A historical, ever-growing counter of how recruitment officers' careers
    /// have ended (or been paused), grouped by reason. Deactivating or
    /// deleting a specific officer never erases these counters.
    /// </summary>
    public sealed class WorkerLifecycleStatistic : Entity<Guid>
    {
        public WorkerEndReason Reason { get; private set; }

        public long Count { get; private set; }

        private WorkerLifecycleStatistic()
        {
            // Required by EF Core.
        }

        public static WorkerLifecycleStatistic Create(WorkerEndReason reason)
        {
            return new WorkerLifecycleStatistic
            {
                Id = Guid.NewGuid(),
                Reason = reason,
                Count = 0
            };
        }

        public void Increment()
        {
            Count++;
        }
    }
}
