namespace LeaveMeAloneFuncSkillForge.Utils
{
    public static class TryCatchExtensions
    {
        /// <summary>
        /// Executes func with try-catch and returns ExecutionResult
        /// </summary>
        public static ExecutionResult<TOut> MapWithTryCatch<TIn, TOut>
            (this TIn input,
            Func<TIn, TOut> func)
        {
            try
            {
                return new ExecutionResult<TOut> { Result = func(input) };
            }
            catch (Exception ex)
            {
                return new ExecutionResult<TOut> { Error = ex };
            }
        }

        public static T OnError<T>
            (this ExecutionResult<T> execResult, 
            Action<Exception> action)
        {
            if (execResult.Error != null)
            {
                action(execResult.Error);
                return default;
            }
            
            return execResult.Result;
        }
    }
}
