namespace MilitaryDraftSystem.Application.Common.Configuration
{
    /// <summary>
    /// Centralizes retention/scheduling parameters used by the population and
    /// worker life-cycle background processes.
    /// </summary>
    public static class SimulationRetention
    {
        /// <summary>
        /// How long a deceased citizen remains in the main population table
        /// after death before being physically purged. This gives the system
        /// time to use the record for history/statistics/cemetery purposes
        /// before removing it to keep the population table lean.
        /// </summary>
        public static readonly TimeSpan DeceasedCitizenRetention = TimeSpan.FromMinutes(5);
    }
}
