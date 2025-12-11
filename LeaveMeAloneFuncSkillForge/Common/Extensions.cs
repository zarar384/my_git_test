using LeaveMeAloneFuncSkillForge.DiscriminatedUnions;
using LeaveMeAloneFuncSkillForge.Functional.Monads;

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

        /// <summary>
        /// Bind for Either: continues the chain if Right, propagates Left otherwise.
        /// </summary>
        /// <typeparam name="TLeft">Type of the Left (error) value.</typeparam>
        /// <typeparam name="TRight">Type of the Right (success) value in the input Either.</typeparam>
        /// <typeparam name="TRight2">Type of the Right (success) value in the resulting Either.</typeparam>
        /// <param name="either">The Either to operate on.</param>
        /// <param name="func">Function to apply if Right; must return Either<TLeft, TRight2>.</param>
        /// <returns>New Either with TRight2 if Right, original Left if Left.</returns>
        public static Either<TLeft, TRight2> Bind<TLeft, TRight, TRight2>(
            this Either<TLeft, TRight> either,
            Func<TRight, Either<TLeft, TRight2>> func)
            => either switch
            {
                Left<TLeft, TRight> left => new Left<TLeft, TRight2>(left.Value),
                Right<TLeft, TRight> right => func(right.Value),
                _ => throw new InvalidOperationException("Unknown Either type")
            };

        /// <summary>
        /// Map for Either: transforms Right value while keeping Left unchanged.
        /// </summary>
        /// <typeparam name="TLeft">Type of the Left (error) value.</typeparam>
        /// <typeparam name="TRight">Type of the Right value in the input Either.</typeparam>
        /// <typeparam name="TRight2">Type of the Right value after mapping.</typeparam>
        /// <param name="either">The Either to map.</param>
        /// <param name="func">Function to transform the Right value.</param>
        /// <returns>New Either with transformed Right or original Left.</returns>
        public static Either<TLeft, TRight2> Map<TLeft, TRight, TRight2>(
            this Either<TLeft, TRight> either,
            Func<TRight, TRight2> func)
            => either.Bind(r => new Right<TLeft, TRight2>(func(r)));

        /// <summary>
        /// Bind for Maybe: continues the chain if Something, propagates Nothing or Error otherwise.
        /// </summary>
        /// <typeparam name="TRight">Type of the value inside the input Maybe.</typeparam>
        /// <typeparam name="TRight2">Type of the value inside the resulting Maybe.</typeparam>
        /// <param name="maybe">The Maybe to operate on.</param>
        /// <param name="func">Function to apply if Something; must return Maybe<TRight2>.</param>
        /// <returns>New Maybe with TRight2 if Something, original Nothing or Error otherwise.</returns>
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

        // a safe version of Bind that catches exceptions and returns Nothing<TOut>
        public static Maybe<TOut> BindSafe<TIn, TOut>(
            this Maybe<TIn> @this,
            Func<TIn, Maybe<TOut>> func)
        {
            try
            {
                var returnValue = @this switch
                {
                    Something<TIn> something => func(something.Value),
                    _ => new Nothing<TOut>()
                };
                return returnValue;
            }
            catch (Exception ex)
            {
                return new Nothing<TOut>();
            }
        }

        // a strict version of Bind
        public static Maybe<TOut> BindStrict<TIn, TOut>(
            this Maybe<TIn> @this,
            Func<TIn, TOut> func)
        {
            try
            {
                Maybe<TOut> updatedValue = @this switch
                {
                    // apply func if Something has a non-default value
                    Something<TIn> s when !EqualityComparer<TIn>.Default.Equals(s.Value, default) =>
                        new Something<TOut>(func(s.Value)),

                    // apply func if TIn is a primitive type (int, bool..)
                    Something<TIn> s when s.GetType().GetGenericArguments()[0].IsPrimitive =>
                        new Something<TOut>(func(s.Value)),

                    Something<TIn> _ => new UnhandledNothing<TOut>(),

                    UnhandledNothing<TIn> _ => new UnhandledNothing<TOut>(),

                    UnhandledError<TIn> e => new UnhandledError<TOut>(e.CapturedError),

                    Error<TIn> e => new Error<TOut>(e.CapturedError),

                    _ => new Error<TOut>(new Exception("New Maybe state that isn't coded for!: " + @this.GetType()))
                };

                return updatedValue;
            }
            catch (Exception ex)
            {
                return new UnhandledError<TOut>(ex);
            }
        }

        // a strict version of Bind for nested Maybes
        public static Maybe<TOut> BindStrict<TIn, TOut>(
            this Maybe<Maybe<TIn>> @this,
            Func<TIn, TOut> func)
        {
            try
            {
                return @this switch
                {
                    Something<Maybe<TIn>> s => s.Value.BindStrict(func),
                    Nothing<Maybe<TIn>> => new Nothing<TOut>(),
                    Error<Maybe<TIn>> e => new Error<TOut>(e.CapturedError),
                    _ => new Error<TOut>(new Exception("New Maybe state that isn't coded for!: " + @this.GetType()))
                };
            }
            catch (Exception e)
            {
                return new Error<TOut>(e);
            }
        }

        // an async strict version of Bind
        public static async Task<Maybe<TOut>> BindStrictAsync<TIn, TOut>(
            this Maybe<TIn> @this,
            Func<TIn, Task<TOut>> func)
        {
            try
            {
                Maybe<TOut> updatedValue = @this switch
                {
                    // apply func if Something has a non-default value
                    Something<TIn> s when !EqualityComparer<TIn>.Default.Equals(s.Value, default) =>
                        new Something<TOut>(await func(s.Value)),

                    // apply func if TIn is a primitive type (int, bool..)
                    Something<TIn> s when s.GetType().GetGenericArguments()[0].IsPrimitive =>
                        new Something<TOut>(await func(s.Value)),

                    Something<TIn> _ => new UnhandledNothing<TOut>(),

                    UnhandledNothing<TIn> _ => new UnhandledNothing<TOut>(),

                    UnhandledError<TIn> e => new UnhandledError<TOut>(e.CapturedError),

                    Error<TIn> e => new Error<TOut>(e.CapturedError),

                    _ => new Error<TOut>(new Exception("New Maybe state that isn't coded for!: " + @this.GetType()))
                };

                return updatedValue;
            }
            catch (Exception ex)
            {
                return new UnhandledError<TOut>(ex);
            }
        }

        // an async strict version of Bind for nested Maybes
        public static async Task<Maybe<TOut>> BindStrictAsync<TIn, TOut>(
            this Maybe<Maybe<TIn>> @this,
            Func<TIn, Task<TOut>> func)
        {
            try
            {
                return @this switch
                {
                    Something<Maybe<TIn>> outer => await outer.Value.BindStrictAsync(func),
                    Nothing<Maybe<TIn>> =>  new Nothing<TOut>(),
                    Error<Maybe<TIn>> e => new Error<TOut>(e.CapturedError),
                    _ => new Error<TOut>(new Exception("Unknown Maybe state: " + @this.GetType()))
                };
            }
            catch (Exception e)
            {
                return new Error<TOut>(e);
            }
        }

        /// <summary>
        /// Map for Maybe: transforms Something value while keeping Nothing or Error unchanged.
        /// </summary>
        /// <typeparam name="TRight">Type of the value inside the input Maybe.</typeparam>
        /// <typeparam name="TRight2">Type of the value after mapping.</typeparam>
        /// <param name="maybe">The Maybe to map.</param>
        /// <param name="func">Function to transform the Something value.</param>
        /// <returns>New Maybe with transformed Something, original Nothing or Error otherwise.</returns>
        public static Maybe<TRight2> Map<TRight, TRight2>(
            this Maybe<TRight> maybe,
            Func<TRight, TRight2> func)
            => maybe.Bind(r => new Something<TRight2>(func(r)));

        #region Maybe logging helpers
        public static Maybe<T> OnSomething<T>(this Maybe<T> @this, Action<T> action)
        {
            if (@this is Something<T> something)
            {
                action(something.Value);
            }
            return @this;
        }

        public static Maybe<T> OnNothing<T>(this Maybe<T> @this, Action action)
        {
            //if (@this is Nothing<T>)
            //{
            //    action();
            //}
            if (@this is UnhandledNothing<T>)
            {
                action();
            }
            return @this;
        }

        public static Maybe<T> OnError<T>(this Maybe<T> @this, Action<Exception> action)
        {
            //if (@this is Error<T> error)
            //{
            //    action(error.CapturedError);
            //}
            if (@this is UnhandledError<T> unhandledError)
            {
                action(unhandledError.CapturedError);
            }
            return @this;
        }


        #endregion

        public static State<TS, TV> ToState<TS, TV>(this TS @this, TV value) =>
            new(@this, value);

        public static State<TS, TV> Update<TS, TV>(
            this State<TS, TV> @this,
            Func<TS, TS> f
            ) => new (f(@this.CurrentState), @this.CurrentValue);

        public static State<TS, TVOut> Bind<TS, TVIn, TVOut>(
            this State<TS, TVIn> state,
            Func<TS, TVIn, TVOut> f
            ) => new State<TS, TVOut>(state.CurrentState, f(state.CurrentState, state.CurrentValue));
    }
}