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
}
