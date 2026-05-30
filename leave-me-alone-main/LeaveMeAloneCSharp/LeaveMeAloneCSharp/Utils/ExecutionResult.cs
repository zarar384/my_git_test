namespace LeaveMeAloneCSharp.Utils
{
    public class ExecutionResult<T>
    {
        public T Result { get; init; }
        public Exception Error { get; init; }
    }
}
