namespace LeaveMeAloneCSharp.Functional
{
    /// <summary>
    /// Represents the result of a match operation, allowing a fallback value
    /// to be provided when no rule matched.
    /// </summary>
    public class MatchValueOrDefault<TInput, TOutput>
    {
        private readonly TOutput value;
        private readonly TInput originalValue;

        public MatchValueOrDefault(TOutput value, TInput originalValue)
        {
            this.value = value;
            this.originalValue = originalValue;
        }

        public TOutput Value => this.value;
        public bool IsMatched => !EqualityComparer<TOutput>.Default.Equals(default, this.value);

        /// <summary>
        /// Returns the matched value, or evaluates the specified fallback
        /// using the original input when no match was found.
        /// </summary>
        public TOutput DefaultMatch(Func<TInput, TOutput> defaultMatch) =>
            EqualityComparer<TOutput>.Default.Equals(default, this.value)
                ? defaultMatch(originalValue)
                : this.value;
    }
}
