namespace MilitaryDraftSystem.Domain.Enums
{
    /// <summary>
    /// Represents the lifecycle state of a military summons.
    /// </summary>
    public enum SummonsStatus
    {
        /// <summary>
        /// Summons has been created.
        /// </summary>
        Created = 0,

        /// <summary>
        /// Summons has been delivered to the citizen.
        /// </summary>
        Delivered = 1,

        /// <summary>
        /// Citizen attended the summons.
        /// </summary>
        Attended = 2,

        /// <summary>
        /// Citizen failed to attend the summons.
        /// </summary>
        Missed = 3,

        /// <summary>
        /// Summons has been cancelled.
        /// </summary>
        Cancelled = 4,

        /// <summary>
        /// Summons has expired.
        /// </summary>
        Expired = 5
    }
}
