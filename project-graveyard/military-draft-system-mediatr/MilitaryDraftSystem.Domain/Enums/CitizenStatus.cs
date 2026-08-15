namespace MilitaryDraftSystem.Domain.Enums
{
    /// <summary>
    /// Represents the current military draft status of a citizen.
    /// </summary>
    public enum CitizenStatus
    {
        /// <summary>
        /// Newborn citizens are not yet eligible for military service.
        /// </summary>
        Newborn = 0,

        /// <summary>
        /// Citizen is registered in the system.
        /// </summary>
        Registered = 1,

        /// <summary>
        /// Citizen is waiting to be drafted.
        /// </summary>
        WaitingForDraft = 2,

        /// <summary>
        /// Citizen has already been drafted.
        /// </summary>
        Drafted = 3,

        /// <summary>
        /// Citizen is permanently exempt from military service.
        /// </summary>
        Exempted = 4,

        /// <summary>
        /// Citizen has received a temporary deferment.
        /// </summary>
        Deferred = 5,

        /// <summary>
        /// Citizen is no longer eligible because of age.
        /// </summary>
        Retired = 6,

        /// <summary>
        /// Citizen has died and no longer takes part in the simulation.
        /// </summary>
        Deceased = 7
    }
}
