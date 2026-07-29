namespace MilitaryDraftSystem.Domain.Enums
{
    /// <summary>
    /// Represents the current military draft status of a citizen.
    /// </summary>
    public enum CitizenStatus
    {
        /// <summary>
        /// Citizen is registered in the system.
        /// </summary>
        Registered = 0,

        /// <summary>
        /// Citizen is waiting to be drafted.
        /// </summary>
        WaitingForDraft = 1,

        /// <summary>
        /// Citizen has already been drafted.
        /// </summary>
        Drafted = 2,

        /// <summary>
        /// Citizen is permanently exempt from military service.
        /// </summary>
        Exempted = 3,

        /// <summary>
        /// Citizen has received a temporary deferment.
        /// </summary>
        Deferred = 4,

        /// <summary>
        /// Citizen is no longer eligible because of age.
        /// </summary>
        Retired = 5
    }
}
