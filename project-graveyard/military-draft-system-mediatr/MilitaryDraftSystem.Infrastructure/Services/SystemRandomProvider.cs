using MilitaryDraftSystem.Application.Common.Interfaces;

namespace MilitaryDraftSystem.Infrastructure.Services
{
    /// <summary>
    /// Default production implementation of <see cref="IRandomProvider"/> backed by <see cref="Random.Shared"/>.
    /// </summary>
    public sealed class SystemRandomProvider : IRandomProvider
    {
        public double NextDouble()
        {
            return Random.Shared.NextDouble();
        }

        public int Next(int minInclusive, int maxExclusive)
        {
            return Random.Shared.Next(minInclusive, maxExclusive);
        }

        public bool Chance(double probabilityPercent)
        {
            return Random.Shared.NextDouble() * 100 < probabilityPercent;
        }

        public T Pick<T>(IReadOnlyList<T> items)
        {
            if (items.Count == 0)
                throw new ArgumentException("Cannot pick from an empty collection.", nameof(items));

            return items[Random.Shared.Next(items.Count)];
        }

        public double NextGaussian(double mean, double standardDeviation)
        {
            // Box-Muller transform: converts two independent uniform samples
            // into a normally-distributed sample.
            var u1 = 1.0 - Random.Shared.NextDouble();
            var u2 = 1.0 - Random.Shared.NextDouble();

            var standardNormal = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2);

            return mean + (standardDeviation * standardNormal);
        }
    }
}
