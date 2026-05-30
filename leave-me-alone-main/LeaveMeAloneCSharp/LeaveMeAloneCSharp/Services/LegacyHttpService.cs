using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeaveMeAloneCSharp.Services
{
    // This simulates a custom async API (callback-based, not standard EAP/APM)
    public class LegacyHttpService
    {
        public void DownloadString(string url, Action<string, Exception?> callback)
        {
            Task.Run(() => {
                try
                {
                    // Simulate network delay
                    Task.Delay(1000).Wait();

                    // Simulate successful download
                    string result = $"Content from {url}";

                    // Success
                    callback(result, null);
                }
                catch (Exception ex)
                {
                    // Error
                    callback(null!, ex);
                }
            });
        }
    }
}
