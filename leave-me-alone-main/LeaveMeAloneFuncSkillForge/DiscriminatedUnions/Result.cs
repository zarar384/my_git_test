namespace LeaveMeAloneFuncSkillForge.DiscriminatedUnions
{
    public abstract class Result<T>
    {
    }

    public sealed class Success<T> : Result<T>
    {
        public Success(T value)
        {
            this.Value = value;
        }

        public T Value { get; init; }
    }

    public sealed class Failure<T> : Result<T>
    {
        public Failure(Exception e)
        {
            this.Error = e;
        }

        public Exception Error { get; init; }
    }
}
