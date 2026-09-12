using MilitaryDraftSystem.Domain.Common;
using MilitaryDraftSystem.Domain.Enums;
using MilitaryDraftSystem.Domain.Events;

namespace MilitaryDraftSystem.Domain.Entities
{
    /// <summary>
    /// Represents a recruitment officer responsible for manual drafting.
    /// The officer is a real person, controlled by a <see cref="Player"/>, not
    /// an anonymous identifier: every draft decision is attributed to them,
    /// and the resulting history is the foundation for future responsibility
    /// and consequence mechanics. The officer has a full life cycle: they may
    /// keep working, take leave, resign, retire, be fired, or die, and every
    /// such transition is preserved as history.
    /// </summary>
    public sealed class RecruitmentOfficer : Entity<Guid>
    {
        public string FullName { get; private set; } = null!;

        public string Department { get; private set; } = null!;

        /// <summary>
        /// The player controlling this officer. Nullable so unowned/legacy
        /// officers (or NPC officers) remain valid, and so the current
        /// single-player game is simply a special case of the future
        /// multiplayer model.
        /// </summary>
        public Guid? PlayerId { get; private set; }

        /// <summary>
        /// Total number of citizens this officer has personally drafted.
        /// </summary>
        public int DraftedCitizensCount { get; private set; }

        /// <summary>
        /// The officer's psychological state, expressed as a percentage (0-100).
        /// Starts at 100 and drops when a citizen they personally drafted dies
        /// in military service. Never goes below 0.
        /// </summary>
        public int MoralePercent { get; private set; } = 100;

        /// <summary>
        /// Number of times this officer has had to bear the guilt of a
        /// drafted citizen's death in military service.
        /// </summary>
        public int GuiltIncidentsCount { get; private set; }

        /// <summary>
        /// The officer's current life-cycle state.
        /// </summary>
        public OfficerStatus Status { get; private set; }

        /// <summary>
        /// The reason the officer's career ended or was paused, if applicable.
        /// </summary>
        public WorkerEndReason? EndReason { get; private set; }

        /// <summary>
        /// When the officer's current status last changed.
        /// </summary>
        public DateTimeOffset? StatusChangedAt { get; private set; }

        /// <summary>
        /// Death record, populated only if the officer has died.
        /// </summary>
        public ValueObjects.Death? Death { get; private set; }

        /// <summary>
        /// Whether the officer has permanently ended their career (resigned,
        /// retired, fired or died). A player whose officer reaches this state
        /// can no longer act as that officer and must start a new game with a
        /// new officer.
        /// </summary>
        public bool HasEndedCareer =>
            Status is OfficerStatus.Resigned or OfficerStatus.Retired or OfficerStatus.Fired or OfficerStatus.Deceased;

        /// <summary>
        /// Whether the officer may currently be assigned to draft citizens.
        /// </summary>
        public bool IsActive => Status == OfficerStatus.Active;

        /// <summary>
        /// Preserved for backward compatibility: true whenever the officer's
        /// career has ended via retirement (voluntary or morale collapse).
        /// </summary>
        public bool IsRetired => Status == OfficerStatus.Retired;

        public RecruitmentOfficer(
            Guid id,
            string fullName,
            string department,
            Guid? playerId = null)
        {
            Id = id;
            FullName = fullName;
            Department = department;
            PlayerId = playerId;
            DraftedCitizensCount = 0;
            MoralePercent = 100;
            GuiltIncidentsCount = 0;
            Status = OfficerStatus.Active;
        }

        private RecruitmentOfficer()
        {
            // Required by EF Core.
        }

        /// <summary>
        /// Records that this officer personally drafted a citizen.
        /// </summary>
        public void RegisterDraftedCitizen()
        {
            DraftedCitizensCount++;
        }

        /// <summary>
        /// Applies a psychological toll to the officer following the death,
        /// in military service, of a citizen they personally drafted.
        /// </summary>
        public void ApplyGuilt(int moraleLossPercent)
        {
            if (moraleLossPercent < 0)
                throw new ArgumentOutOfRangeException(nameof(moraleLossPercent), "Morale loss cannot be negative.");

            MoralePercent = Math.Max(0, MoralePercent - moraleLossPercent);
            GuiltIncidentsCount++;

            if (MoralePercent == 0 && Status == OfficerStatus.Active)
            {
                EndCareer(OfficerStatus.Retired, WorkerEndReason.MoraleCollapse, DateTimeOffset.UtcNow);
            }
        }

        /// <summary>
        /// Sends the officer on temporary leave. They cannot draft citizens
        /// while away, but may return to active duty later.
        /// </summary>
        public void GoOnLeave(DateTimeOffset occurredAt)
        {
            EnsureActive();

            Status = OfficerStatus.OnLeave;
            EndReason = WorkerEndReason.Leave;
            StatusChangedAt = occurredAt;

            RaiseDomainEvent(new OfficerCareerEndedDomainEvent(Id, WorkerEndReason.Leave));
        }

        /// <summary>
        /// Returns the officer to active duty from leave.
        /// </summary>
        public void ReturnFromLeave(DateTimeOffset occurredAt)
        {
            if (Status != OfficerStatus.OnLeave)
                throw new InvalidOperationException($"Officer {Id} cannot return from leave from status {Status}.");

            Status = OfficerStatus.Active;
            EndReason = null;
            StatusChangedAt = occurredAt;
        }

        /// <summary>
        /// The officer voluntarily quits their job. Their career has ended.
        /// </summary>
        public void Resign(DateTimeOffset occurredAt)
        {
            EndCareer(OfficerStatus.Resigned, WorkerEndReason.Resignation, occurredAt);
        }

        /// <summary>
        /// The officer voluntarily retires. Their career has ended.
        /// </summary>
        public void RetireVoluntarily(DateTimeOffset occurredAt)
        {
            EndCareer(OfficerStatus.Retired, WorkerEndReason.Retirement, occurredAt);
        }

        /// <summary>
        /// The officer is dismissed from duty. Their career has ended.
        /// </summary>
        public void Fire(DateTimeOffset occurredAt)
        {
            EndCareer(OfficerStatus.Fired, WorkerEndReason.Dismissal, occurredAt);
        }

        /// <summary>
        /// The officer dies of a random, non-military cause while off duty.
        /// Every death must carry a reason so the simulation never ends a
        /// life without explanation.
        /// </summary>
        public void Die(DeathReason reason, DateTimeOffset occurredAt)
        {
            if (Status == OfficerStatus.Deceased)
                return;

            Death = ValueObjects.Death.Create(reason, occurredAt);
            Status = OfficerStatus.Deceased;
            EndReason = reason == DeathReason.Suicide ? WorkerEndReason.Suicide : WorkerEndReason.AccidentalDeath;
            StatusChangedAt = occurredAt;

            RaiseDomainEvent(new OfficerDiedDomainEvent(Id, Death, EndReason.Value));
        }

        /// <summary>
        /// The officer dies while performing their duties.
        /// </summary>
        public void DieOnDuty(DeathReason reason, DateTimeOffset occurredAt)
        {
            if (Status == OfficerStatus.Deceased)
                return;

            Death = ValueObjects.Death.Create(reason, occurredAt);
            Status = OfficerStatus.Deceased;
            EndReason = WorkerEndReason.DiedOnDuty;
            StatusChangedAt = occurredAt;

            RaiseDomainEvent(new OfficerDiedDomainEvent(Id, Death, EndReason.Value));
        }

        #region Helpers
        private void EnsureActive()
        {
            if (Status != OfficerStatus.Active)
                throw new InvalidOperationException($"Officer {Id} must be active to perform this action, but is {Status}.");
        }

        private void EndCareer(OfficerStatus status, WorkerEndReason reason, DateTimeOffset occurredAt)
        {
            if (HasEndedCareer)
                return;

            Status = status;
            EndReason = reason;
            StatusChangedAt = occurredAt;

            RaiseDomainEvent(new OfficerCareerEndedDomainEvent(Id, reason));
        }
        #endregion
    }
}
