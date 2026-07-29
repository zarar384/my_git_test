namespace MilitaryDraftSystem.Domain.Enums
{
    /// <summary>
    /// Specifies who initiated the military draft.
    /// </summary>
    public enum DraftSource
    {
        /// <summary>
        /// Draft initiated by a recruitment officer.
        /// </summary>
        RecruitmentOfficer = 0,

        /// <summary>
        /// Draft initiated automatically by the system.
        /// </summary>
        AutomaticAgent = 1
    }
}
