namespace LeaveMeAloneFuncSkillForge.DiscriminatedUnions
{
    public abstract class Either<T1, T2>
    {
    }

    // Left is used to represent an error or failure case
    public sealed class Left<T1, T2> : Either<T1, T2>
    {
        public Left(T1 value)
        {
            this.Value = value;
        }
        public T1 Value { get; init; }
    }

    // Right is used to represent a success case
    public sealed class Right<T1, T2> : Either<T1, T2>
    {
        public Right(T2 value)
        {
            this.Value = value;
        }
        public T2 Value { get; init; }
    }
}
