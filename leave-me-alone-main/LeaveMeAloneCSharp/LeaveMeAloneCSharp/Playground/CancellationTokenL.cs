using System.Collections.Concurrent;
using System.Net.NetworkInformation;

namespace LeaveMeAloneCSharp.Playground
{
    public static class CancellationTokenL
    {
        public static async Task Run()
        {
            await DemoBackgroundWorkerAsync();
        }

        // Basic cancellation
        private static async Task BasicCancellationAsync()
        {
            // Creates a source that can cancel the operation.
            using var cts = new CancellationTokenSource();

            Task task = SimulateWorkAsync(cts.Token);

            await Task.Delay(2000);

            // Requests cancellation.
            cts.Cancel();

            try
            {
                await task;
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("Operation was cancelled.");
            }
        }

        // CancellationTokenSource
        private static void ManualCancellationSource()
        {
            // Creates a source and cancels it manually.
            using var cts = new CancellationTokenSource();

            CancellationToken token = cts.Token;

            cts.Cancel();

            Console.WriteLine(token.IsCancellationRequested);
        }

        // Async cancellation
        private static async Task AsyncCancellationAsync(CancellationToken cancellationToken)
        {
            // Passes token to async APIs.
            await Task.Delay(TimeSpan.FromSeconds(5), cancellationToken);
        }

        private static async Task<int> GetValueAsync(CancellationToken cancellationToken)
        {
            // Async methods should propagate the token.
            await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);

            return 42;
        }

        // Timeout cancellation
        private static async Task TimeoutCancellationAsync()
        {
            // Automatically cancels after timeout.
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(3));

            try
            {
                await Task.Delay(TimeSpan.FromSeconds(10), cts.Token);
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("Timeout reached.");
            }
        }
        private static async Task CancelAfterAsync()
        {
            // Starts a cancellation timer.
            using var cts = new CancellationTokenSource();

            cts.CancelAfter(TimeSpan.FromSeconds(3));

            try
            {
                await Task.Delay(TimeSpan.FromSeconds(10), cts.Token);
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("Operation timed out.");
            }
        }

        private static async Task DemoPropagationAsync()
        {
            // Creates a token that will be passed through all layers.
            using var cts = new CancellationTokenSource();

            cts.CancelAfter(TimeSpan.FromSeconds(1));

            try
            {
                await PropagateCancellationAsync(cts.Token);
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("Propagation cancelled.");
            }
        }

        // Cancellation propagation
        private static async Task PropagateCancellationAsync(CancellationToken cancellationToken)
        {
            // Uses the same token through the entire call chain.
            await LoadDataAsync(cancellationToken);

            await ProcessDataAsync(cancellationToken);

            await SaveDataAsync(cancellationToken);
        }

        private static async Task DemoLinkedCancellationAsync()
        {
            // Simulates a user cancellation token.
            using var userCts = new CancellationTokenSource();

            try
            {
                await LinkedCancellationAsync(userCts.Token);
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("Linked cancellation detected.");
            }
        }

        // Linked tokens
        private static async Task LinkedCancellationAsync(CancellationToken userCancellationToken)
        {
            // Combines user cancellation with internal timeout.
            using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(userCancellationToken);

            linkedCts.CancelAfter(TimeSpan.FromSeconds(5));

            try
            {
                await Task.Delay(TimeSpan.FromSeconds(30), linkedCts.Token);
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("Linked token cancelled.");
            }
        }

        private static void DemoCpuBoundCancellation()
        {
            // Cancels a CPU intensive operation.
            using var cts = new CancellationTokenSource();

            cts.CancelAfter(TimeSpan.FromMilliseconds(50));

            try
            {
                CpuBoundCancellation(cts.Token);
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("CPU work cancelled.");
            }
        }

        // CPU-bound cancellation
        private static void CpuBoundCancellation(CancellationToken cancellationToken)
        {
            // CPU loops must periodically check cancellation.
            for (int i = 0; i < 1_000_000; i++)
            {
                if (i % 1000 == 0)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                }

                _ = Math.Sqrt(i);
            }
        }

        private static void DemoParallelForCancellation()
        {
            using var cts = new CancellationTokenSource();

            cts.CancelAfter(TimeSpan.FromSeconds(1));

            try
            {
                ParallelForCancellation(Enumerable.Range(1, 1000), cts.Token);
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("Parallel loop cancelled.");
            }
        }

        // Parallel cancellation
        private static void ParallelForCancellation(
            IEnumerable<int> values,
            CancellationToken cancellationToken)
        {
            // Lets Parallel API stop all workers.
            Parallel.ForEach(
                values,
                new ParallelOptions
                {
                    CancellationToken = cancellationToken
                },
                value =>
                {
                    Thread.Sleep(100);

                    Console.WriteLine(value);
                });
        }

        private static void DemoPlinqCancellation()
        {
            using var cts = new CancellationTokenSource();

            cts.CancelAfter(TimeSpan.FromMilliseconds(500));

            try
            {
                var result = PlinqCancellation(Enumerable.Range(1, 100_000_000), cts.Token);

                foreach (int item in result)
                {
                    // Forces query execution.
                    Thread.Sleep(1);
                }
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("PLINQ query cancelled.");
            }
        }

        // PLINQ cancellation
        private static IEnumerable<int> PlinqCancellation(
            IEnumerable<int> values,
            CancellationToken cancellationToken)
        {
            // Demonstrates cancellation inside expensive projections.
            return values
                .AsParallel()
                .WithCancellation(cancellationToken)
                .Select(x =>
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    Thread.Sleep(10);

                    return x * 2;
                });
        }

        // HTTP cancellation
        private static async Task HttpClientCancellationAsync(
            HttpClient httpClient,
            CancellationToken cancellationToken)
        {
            // Stops the HTTP request when cancellation is requested.
            HttpResponseMessage response = await httpClient.GetAsync("https://google.com", cancellationToken);

            Console.WriteLine(response.StatusCode);
        }

        private static async Task DemoCallbackRegistrationAsync()
        {
            using var cts = new CancellationTokenSource();

            Task task = CallbackRegistrationAsync(cts.Token);

            await Task.Delay(1000);

            cts.Cancel();

            await task;
        }

        // Callback registration
        private static async Task CallbackRegistrationAsync(CancellationToken cancellationToken)
        {
            // Executes custom logic when cancellation occurs.
            using CancellationTokenRegistration registration =
                cancellationToken.Register(() =>
                {
                    Console.WriteLine("Cancellation callback executed.");
                });

            await Task.Delay(5000);
        }

        private static async Task DemoLegacyApiCancellationAsync()
        {
            using var cts = new CancellationTokenSource();

            cts.CancelAfter(TimeSpan.FromMilliseconds(100));

            try
            {
                await LegacyApiCancellationAsync("google.com", cts.Token);
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("Legacy API cancelled.");
            }
        }

        // Legacy API integration
        private static async Task LegacyApiCancellationAsync(
            string host,
            CancellationToken cancellationToken)
        {
            // Bridges CancellationToken to a legacy cancel mechanism.
            using var ping = new Ping();

            Task<PingReply> pingTask = ping.SendPingAsync(host);

            using CancellationTokenRegistration registration =
                cancellationToken.Register(() =>
                {
                    ping.SendAsyncCancel();
                });

            await pingTask;
        }

        private static async Task DemoBackgroundWorkerAsync()
        {
            using var cts = new CancellationTokenSource();

            Task worker = BackgroundWorkerAsync(cts.Token);

            await Task.Delay(3000);

            cts.Cancel();

            try
            {
                await worker;
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("Background worker stopped.");
            }
        }

        // Background service cancellation
        private static async Task BackgroundWorkerAsync(CancellationToken stoppingToken)
        {
            // Runs until the application requests shutdown.
            while (!stoppingToken.IsCancellationRequested)
            {
                Console.WriteLine("Background work.");

                await Task.Delay(TimeSpan.FromSeconds(1), stoppingToken);
            }
        }

        private static async Task DemoProducerConsumerAsync()
        {
            using var cts = new CancellationTokenSource();

            cts.CancelAfter(TimeSpan.FromSeconds(1));

            try
            {
                await ProducerConsumerAsync(cts.Token);
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("Producer-consumer cancelled.");
            }
        }

        // Producer consumer cancellation
        private static async Task ProducerConsumerAsync(CancellationToken cancellationToken)
        {
            // Cancels producer and consumer with a single token.
            var queue = new BlockingCollection<int>();

            Task producer = Task.Run(() =>
            {
                for (int i = 0; i < 100; i++)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    queue.Add(i, cancellationToken);
                }

                queue.CompleteAdding();
            }, cancellationToken);

            Task consumer = Task.Run(() =>
            {
                foreach (int item in queue.GetConsumingEnumerable(cancellationToken))
                {
                    Console.WriteLine(item);
                }
            }, cancellationToken);

            await Task.WhenAll(producer, consumer);
        }

        // Race conditions
        private static async Task RaceConditionExampleAsync()
        {
            // Cancellation is always a race between completion and cancel.
            using var cts = new CancellationTokenSource();

            Task task = Task.Run(async () =>
            {
                await Task.Delay(1000);
            });

            cts.Cancel();

            try
            {
                await task;
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("Cancelled.");
            }
        }

        // CancellationToken.None
        private static async Task CancellationTokenNoneAsync()
        {
            // Uses a token that can never be cancelled.
            await SimulateWorkAsync(CancellationToken.None);
        }

        // Optional token parameter
        private static async Task OptionalTokenAsync(CancellationToken cancellationToken = default)
        {
            // Makes cancellation optional for callers.
            await Task.Delay(
                TimeSpan.FromSeconds(1),
                cancellationToken);
        }

        // Correct cancellation pattern
        private static void CorrectCancellationPattern(CancellationToken cancellationToken)
        {
            // Throws the standard exception expected by .NET.
            cancellationToken.ThrowIfCancellationRequested();
        }

        // Wrong cancellation pattern
        private static bool WrongCancellationPattern(CancellationToken cancellationToken)
        {
            // Hides cancellation from the caller.
            if (cancellationToken.IsCancellationRequested)
            {
                return false;
            }

            return true;
        }

        // Helper methods
        private static async Task SimulateWorkAsync(CancellationToken cancellationToken)
        {
            // Simulates a long-running operation.
            for (int i = 0; i < 10; i++)
            {
                cancellationToken.ThrowIfCancellationRequested();

                await Task.Delay(TimeSpan.FromMilliseconds(500), cancellationToken);

                Console.WriteLine($"Step {i}");
            }
        }

        private static async Task LoadDataAsync(CancellationToken cancellationToken)
        {
            // Simulates data loading.
            await Task.Delay(500, cancellationToken);
        }

        private static async Task ProcessDataAsync(CancellationToken cancellationToken)
        {
            // Simulates data processing.
            await Task.Delay(500, cancellationToken);
        }

        private static async Task SaveDataAsync(CancellationToken cancellationToken)
        {
            // Simulates data saving.
            await Task.Delay(500, cancellationToken);
        }
    }
}
