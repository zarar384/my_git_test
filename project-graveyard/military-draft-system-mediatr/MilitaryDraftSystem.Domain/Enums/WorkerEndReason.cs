namespace MilitaryDraftSystem.Domain.Enums
{
    /// <summary>
    /// Explains why a recruitment officer's active career came to an end (or was
    /// paused), so the historical record and statistics stay meaningful.
    /// </summary>
    public enum WorkerEndReason
    {
        /// <summary>
        /// The officer took a temporary leave of absence.
        /// </summary>
        Leave = 0,

        /// <summary>
        /// The officer voluntarily resigned from their job.
        /// </summary>
        Resignation = 1,

        /// <summary>
        /// The officer voluntarily retired.
        /// </summary>
        Retirement = 2,

        /// <summary>
        /// The officer retired involuntarily after their morale collapsed to zero.
        /// </summary>
        MoraleCollapse = 3,

        /// <summary>
        /// The officer was dismissed from duty.
        /// </summary>
        Dismissal = 4,

        /// <summary>
        /// The officer died of a random, non-military cause.
        /// </summary>
        AccidentalDeath = 5,

        /// <summary>
        /// The officer took their own life.
        /// </summary>
        Suicide = 6,

        /// <summary>
        /// The officer died while performing their duties.
        /// </summary>
        DiedOnDuty = 7
    }
}
