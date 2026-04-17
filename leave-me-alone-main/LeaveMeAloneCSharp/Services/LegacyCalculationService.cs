namespace LeaveMeAloneCSharp.Services
{
    // This is a legacy calculation service that simulates an old APM-style (Asynchronous Programming Model) API. (Begin/End pattern)
    public class LegacyCalculationService
    {
        public IAsyncResult BeginCalculateSquare(int number, AsyncCallback? callback, object? state)
        {
            var task = Task.Run(() =>
            {
               // Simulate a long-running calculation
                Thread.Sleep(2000);
                return number * number;
            });

            // if callback is provided, invoke it when the task completes
            if (callback != null)
            {
                task.ContinueWith(t => callback(t));
            }

            return task;
        }

        public int EndCalculateSquare(IAsyncResult asyncResult)
        {
            var task = (Task<int>)asyncResult;
            return task.GetAwaiter().GetResult(); // This will throw if the task failed
        }
    }
}
