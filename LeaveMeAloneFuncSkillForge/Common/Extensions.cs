namespace LeaveMeAloneFuncSkillForge.Common
{
    public static class Extensions
    {
        //public static TOutput Match<TInput, TOutput>(
        //   this TInput @this,
        //    params (Func<TInput, bool> IsMatch,
        //    Func<TInput, TOutput> Transform)[] matches)
        //{
        //    var match = matches.FirstOrDefault(x => x.IsMatch(@this));
        //    if (match.Transform == null)
        //        return default;
        //    return match.Transform(@this);
        //}

        public static MatchValueOrDefault<TInput, TOutput> Match<TInput, TOutput>(
            this TInput @this,
            params (Func<TInput, bool>, // or using KeyValuePair
            Func<TInput, TOutput>)[] predicates)
        {
            var match = predicates.FirstOrDefault(x => x.Item1(@this));
            var returnValue = match.Item2 != null ? match.Item2(@this) : default;
            return new MatchValueOrDefault<TInput, TOutput>(returnValue, @this);
        }

        public static Func<TKey, TValue> ToLookup<TKey, TValue>(
            this IDictionary<TKey, TValue> @this)
        {
            return x => @this.TryGetValue(x, out TValue? value) ? value : default;
        }

        public static Func<TKey, TValue> ToLookup<TKey, TValue>(
            this IDictionary<TKey, TValue> @this,
            TValue defaultVal)
        {
            return x => @this.ContainsKey(x) ? @this[x] : default;
        }

        public static int ToIntOrDefault(this object @this, int defaultValue = 0) =>
            int.TryParse(@this?.ToString() ?? string.Empty, out var parserValue)
                ? parserValue
                : defaultValue;

        public static string ToStringOrDefault(
            this object @this, 
            string defaultValue = "") =>
            string.IsNullOrWhiteSpace(@this?.ToString() ?? string.Empty)
                ? defaultValue
                : @this?.ToString() ?? string.Empty;
    }
}
