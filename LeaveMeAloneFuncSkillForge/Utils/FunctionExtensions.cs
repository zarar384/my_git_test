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
    }
}
