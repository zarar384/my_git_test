using MilitaryDraftSystem.Domain.Enums;

namespace MilitaryDraftSystem.Domain.ValueObjects
{
    /// <summary>
    /// Represents the immutable record of a citizen's death.
    /// Every death must contain a reason so the simulation stays meaningful rather than silent.
    /// </summary>
    public sealed record Death
    {
        public DeathReason Reason { get; private set; }

        public DateTimeOffset OccurredAt { get; private set; }

        private Death(DeathReason reason, DateTimeOffset occurredAt)
        {
            Reason = reason;
            OccurredAt = occurredAt;
        }

        private Death()
        {
            // Required by EF Core.
        }

        /// <summary>
        /// Creates a new death record for the specified reason and moment in time.
        /// </summary>
        public static Death Create(DeathReason reason, DateTimeOffset occurredAt)
        {
            return new Death(reason, occurredAt);
        }

        /// <summary>
        /// Produces a human-readable description of the cause of death.
        /// </summary>
        public string Describe()
        {
            return Reason switch
            {
                DeathReason.OldAge => "Old age.",
                DeathReason.HeartAttack => "Heart attack.",
                DeathReason.Cancer => "Cancer.",
                DeathReason.UnknownIllness => "Unknown illness.",
                DeathReason.TrafficAccident => "Traffic accident.",
                DeathReason.Drowned => "Drowned.",
                DeathReason.LightningStrike => "Lightning strike.",
                DeathReason.FellFromStairs => "Fell from stairs.",
                DeathReason.FriendlyFireDuringTraining => "Friendly fire during military training.",
                DeathReason.KilledInCombat => "Killed during combat.",
                DeathReason.Suicide => "Suicide.",
                _ => "Unknown cause."
            };
        }
    }
}
