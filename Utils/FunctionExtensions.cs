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
    }
}
