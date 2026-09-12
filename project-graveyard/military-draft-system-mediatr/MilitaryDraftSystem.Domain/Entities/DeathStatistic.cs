using MilitaryDraftSystem.Domain.Common;
using MilitaryDraftSystem.Domain.Enums;

namespace MilitaryDraftSystem.Domain.Entities
{
    /// <summary>
    /// A historical, ever-growing counter of deaths grouped by subject type and
    /// reason. Statistics reflect actual events that occurred, so counters are
    /// only ever incremented, never recomputed from the current population.
    /// </summary>
    public sealed class DeathStatistic : Entity<Guid>
    {
        public SubjectType SubjectType { get; private set; }

        public DeathReason Reason { get; private set; }

        public long Count { get; private set; }

        private DeathStatistic()
        {
            // Required by EF Core.
        }

        public static DeathStatistic Create(SubjectType subjectType, DeathReason reason)
        {
            return new DeathStatistic
            {
                Id = Guid.NewGuid(),
                SubjectType = subjectType,
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
