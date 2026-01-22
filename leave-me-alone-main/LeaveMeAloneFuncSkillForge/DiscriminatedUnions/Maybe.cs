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

    public class Nothing<T> : Maybe<T>
    {

    }

    // v 2.0 of Maybe DU
    public class Error<T> : Maybe<T>
    {
        public Error(Exception e)
        {
            this.CapturedError = e;
        }

        public Exception CapturedError { get; init; }
    }

    // logging helpers for v2.0 Maybe DU
    public sealed class  UnhandledNothing<T>: Nothing<T> 
    {
    }

    public sealed class UnhandledError<T> : Error<T>
    {
        public UnhandledError(Exception e) : base(e)
        {
        }
    }
}
