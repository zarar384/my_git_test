namespace MilitaryDraftSystem.Application.Common.Interfaces
{
    /// <summary>
    /// Provides primitive randomization operations used to drive all simulation
    /// randomness through a single, testable and configurable seam.
    /// This abstraction has no business meaning by itself; probability
    /// decisions belong to the callers (Application services, weighted event catalogs).
    /// </summary>
    public interface IRandomProvider
    {
        /// <summary>
        /// Returns a random floating-point value in the range [0.0, 1.0).
        /// </summary>
        double NextDouble();

        /// <summary>
        /// Returns a random integer in the range [minInclusive, maxExclusive).
        /// </summary>
        int Next(int minInclusive, int maxExclusive);

        /// <summary>
        /// Returns true with the specified probability, expressed as a percentage (0-100).
        /// </summary>
        bool Chance(double probabilityPercent);

        /// <summary>
        /// Selects a random element from the specified collection.
        /// </summary>
        T Pick<T>(IReadOnlyList<T> items);

        /// <summary>
        /// Returns a random value sampled from a normal (Gaussian) distribution
        /// with the given mean and standard deviation. Used for statistical
        /// modeling (e.g. natural lifespan) rather than uniform chance rolls.
        /// </summary>
        double NextGaussian(double mean, double standardDeviation);
    }
}
