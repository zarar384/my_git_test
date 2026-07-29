namespace MilitaryDraftSystem.Domain.Enums
{
    /// <summary>
    /// Represents the medical fitness category of a citizen.
    /// </summary>
    public enum MedicalCategory
    {
        /// <summary>
        /// Citizen is fully fit for military service.
        /// </summary>
        Fit = 0,

        /// <summary>
        /// Citizen has limited fitness for military service.
        /// </summary>
        LimitedFit = 1,

        /// <summary>
        /// Citizen is temporarily unfit for military service.
        /// </summary>
        TemporarilyUnfit = 2,

        /// <summary>
        /// Citizen is permanently unfit for military service.
        /// </summary>
        PermanentlyUnfit = 3
    }
}