namespace LeaveMeAloneCSharp.Utils.Adapters
{
    // Adapter: Converts the custom callback-based async pattern of LegacyHttpService
    // into a Task-based asynchronous pattern (TAP).
    // This is a bit more complex than EAP/APM because the original API doesn't follow a standard pattern,
    // and we need to manually bridge the callback to a Task.
    // THE MAIN RULE: NEVER leave TaskCompletionSource unfinished!
    public static class LegacyHttpServiceExtensions
    {
        public static Task<string> DownloadStringAsync(this LegacyHttpService service, string url)
        {
            var tsc = new TaskCompletionSource<string>();

            try
            {
                service.DownloadString(url, (result, exception) =>
                {
                    if (exception != null)
                    {
                        tsc.SetException(exception);
                    }

                    // even if the API is designed to never return a null result, it's necesserary complete the Task
                    else if (result == null)
                    {
                        tsc.SetException(new InvalidOperationException("DownloadString callback returned null result without an exception."));
                    }
                    else
                    {
                        tsc.SetResult(result);
                    }
                });

            }
            catch (Exception ex)
            {
                // if the method throws synchronously, set the exception on the TaskCompletionSource
                tsc.TrySetException(ex);
            }

            return tsc.Task;
        }
    }
}
