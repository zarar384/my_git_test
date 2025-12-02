using LeaveMeAloneFuncSkillForge.DiscriminatedUnions;
using System.Net.Http.Headers;

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
            Func<T, T> update) =>
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

        /// <summary>
        /// Runs ftransformer (Select, Where chain..) ande then aggregator (Aggregate, Join, ToList..)
        /// </summary>
        public static TFinalOut Transduce<TIn, TMid, TFinalOut>(
            this IEnumerable<TIn> source,
            Func<IEnumerable<TIn>, IEnumerable<TMid>> transformer,
            Func<IEnumerable<TMid>, TFinalOut> aggregator)
                => aggregator(transformer(source)
            );

        /// <summary>
        /// Convert transformer + aggregator into a reusable function (transducer)
        /// </summary>
        public static Func<IEnumerable<TIn>, TFinalOut> ToTransducer<TIn, TMid, TFinalOut>(
            this Func<IEnumerable<TIn>, IEnumerable<TMid>> transformer,
            Func<IEnumerable<TMid>, TFinalOut> aggregator)
                => items => aggregator(transformer(items));

        public static string ToFormattedString<T>(this List<T> list, string separator = ", ")
            => list == null ? string.Empty : string.Join(separator, list);

        public static T Tap<T>(this T @this, Action<T> action)
        {
            action(@this);
            return @this;
        }

        public static Either<TLeft, TRight2> Bind<TLeft, TRight, TRight2>(
            this Either<TLeft, TRight> either,
            Func<TRight, Either<TLeft, TRight2>> func)
            => either switch
            {
                Left<TLeft, TRight> left => new Left<TLeft, TRight2>(left.Value),
                Right<TLeft, TRight> right => func(right.Value),
                _ => throw new InvalidOperationException("Unknown Either type")
            };

        public static Either<TLeft, TRight2> Map<TLeft, TRight, TRight2>(
            this Either<TLeft, TRight> either,
            Func<TRight, TRight2> func)
            => either.Bind(r => new Right<TLeft, TRight2>(func(r)));

        public static Maybe<TRight2> Bind<TRight, TRight2>(
            this Maybe<TRight> maybe,
            Func<TRight, Maybe<TRight2>> func)
            => maybe switch
            {
                Something<TRight> something => func(something.Value),
                Nothing<TRight> => new Nothing<TRight2>(),
                Error<TRight> error => new Error<TRight2>(error.CapturedError),
                _ => throw new InvalidOperationException("Unknown Maybe type")
            };

        public static Maybe<TRight2> Map<TRight, TRight2>(
            this Maybe<TRight> maybe,
            Func<TRight, TRight2> func)
            => maybe.Bind(r => new Something<TRight2>(func(r)));
    }
}