namespace LeaveMeAloneFuncSkillForge.Functional
{
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

        public TOutput DefaultMatch(Func<TInput, TOutput> defaultMatch) =>
            EqualityComparer<TOutput>.Default.Equals(default, this.value)
                ? defaultMatch(originalValue)
                : this.value;
    }
}
