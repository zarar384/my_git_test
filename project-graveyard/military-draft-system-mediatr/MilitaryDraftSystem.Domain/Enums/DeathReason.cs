namespace MilitaryDraftSystem.Domain.Enums
{
    /// <summary>
    /// Represents the cause of a citizen's death.
    /// The simulation should feel alive, so deaths come from a variety of causes.
    /// </summary>
    public enum DeathReason
    {
        /// <summary>
        /// Death caused by advanced age.
        /// </summary>
        OldAge = 0,

        /// <summary>
        /// Death caused by a heart attack.
        /// </summary>
        HeartAttack = 1,

        /// <summary>
        /// Death caused by cancer.
        /// </summary>
        Cancer = 2,

        /// <summary>
        /// Death caused by an unknown or undiagnosed illness.
        /// </summary>
        UnknownIllness = 3,

        /// <summary>
        /// Death caused by a traffic accident.
        /// </summary>
        TrafficAccident = 4,

        /// <summary>
        /// Death caused by drowning.
        /// </summary>
        Drowned = 5,

        /// <summary>
        /// Death caused by a lightning strike.
        /// </summary>
        LightningStrike = 6,

        /// <summary>
        /// Death caused by falling from stairs.
        /// </summary>
        FellFromStairs = 7,

        /// <summary>
        /// Death caused by friendly fire during military training.
        /// </summary>
        FriendlyFireDuringTraining = 8,

        /// <summary>
        /// Death caused during active combat.
        /// </summary>
        KilledInCombat = 9,

        /// <summary>
        /// Death caused by suicide.
        /// </summary>
        Suicide = 10
    }
}
