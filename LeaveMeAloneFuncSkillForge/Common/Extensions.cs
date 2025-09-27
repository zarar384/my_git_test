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

        public static bool AllAdjacent<T>(
            this IEnumerable<T> source,
            Func<T, T, bool> evaluator) =>
            source?.Zip(source.Skip(1), (prev, next) => evaluator(prev, next))
                .All(x => x) ?? true;

        public static bool AnyAdjacent<T>(
            this IEnumerable<T> source,
            Func<T, T, bool> evaluator) =>
            source?.Zip(source.Skip(1), (prev, next) => evaluator(prev, next))
                .Any(x => x) ?? false;

        public static T AggregateUntil<T>(
            this T @this,
            Func<T, bool> endCondition,
            Func<T,T> update)=>
            endCondition(@this)
            ? @this
            : AggregateUntil(update(@this), endCondition, update);

        /// <summary>
        /// Concatenates two functions into one
        /// 1. the first result is f1 (TIn => TOut1),
        /// 2. then the result of f1 is passed to f2 (TOut1 => TOut2),
        /// 3. returns a new function (TIn => TOut2).
        /// </summary>
        public static Func<TIn, TOut2> Compose<TIn, TOut1, TOut2>(
             this Func<TIn, TOut1> f1,
             Func<TOut1, TOut2> f2)
             => x => f2(f1(x));
    }
}