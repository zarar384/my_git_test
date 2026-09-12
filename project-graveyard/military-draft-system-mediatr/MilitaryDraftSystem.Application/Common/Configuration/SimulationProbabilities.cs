namespace MilitaryDraftSystem.Application.Common.Configuration
{
    /// <summary>
    /// Centralizes every configurable probability used by the population simulation.
    /// Keeping all magic numbers here means business logic never hardcodes chances
    /// directly, and tuning the simulation only requires changing this one place.
    /// </summary>
    public static class SimulationProbabilities
    {
        public static class MedicalCategory
        {
            public const double FitWeight = 70;
            public const double LimitedFitWeight = 15;
            public const double TemporarilyUnfitWeight = 10;
            public const double PermanentlyUnfitWeight = 5;
        }

        public static class Age
        {
            public const double MinorWeight = 15;
            public const double YoungAdultWeight = 55;
            public const double MiddleAgedWeight = 20;
            public const double ElderlyWeight = 10;
        }

        public static class Birthday
        {
            /// <summary>
            /// Chance, per simulation tick, that a living citizen has a birthday.
            /// </summary>
            public const double OccursPercent = 5;
        }

        public static class Death
        {
            /// <summary>
            /// Base chance of dying from old age once a citizen passes <see cref="OldAgeThreshold"/>.
            /// Increases by <see cref="OldAgePercentPerYearOver"/> for every year above the threshold.
            /// </summary>
            public const int OldAgeThreshold = 70;

            public const double OldAgeBasePercent = 1;

            public const double OldAgePercentPerYearOver = 1;

            public const double DiseasePercent = 1;

            public const double AccidentPercent = 1;

            public const double MilitaryPercent = 2;

            public const double SuicidePercent = 1;

            /// <summary>
            /// Statistical distribution parameters used to assign each citizen's
            /// natural lifespan at creation time. Life expectancy differs between
            /// men and women, so each gender has its own mean and standard
            /// deviation. The resulting value is sampled from a normal
            /// distribution and clamped to a sane range so no citizen is immortal
            /// or dies of old age unrealistically young.
            /// </summary>
            public static class Lifespan
            {
                public const double MaleMeanYears = 75;

                public const double MaleStdDevYears = 8;

                public const double FemaleMeanYears = 81;

                public const double FemaleStdDevYears = 8;

                public const int MinYears = 60;

                public const int MaxYears = 100;
            }
        }

        public static class LifeEvents
        {
            /// <summary>
            /// Chance that a draft-eligible citizen (otherwise waiting for draft) instead
            /// receives a temporary deferment.
            /// </summary>
            public const double DefermentChancePercent = 8;

            /// <summary>
            /// Chance that a draft-eligible citizen (otherwise waiting for draft) instead
            /// receives a permanent exemption.
            /// </summary>
            public const double ExemptionChancePercent = 4;
        }

        public static class OfficerConsequences
        {
            /// <summary>
            /// Morale, in percentage points, an officer loses each time a citizen
            /// they personally drafted dies in military service.
            /// </summary>
            public const int MoraleLossPerMilitaryDeath = 15;
        }

        /// <summary>
        /// Chances, per simulation tick, that an active recruitment officer's
        /// career takes an unplanned turn. Only one outcome can occur per tick
        /// and outcomes are rolled in the order declared here.
        /// </summary>
        public static class Officer
        {
            public const double ResignationPercent = 0.2;

            public const double AccidentalDeathPercent = 0.1;

            public const double SuicidePercent = 0.05;

            public const double DiedOnDutyPercent = 0.05;
        }
    }
}
