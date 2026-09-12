namespace MilitaryDraftSystem.Domain.Enums
{
    /// <summary>
    /// Represents the current life-cycle state of a recruitment officer.
    /// A recruitment officer is treated as a real person controlled by a player,
    /// so this state carries meaning beyond a simple boolean flag.
    /// </summary>
    public enum OfficerStatus
    {
        /// <summary>
        /// The officer is actively working and may draft citizens.
        /// </summary>
        Active = 0,

        /// <summary>
        /// The officer is temporarily away from duty and cannot draft citizens,
        /// but may return to active duty later.
        /// </summary>
        OnLeave = 1,

        /// <summary>
        /// The officer voluntarily left their job. Their career has ended.
        /// </summary>
        Resigned = 2,

        /// <summary>
        /// The officer retired from duty (e.g. voluntarily, or after their
        /// morale collapsed). Their career has ended.
        /// </summary>
        Retired = 3,

        /// <summary>
        /// The officer was dismissed from duty. Their career has ended.
        /// </summary>
        Fired = 4,

        /// <summary>
        /// The officer has died and can no longer act. Their career has ended.
        /// </summary>
        Deceased = 5
    }
}
