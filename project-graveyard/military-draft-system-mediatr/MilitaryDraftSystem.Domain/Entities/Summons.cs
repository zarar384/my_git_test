using MilitaryDraftSystem.Domain.Common;
using MilitaryDraftSystem.Domain.Enums;

namespace MilitaryDraftSystem.Domain.Entities
{
    /// <summary>
    /// Represents a military summons issued to a citizen.
    /// </summary>
    public sealed class Summons : Entity<Guid>
    {
        public Guid CitizenId { get; private set; }

        public DraftSource Source { get; private set; }

        public Guid? RecruitmentOfficerId { get; private set; }

        public Guid? AutomaticRecruitmentAgentId { get; private set; }

        public SummonsStatus Status { get; private set; }

        public DateTimeOffset IssuedAt { get; private set; }

        private Summons()
        {
            // Required by EF Core.
        }

        public static Summons Create(
            Guid citizenId,
            DraftSource source,
            Guid? recruitmentOfficerId,
            Guid? automaticRecruitmentAgentId,
            DateTimeOffset issuedAt)
        {
            // Create a new summons with its initial state.
            return new Summons
            {
                Id = Guid.NewGuid(),
                CitizenId = citizenId,
                Source = source,
                RecruitmentOfficerId = recruitmentOfficerId,
                AutomaticRecruitmentAgentId = automaticRecruitmentAgentId,
                IssuedAt = issuedAt,
                Status = SummonsStatus.Created
            };
        }

        /// <summary>
        /// Marks the summons as delivered to the citizen. Delivery is assumed to
        /// be instantaneous in this simulation: as soon as a summons is issued,
        /// it is considered delivered.
        /// </summary>
        public void MarkDelivered()
        {
            if (Status != SummonsStatus.Created)
                throw new InvalidOperationException($"Summons {Id} cannot be delivered from status {Status}.");

            Status = SummonsStatus.Delivered;
        }

        /// <summary>
        /// Marks the summons as attended: the citizen reported as required.
        /// </summary>
        public void MarkAttended()
        {
            if (Status != SummonsStatus.Delivered)
                throw new InvalidOperationException($"Summons {Id} cannot be attended from status {Status}.");

            Status = SummonsStatus.Attended;
        }

        /// <summary>
        /// Marks the summons as missed: the citizen failed to report.
        /// </summary>
        public void MarkMissed()
        {
            if (Status != SummonsStatus.Delivered)
                throw new InvalidOperationException($"Summons {Id} cannot be missed from status {Status}.");

            Status = SummonsStatus.Missed;
        }

        /// <summary>
        /// Cancels the summons. A summons can be cancelled at any point before
        /// it has been resolved as attended, missed or already cancelled.
        /// </summary>
        public void Cancel()
        {
            if (Status is SummonsStatus.Attended or SummonsStatus.Missed or SummonsStatus.Cancelled)
                throw new InvalidOperationException($"Summons {Id} cannot be cancelled from status {Status}.");

            Status = SummonsStatus.Cancelled;
        }

        /// <summary>
        /// Marks the summons as expired because the citizen was never
        /// resolved (attended, missed or cancelled) before its deadline.
        /// </summary>
        public void Expire()
        {
            if (Status is not (SummonsStatus.Created or SummonsStatus.Delivered))
                throw new InvalidOperationException($"Summons {Id} cannot expire from status {Status}.");

            Status = SummonsStatus.Expired;
        }
    }
}
