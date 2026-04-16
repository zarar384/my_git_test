using LeaveMeAloneCSharp.Utils.Adapters;

namespace LeaveMeAloneCSharp.Utils
{
    // Adapter: converts the event-based asynchronous pattern (EAP) of LegacyFileDownloader
    // into a Task-based asynchronous pattern (TAP).
    public static class LegacyFileDownloaderExtensions
    {
        public static Task<string> DownloadFileTaskAsync(
            this LegacyFileDownloader client, string url)
        {
            // TaskCompletionSource is the bridge between event-based and Task-based async
            var tcs = new TaskCompletionSource<string>();

            EventHandler<DownloadCompletedEventArgs> handler = null;

            handler = (s, e) =>
            {
                if (e.Error != null)
                {
                    tcs.TrySetException(e.Error);
                }
                else if (e.Canceled)
                {
                    tcs.TrySetCanceled();
                }
                else
                {
                    tcs.TrySetResult(e.Result);
                }

                // IMPORTANT: Unsubscribe from the event to prevent memory leaks
                client.DownloadCompleted -= handler;
            };

            // Subscribe to the event before starting the async operation
            client.DownloadCompleted += handler;

            try
            {
                client.DownloadFileAsync(url);
            }
            catch (Exception ex)
            {
                // If the start method throws, propagate it into the Task
                tcs.TrySetException(ex);

                // Unsubscribe from the event in case of an immediate exception
                client.DownloadCompleted -= handler;
            }

            return tcs.Task;
        }
    }
}
