namespace LeaveMeAloneCSharp.Utils.Adapters
{
    // This is a legacy file downloader that uses an event-based asynchronous pattern (EAP).
    public class LegacyFileDownloader
    {
        // Event that is raised when the download is completed
        public event EventHandler<DownloadCompletedEventArgs> DownloadCompleted;

        public void DownloadFileAsync(string url)
        {
            // Simulate a file download with a delay
            Task.Run(async () =>
            {
                try
                {
                    await Task.Delay(2000); // Simulate download time
                    string content = $"File downloaded from {url}"; // Simulated result

                    // notify subscribers that the download is completed
                    DownloadCompleted?.Invoke(this, new DownloadCompletedEventArgs(content, null, false));
                }
                catch (Exception ex)
                {
                    // notify subscribers that an error occurred during the download
                    DownloadCompleted?.Invoke(this, new DownloadCompletedEventArgs(null, ex, false));
                }
            });
        }
    }

    // custom EventArgs class to hold the result of the download, any error that occurred, and whether the operation was canceled
    public class DownloadCompletedEventArgs : EventArgs
    {
        public string Result { get; }
        public Exception Error { get; }
        public bool Canceled { get; }

        public DownloadCompletedEventArgs(string result, Exception error, bool canceled)
        {
            Result = result;
            Error = error;
            Canceled = canceled;
        }
    }
}