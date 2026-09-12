using MilitaryDraftSystem.Application.Common.Interfaces;

namespace MilitaryDraftSystem.Tests.Common
{
    /// <summary>
    /// Deterministic <see cref="IRandomProvider"/> test double. Every random operation
    /// is driven by pre-configured, queued values so simulation tests never become flaky.
    /// </summary>
    public sealed class FakeRandomProvider : IRandomProvider
    {
        private readonly Queue<double> _doubleValues = new();
        private readonly Queue<int> _intValues = new();
        private readonly Queue<double> _gaussianValues = new();

        /// <summary>
        /// Fixed value returned by <see cref="NextDouble"/> when no queued value is configured.
        /// </summary>
        public double DefaultDouble { get; set; }

        /// <summary>
        /// Fixed index returned by <see cref="Pick{T}"/> / <see cref="Next"/> when no queued value is configured.
        /// </summary>
        public int DefaultInt { get; set; }

        /// <summary>
        /// Fixed value returned by <see cref="NextGaussian"/> when no queued value is configured.
        /// </summary>
        public double DefaultGaussian { get; set; }

        public void EnqueueDouble(double value)
        {
            _doubleValues.Enqueue(value);
        }

        public void EnqueueInt(int value)
        {
            _intValues.Enqueue(value);
        }

        public void EnqueueGaussian(double value)
        {
            _gaussianValues.Enqueue(value);
        }

        public double NextDouble()
        {
            return _doubleValues.Count > 0
                ? _doubleValues.Dequeue()
                : DefaultDouble;
        }

        public int Next(int minInclusive, int maxExclusive)
        {
            if (_intValues.Count > 0)
                return _intValues.Dequeue();

            return minInclusive + (DefaultInt % Math.Max(1, maxExclusive - minInclusive));
        }

        public bool Chance(double probabilityPercent)
        {
            return NextDouble() * 100 < probabilityPercent;
        }

        public T Pick<T>(IReadOnlyList<T> items)
        {
            if (items.Count == 0)
                throw new ArgumentException("Cannot pick from an empty collection.", nameof(items));

            var index = _intValues.Count > 0
                ? _intValues.Dequeue()
                : DefaultInt;

            return items[index % items.Count];
        }

        public double NextGaussian(double mean, double standardDeviation)
        {
            return _gaussianValues.Count > 0
                ? _gaussianValues.Dequeue()
                : DefaultGaussian != 0 ? DefaultGaussian : mean;
        }
    }
}
