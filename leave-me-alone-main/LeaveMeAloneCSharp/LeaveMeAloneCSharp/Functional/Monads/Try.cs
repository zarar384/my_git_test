namespace LeaveMeAloneCSharp.Functional.Monads
{
    /// <summary>
    /// Non-generic static class to provide convenient factory methods for Try monad.
    /// The actual Try monad is represented by the generic class <see cref="Try{T}"/>.
    /// </summary>
    public static class Try
    {
        /// <summary>
        /// Creates a successful Try containing a value.
        /// </summary>
        public static Try<T> FromValue<T>(T value)
            => Try<T>.FromValue(value);
        
        /// <summary>
        /// Creates a failed Try containing an exception.
        /// </summary>
        public static Try<T> FromException<T>(Exception ex)
            => Try<T>.FromException(ex);

        /// <summary>
        /// Executes a function and captures its result or any exception thrown.
        /// </summary>
        public static Try<T> Of<T>(Func<T> func)
        {
            try
            {
                return Try<T>.FromValue(func());
            }
            catch (Exception ex)
            {
                return Try<T>.FromException(ex);
            }
        }
    }

    /// <summary>
    /// Represents the result of an operation that can either succeed with a value
    /// or fail with an exception.
    /// </summary>
    /// <typeparam name="T">Type of successful value.</typeparam>
    public sealed class Try<T>
    {
        private Try(T value)
        {
            Value = value;
            Exception = null;
        }

        private Try(Exception exception)
        {
            Value = default;
            Exception = exception;
        }

        /// <summary>
        /// Successful value.
        /// Available only when <see cref="IsSuccess"/> is true.
        /// </summary>
        public T? Value { get; }

        /// <summary>
        /// Captured exception.
        /// Available only when <see cref="IsFailure"/> is true.
        /// </summary>
        public Exception? Exception { get; }

        /// <summary>
        /// Indicates whether the operation completed successfully.
        /// </summary>
        public bool IsSuccess => Exception is null;

        /// <summary>
        /// Indicates whether the operation failed.
        /// </summary>
        public bool IsFailure => Exception is not null;

        /// <summary>
        /// Creates a successful Try containing a value.
        /// </summary>
        public static Try<T> FromValue(T value)
            => new(value);

        /// <summary>
        /// Creates a failed Try containing an exception.
        /// </summary>
        public static Try<T> FromException(Exception exception)
            => new(exception);

        /// <summary>
        /// Transforms the successful value.
        /// If the current Try is failed, the error is propagated unchanged.
        /// Any exception thrown by the mapping function is captured.
        /// </summary>
        public Try<TResult> Map<TResult>(Func<T, TResult> mapper)
        {
            if (IsFailure)
                return Try<TResult>.FromException(Exception!);

            try
            {
                return Try<TResult>.FromValue(mapper(Value!));
            }
            catch (Exception ex)
            {
                return Try<TResult>.FromException(ex);
            }
        }

        /// <summary>
        /// Chains another Try-producing operation.
        /// Prevents nested Try&lt;Try&lt;T&gt;&gt; structures.
        /// </summary>
        public Try<TResult> Bind<TResult>(Func<T, Try<TResult>> binder)
        {
            if (IsFailure)
                return Try<TResult>.FromException(Exception!);

            try
            {
                return binder(Value!);
            }
            catch (Exception ex)
            {
                return Try<TResult>.FromException(ex);
            }
        }

        /// <summary>
        /// Returns the value or a fallback value if failed.
        /// </summary>
        public T GetOrElse(T fallback)
            => IsSuccess ? Value! : fallback;

        /// <summary>
        /// Executes one of two actions depending on the state.
        /// </summary>
        public void Match(
            Action<T> onSuccess,
            Action<Exception> onFailure)
        {
            if (IsSuccess)
                onSuccess(Value!);
            else
                onFailure(Exception!);
        }
    }
}