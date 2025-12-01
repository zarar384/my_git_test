namespace LeaveMeAloneFuncSkillForge.DiscriminatedUnions
{
    public abstract class Maybe<T>
    {
    }

    public sealed class  Something<T>: Maybe<T>
    {
        public Something(T value)
        {
            this.Value = value;
        }

        public T Value { get; init; }
    }

    public sealed class Nothing<T> : Maybe<T>
    {

    }

    // v 2.0 of Maybe DU
    public sealed class Error<T> : Maybe<T>
    {
        public Error(Exception e)
        {
            this.CapturedError = e;
        }

        public Exception CapturedError { get; init; }
    }
}
