using MilitaryDraftSystem.Domain.Common;

namespace MilitaryDraftSystem.Domain.Entities
{
    /// <summary>
    /// Represents the automatic population generation process.
    /// Every generation begins here.
    /// </summary>
    public sealed class PopulationGenerator : Entity<Guid>
    {
        /// <summary>
        /// Indicates whether the population generator is enabled.
        /// </summary>
        public bool Enabled { get; private set; }

        /// <summary>
        /// Specifies how often a new generation should occur.
        /// </summary>
        public TimeSpan GenerationInterval { get; private set; }

        /// <summary>
        /// Stores the timestamp of the last successful generation.
        /// </summary>
        public DateTimeOffset? LastGenerationAt { get; private set; }

        /// <summary>
        /// Minimum number of citizens created in a single generation.
        /// </summary>
        public int MinCitizensPerGeneration { get; private set; }

        /// <summary>
        /// Maximum number of citizens created in a single generation.
        /// </summary>
        public int MaxCitizensPerGeneration { get; private set; }

        /// <summary>
        /// Minimum age assigned to a generated citizen.
        /// </summary>
        public int MinAge { get; private set; }

        /// <summary>
        /// Maximum age assigned to a generated citizen.
        /// </summary>
        public int MaxAge { get; private set; }

        /// <summary>
        /// Chance that a generated citizen is a student.
        /// Value is expressed as a percentage (0-100).
        /// </summary>
        public int StudentChance { get; private set; }

        /// <summary>
        /// Chance that a generated citizen has a criminal record.
        /// Value is expressed as a percentage (0-100).
        /// </summary>
        public int CriminalRecordChance { get; private set; }

        /// <summary>
        /// Records the last successful generation.
        /// </summary>
        public void MarkExecuted(DateTimeOffset executedAt)
        {
            LastGenerationAt = executedAt;
        }

        public PopulationGenerator(
            Guid id,
            bool enabled,
            TimeSpan generationInterval,
            int minCitizensPerGeneration,
            int maxCitizensPerGeneration,
            int minAge,
            int maxAge,
            int studentChance,
            int criminalRecordChance)
        {
            Id = id;
            Enabled = enabled;
            GenerationInterval = generationInterval;
            MinCitizensPerGeneration = minCitizensPerGeneration;
            MaxCitizensPerGeneration = maxCitizensPerGeneration;
            MinAge = minAge;
            MaxAge = maxAge;
            StudentChance = studentChance;
            CriminalRecordChance = criminalRecordChance;
        }

        private PopulationGenerator()
        {
            // Required by EF Core.
        }

        public void Enable()
        {
            Enabled = true;
        }

        public void Disable()
        {
            Enabled = false;
        }
    }
}