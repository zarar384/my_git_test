using LeaveMeAloneCSharp.DiscriminatedUnions;

namespace LeaveMeAloneCSharp.Functional.Monads
{
    /// <summary>
    /// Non-generic static class to provide convenient factory methods for the Try monad.
    /// The actual Try monad is represented by the generic class <see cref="Try{T}"/>.
    /// </summary>
    public static class Try
    {
        /// <summary>
        /// Creates a successful Try containing a value.
        /// </summary>
        /// <typeparam name="T">Type of the value.</typeparam>
        /// <param name="value">The successful value.</param>
        /// <returns>A successful <see cref="Try{T}"/> wrapping <paramref name="value"/>.</returns>
        public static Try<T> FromValue<T>(T value)
            => Try<T>.FromValue(value);

        /// <summary>
        /// Creates a failed Try containing an exception.
        /// </summary>
        /// <typeparam name="T">Type of the value.</typeparam>
        /// <param name="ex">The exception to capture.</param>
        /// <returns>A failed <see cref="Try{T}"/> wrapping <paramref name="ex"/>.</returns>
        public static Try<T> FromException<T>(Exception ex)
            => Try<T>.FromException(ex);

        /// <summary>
        /// Executes a function and captures its result or any exception thrown.
        /// </summary>
        /// <typeparam name="T">Type of the value.</typeparam>
        /// <param name="func">The function to execute.</param>
        /// <returns>
        /// A successful <see cref="Try{T}"/> if <paramref name="func"/> completes normally;
        /// a failed <see cref="Try{T}"/> if it throws.
        /// </returns>
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

        /// <summary>
        /// Converts a <see cref="Result{T}"/> to a <see cref="Try{T}"/>.
        /// </summary>
        /// <typeparam name="T">Type of the value.</typeparam>
        /// <param name="result">The result to convert.</param>
        /// <returns>A <see cref="Try{T}"/> representing <paramref name="result"/>.</returns>
        public static Try<T> FromResult<T>(Result<T> result)
            => Try<T>.FromResult(result);

        /// <summary>
        /// Converts a <see cref="Try{T}"/> to a <see cref="Result{T}"/>.
        /// </summary>
        /// <typeparam name="T">Type of the value.</typeparam>
        /// <param name="tryValue">The Try to convert.</param>
        /// <returns>A <see cref="Result{T}"/> representing <paramref name="tryValue"/>.</returns>
        public static Result<T> ToResult<T>(Try<T> tryValue)
            => tryValue.ToResult();
    }

    /// <summary>
    /// Represents the result of an operation that can either succeed with a value
    /// or fail with an exception.
    /// </summary>
    /// <typeparam name="T">Type of the successful value.</typeparam>
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
        /// The successful value.
        /// Only meaningful when <see cref="IsSuccess"/> is <c>true</c>.
        /// </summary>
        public T? Value { get; }

        /// <summary>
        /// The captured exception.
        /// Only meaningful when <see cref="IsFailure"/> is <c>true</c>.
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

        // Note: implicit operator is intentionally defined only for T, not for Exception.
        // If T itself were an Exception subtype, an implicit operator for Exception
        // would create an ambiguity — the compiler would produce a Failure instead of
        // a Success, which is surprising and hard to debug. Use FromException explicitly.

        /// <summary>
        /// Implicitly converts a value to a successful <see cref="Try{T}"/>.
        /// </summary>
        /// <param name="value">The value to wrap.</param>
        public static implicit operator Try<T>(T value) => FromValue(value);

        /// <summary>
        /// Creates a successful Try containing a value.
        /// </summary>
        /// <param name="value">The successful value.</param>
        /// <returns>A successful <see cref="Try{T}"/> wrapping <paramref name="value"/>.</returns>
        public static Try<T> FromValue(T value)
            => new(value);

        /// <summary>
        /// Creates a failed Try containing an exception.
        /// </summary>
        /// <param name="exception">The exception to capture.</param>
        /// <returns>A failed <see cref="Try{T}"/> wrapping <paramref name="exception"/>.</returns>
        public static Try<T> FromException(Exception exception)
            => new(exception);

        /// <summary>
        /// Converts a <see cref="Result{T}"/> to a <see cref="Try{T}"/>.
        /// </summary>
        /// <param name="result">The result to convert.</param>
        /// <returns>A <see cref="Try{T}"/> representing <paramref name="result"/>.</returns>
        /// <exception cref="InvalidOperationException">
        /// Thrown if <paramref name="result"/> is an unknown <see cref="Result{T}"/> subtype.
        /// </exception>
        public static Try<T> FromResult(Result<T> result)
            => result switch
            {
                Success<T> s => FromValue(s.Value),
                Failure<T> f => FromException(f.Error),
                _ => throw new InvalidOperationException($"Unknown Result type: {result.GetType()}")
            };

        /// <summary>
        /// Converts this <see cref="Try{T}"/> to a <see cref="Result{T}"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="Success{T}"/> if this Try is successful;
        /// a <see cref="Failure{T}"/> otherwise.
        /// </returns>
        public Result<T> ToResult()
            => IsSuccess
                ? new Success<T>(Value!)
                : new Failure<T>(Exception!);

        /// <summary>
        /// Transforms the successful value using <paramref name="mapper"/>.
        /// If this Try is failed, the error is propagated unchanged.
        /// Any exception thrown by <paramref name="mapper"/> is captured.
        /// </summary>
        /// <typeparam name="TResult">Type of the transformed value.</typeparam>
        /// <param name="mapper">A function to transform the successful value.</param>
        /// <returns>A <see cref="Try{TResult}"/> containing the transformed value or a captured error.</returns>
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
        /// Asynchronously transforms the successful value using <paramref name="mapper"/>.
        /// If this Try is failed, the error is propagated unchanged.
        /// Any exception thrown by <paramref name="mapper"/> is captured.
        /// </summary>
        /// <typeparam name="TResult">Type of the transformed value.</typeparam>
        /// <param name="mapper">An async function to transform the successful value.</param>
        /// <returns>
        /// A <see cref="Task{T}"/> resolving to a <see cref="Try{TResult}"/>
        /// containing the transformed value or a captured error.
        /// </returns>
        public async Task<Try<TResult>> MapAsync<TResult>(Func<T, Task<TResult>> mapper)
        {
            if (IsFailure)
                return Try<TResult>.FromException(Exception!);

            try
            {
                return Try<TResult>.FromValue(await mapper(Value!));
            }
            catch (Exception ex)
            {
                return Try<TResult>.FromException(ex);
            }
        }

        /// <summary>
        /// Chains another Try-producing operation, preventing nested <c>Try&lt;Try&lt;T&gt;&gt;</c>.
        /// If this Try is failed, the error is propagated unchanged.
        /// Any exception thrown by <paramref name="binder"/> is captured.
        /// </summary>
        /// <typeparam name="TResult">Type of the result value.</typeparam>
        /// <param name="binder">A function that takes the current value and returns a <see cref="Try{TResult}"/>.</param>
        /// <returns>The <see cref="Try{TResult}"/> returned by <paramref name="binder"/>, or a propagated failure.</returns>
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
        /// Chains a Result-producing operation, converting the returned <see cref="Result{TResult}"/>
        /// to <see cref="Try{TResult}"/> automatically.
        /// If this Try is failed, the error is propagated unchanged.
        /// Any exception thrown by <paramref name="binder"/> is captured.
        /// </summary>
        /// <typeparam name="TResult">Type of the result value.</typeparam>
        /// <param name="binder">A function that takes the current value and returns a <see cref="Result{TResult}"/>.</param>
        /// <returns>A <see cref="Try{TResult}"/> derived from the returned <see cref="Result{TResult}"/>, or a propagated failure.</returns>
        public Try<TResult> Bind<TResult>(Func<T, Result<TResult>> binder)
        {
            if (IsFailure)
                return Try<TResult>.FromException(Exception!);

            try
            {
                return Try<TResult>.FromResult(binder(Value!));
            }
            catch (Exception ex)
            {
                return Try<TResult>.FromException(ex);
            }
        }

        /// <summary>
        /// Returns the successful value, or <paramref name="fallback"/> if this Try is failed.
        /// </summary>
        /// <param name="fallback">The value to return on failure.</param>
        /// <returns>The successful value or <paramref name="fallback"/>.</returns>
        public T GetOrElse(T fallback)
            => IsSuccess ? Value! : fallback;

        /// <summary>
        /// Executes one of two actions depending on the state.
        /// </summary>
        /// <param name="onSuccess">Action to execute if this Try is successful.</param>
        /// <param name="onFailure">Action to execute if this Try is failed.</param>
        public void Match(
            Action<T> onSuccess,
            Action<Exception> onFailure)
        {
            if (IsSuccess)
                onSuccess(Value!);
            else
                onFailure(Exception!);
        }

        /// <summary>
        /// Executes one of two functions depending on the state and returns the result.
        /// </summary>
        /// <typeparam name="TResult">Type of the returned value.</typeparam>
        /// <param name="onSuccess">Function to execute if this Try is successful.</param>
        /// <param name="onFailure">Function to execute if this Try is failed.</param>
        /// <returns>The result of whichever function was executed.</returns>
        public TResult Match<TResult>(
            Func<T, TResult> onSuccess,
            Func<Exception, TResult> onFailure)
            => IsSuccess ? onSuccess(Value!) : onFailure(Exception!);
    }
}