namespace LeaveMeAloneFuncSkillForge.Utils
{
    public static class FunctionExtensions
    {
        public static TResult Pipe<TSource, TResult>(this TSource input, Func<TSource, TResult> func) =>
            func(input);

        public static Func<TInput, TOutput> AndThen<TInput, TMiddle, TOutput>(
            this Func<TInput, TMiddle> f,
            Func<TMiddle, TOutput> g) =>
            x => g(f(x));

        public static IEnumerable<(int x, int y)> GenerateGridCoord(int width, int height, bool reverse = false) =>
            (reverse
                ? Enumerable.Range(0, width).Select(i => width - i)         // [width, ..., 1]
                : Enumerable.Range(1, width))                               // [1, ..., width]
            .SelectMany(x =>
                (reverse
                    ? Enumerable.Range(0, height).Select(i => height - i)   // [height, ..., 1]
                    : Enumerable.Range(1, height))                          // [1, ..., height]
                .Select(y => (X: x, Y: y))
            );

        public static T Map<T>(this T @this, params Func<T, T>[] transforamtions) =>
            // apply each functions to the current value in sequence
            transforamtions.Aggregate(@this, (current, transform) => transform(current)); 

        public static TOut Map<TIn, TOut>(this TIn @this, Func<TIn, TOut> transformation) =>
            transformation(@this);

        public static TOut Fork<TIn, T1, T2, TOut>(
            this TIn @this,
            Func<TIn, T1> f1,
            Func<TIn, T2> f2,
            Func<T1, T2, TOut> join) =>
            join(f1(@this), f2(@this));

        public static TOut Alt<TIn, TOut>(
            this TIn @this,
            params Func<TIn, TOut>[] funcs) =>
            funcs.Select(f => f(@this))
                 .FirstOrDefault(x => x != null);

        /// <summary>
        /// Executes the action only if the condition is false; useful for null checks or guarding side effects.
        /// </summary>
        public static void Unless<T>(
            this T @this, 
            Func<T, bool> condition, 
            Action<T> action)
        {
            if (!condition(@this))
            {
                action(@this);
            }
        }
    }
}
