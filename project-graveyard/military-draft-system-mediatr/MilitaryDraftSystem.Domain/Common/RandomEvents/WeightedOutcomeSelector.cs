namespace MilitaryDraftSystem.Domain.Common.RandomEvents
{
    /// <summary>
    /// Selects an outcome from a weighted catalog given a pre-rolled random value.
    /// Kept free of any randomness source so it stays pure Domain logic; callers
    /// are responsible for producing the roll (typically via IRandomProvider).
    /// </summary>
    public static class WeightedOutcomeSelector
    {
        /// <summary>
        /// Picks the outcome whose cumulative weight range contains the given roll.
        /// The roll must be in the range [0, total weight of all outcomes).
        /// </summary>
        public static TResult Select<TResult>(
            IReadOnlyList<WeightedOutcome<TResult>> outcomes,
            double roll)
        {
            if (outcomes.Count == 0)
                throw new ArgumentException("Cannot select from an empty outcome catalog.", nameof(outcomes));

            var cumulative = 0.0;

            foreach (var outcome in outcomes)
            {
                cumulative += outcome.Weight;

                if (roll < cumulative)
                    return outcome.Result;
            }

            // Guards against floating-point rounding at the upper boundary.
            return outcomes[^1].Result;
        }

        /// <summary>
        /// Computes the total weight of the outcome catalog, used to scale a random roll.
        /// </summary>
        public static double TotalWeight<TResult>(IReadOnlyList<WeightedOutcome<TResult>> outcomes)
        {
            var total = 0.0;

            foreach (var outcome in outcomes)
                total += outcome.Weight;

            return total;
        }
    }
}
