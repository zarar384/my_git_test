using LeaveMeAloneFuncSkillForge.API;
using LeaveMeAloneFuncSkillForge.Interfaces;
using System.Diagnostics;

namespace LeaveMeAloneFuncSkillForge.Playground
{
    public static class AsyncPL
    {
        public static async Task Run()
        {
            await TestExternalFilmServiceKeysetAsyncStreamDemoAsync();
        }

        private static async Task TestApiClientFactoryAsync()
        {
            // DI-friendly factory design
            // factory implements IApiClientFactory
            // and can easily be injected where needed, for example in ASP.NET Core services
            // (services.AddSingleton<IApiClientFactory, ApiClientFactory>();)
            IApiClientFactory factory = new ApiClientFactory();

            // create and cache clients 
            var client1 = await factory.CreateClientAsync("testClient_1");
            Console.WriteLine(client1 != null ? "Client 1 created" : "Null client");

            var client2 = await factory.CreateClientAsync("testClient_2");
            Console.WriteLine(client2 != null ? "Client 2 created" : "Null client");

            var results = await client1.GetData("/data/endpoint1");
            Console.WriteLine(results);

            // reuse cached client (no re-initialization)
            var client1Cached = await factory.CreateClientAsync("testClient_1");
            Console.WriteLine(ReferenceEquals(client1, client1Cached));
        }

        private static async Task TestMockApiClientFactoryAsync()
        {
            // DI-friendly factory design
            // factory implements IApiClientFactory
            // and can easily be injected where needed, for example in ASP.NET Core services
            // (services.AddSingleton<IApiClientFactory, ApiClientFactory>();)
            // mock factory for testing
            Func<string, Task<IApiClient>> factory = MockApiClientFactory.GetClientAsync;

            // create and cache clients
            var client1 = await factory("testClient_1");
            Console.WriteLine(client1 != null ? "Client 1 created" : "Null client");

            var client2 = await factory("testClient_2");
            Console.WriteLine(client2 != null ? "Client 2 created" : "Null client");

            var results = await client1.GetData("/data/endpoint1");
            Console.WriteLine(results); // Fake data from testClient_1 for /data/endpoint1

            // reuse cached client (if you implement caching in your mock factory)
            var client1Cached = await factory("testClient_1");
            Console.WriteLine(ReferenceEquals(client1, client1Cached));
        }

        private static async Task RunBasicAsyncExamples()
        {
            Console.WriteLine("Async basics");
            await BasicAsyncFlow();

            Console.WriteLine("\nException handling");
            try
            {
                await ExceptionFlowAsync();
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine($"Caught exception: {ex.Message}");
            }

            Console.WriteLine("\nConfigureAwait(false)");
            await ConfigureAwaitDemoAsync();

            Console.WriteLine("\nTaskCompletionSource (event-based async)");
            int eventResult = await EventBasedAsync();
            Console.WriteLine($"Event result: {eventResult}");

            Console.WriteLine("\nCPU-bound vs I/O-bound");
            await CpuBoundAsync();
            await IoBoundAsync();

            Console.WriteLine("\nI/O + CPU combined example");
            var result = await CalculateOrderPriceAsync();
            Console.WriteLine($"Final price: {result}");

            Console.WriteLine("\nAsync stream");
            await foreach (var value in GenerateNumbersAsync())
            {
                Console.WriteLine($"Stream value: {value}");
            }

            Console.WriteLine("\nDone");
        }

        // Basic async flow and freeing threads
        private static async Task BasicAsyncFlow()
        {
            Console.WriteLine($"Start thread: {Thread.CurrentThread.ManagedThreadId}");

            await Task.Delay(500);

            Console.WriteLine($"After await thread: {Thread.CurrentThread.ManagedThreadId}");
        }

        // Exception propagation through Task and await
        private static async Task ExceptionFlowAsync()
        {
            await Task.Delay(300);
            throw new InvalidOperationException("Something went wrong");
        }

        // ConfigureAwait(false)
        private static async Task ConfigureAwaitDemoAsync()
        {
            Console.WriteLine($"Before await: {Thread.CurrentThread.ManagedThreadId}");

            await Task.Delay(300).ConfigureAwait(false);

            Console.WriteLine($"After await (ConfigureAwait false): {Thread.CurrentThread.ManagedThreadId}");
        }

        // Event-style async using TaskCompletionSource
        private static Task<int> EventBasedAsync()
        {
            var tcs = new TaskCompletionSource<int>();

            _ = Task.Run(async () =>
            {
                await Task.Delay(400);
                tcs.SetResult(42);
            });

            return tcs.Task;
        }

        //  CPU-bound work (needs thread pool)
        private static async Task CpuBoundAsync()
        {
            Console.WriteLine("CPU-bound start");

            int result = await Task.Run(() =>
            {
                int sum = 0;
                for (int i = 0; i < 10_000_000; i++)
                    sum += i;
                return sum;
            });

            Console.WriteLine($"CPU-bound result: {result}");
        }

        // I/O-bound work (no extra thread)
        private static async Task IoBoundAsync()
        {
            Console.WriteLine("I/O-bound start");
            await Task.Delay(500);
            Console.WriteLine("I/O-bound finished");
        }

        // Async stream (IAsyncEnumerable)
        private static async IAsyncEnumerable<int> GenerateNumbersAsync()
        {
            for (int i = 1; i <= 5; i++)
            {
                await Task.Delay(200);
                yield return i;
            }
        }

        private static async Task<decimal> CalculateOrderPriceAsync()
        {
            // I/O-bound: call external services in parallel
            Task<Order> orderTask = GetOrderFromOrderServiceAsync();
            Task<Product> productTask = GetProductFromProductServiceAsync();

            await Task.WhenAll(orderTask, productTask);

            Order order = orderTask.Result;
            Product product = productTask.Result;

            // CPU-bound: heavy calculation
            decimal finalPrice = await Task.Run(() =>
            {
                return CalculateFinalPrice(order, product);
            });

            return finalPrice;
        }

        // Simulated microservice calls (I/O-bound)

        private static async Task<Order> GetOrderFromOrderServiceAsync()
        {
            Console.WriteLine($"Order service call started on thread {Thread.CurrentThread.ManagedThreadId}");
            await Task.Delay(500); // simulate HTTP / DB
            return new Order { Quantity = 3 };
        }

        private static async Task<Product> GetProductFromProductServiceAsync()
        {
            Console.WriteLine($"Product service call started on thread {Thread.CurrentThread.ManagedThreadId}");
            await Task.Delay(700); // simulate HTTP / DB
            return new Product { Price = 19.99m };
        }

        // CPU-bound calculation
        private static decimal CalculateFinalPrice(Order order, Product product)
        {
            Console.WriteLine($"Calculation started on thread {Thread.CurrentThread.ManagedThreadId}");

            decimal result = 0;
            for (int i = 0; i < 10_000_000; i++)
            {
                result += product.Price * order.Quantity * 0.0000001m;
            }

            return result;
        }

        private static async Task TestExternalFilmServiceGetAllAsync()
        {
            using var httpClient = new HttpClient(new FakeHttpMessageHandler())
            {
                BaseAddress = new Uri("https://filmDB-fake.api/")
            };
            IExternalFilmService filmService = new Services.ExternalFilmService(httpClient);
            var filmIds = new List<int> { 1, 2, 3, 4, 5 };
            var allFilmsHtml = await filmService.GetAllAsync(filmIds);

            Console.WriteLine(allFilmsHtml);
        }

        private static async Task TestExternalFilmServiceGetFirstRespondingAsync()
        {
            using var httpClient = new HttpClient(new FakeHttpMessageHandler())
            {
                BaseAddress = new Uri("https://filmDB-fake.api/")
            };
            IExternalFilmService filmService = new Services.ExternalFilmService(httpClient);

            var filmIdFromNewService = 12;
            var filmIdFromOldService = 1526237;

            var allFilmsHtml = await filmService.GetFirstRespondingAsync(filmIdFromNewService, filmIdFromOldService);

            Console.WriteLine(allFilmsHtml);
        }

        private static async Task TestExternalFilmServiceGetFirstSuccessfulResponseAsync()
        {
            using var httpClient = new HttpClient(new FakeHttpMessageHandler())
            {
                BaseAddress = new Uri("https://filmDB-fake.api/")
            };
            IExternalFilmService filmService = new Services.ExternalFilmService(httpClient);

            var filmIdFromNewService = 12;
            var filmIdFromOldService = 1526237;

            var allFilmsHtml = await filmService.GetFirstSuccessfulResponseAsync(filmIdFromNewService, filmIdFromOldService);

            Console.WriteLine(allFilmsHtml);
        }

        private static async Task TestExternalFilmServiceKeysetAsyncStreamDemoAsync()
        {
            Console.WriteLine("KEYSET ASYNC STREAM DEMO");
            Console.WriteLine();

            using var httpClient = new HttpClient(new FakeHttpMessageHandler())
            {
                BaseAddress = new Uri("https://filmDB-fake.api/")
            };

            IExternalFilmService filmService = new Services.ExternalFilmService(httpClient);

            // stream results using keyset pagination
            // this allows processing large datasets without loading everything in memory at once
            await foreach (var film in FunctionExtensions.StreamByKeysetAsync<Film, int>(
                filmService.GetFilmsPageAsync,
                pageSize: 10))
            {
                Console.WriteLine($"Film ID: {film.Id}, Title: {film.Title}");
                await Task.Delay(100); // simulate processing time
            }

            Console.WriteLine();
            Console.WriteLine("END KEYSET DEMO");
        }

        private static async Task TestAsyncExceptionHandling()
        {
            Console.WriteLine("ASYNC EXCEPTION HANDLING TEST");
            Console.WriteLine();

            using var httpClient = new HttpClient(new FakeHttpMessageHandler())
            {
                BaseAddress = new Uri("https://fake.api/")
            };

            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));

            IMyAsyncInterface service = new MySyncImplementation(new FeatureFlagService(httpClient));

            try
            {
                await service.GetPaymentMethodWithExceptionAsync(cts.Token);
                Console.WriteLine("[OK] Operation succeeded");
            }
            catch(InvalidOperationException ex)
            {
                Console.WriteLine($"Caught InvalidOperationException: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Caught general exception: {ex.Message}");
            }
        }

        private static async Task RunWithMySyncImplementation()
        {
            using var httpClient = new HttpClient(new FakeHttpMessageHandler())
            {
                BaseAddress = new Uri("https://fake.api/")
            };

            IFeatureFlagService featureFlagService = new FeatureFlagService(httpClient);
            IMyAsyncInterface service = new MySyncImplementation(featureFlagService);

            Console.WriteLine("* SIMPLE ASYNC CONTRACT (SYNC IMPLEMENTATION)");
            await service.DoSomethingAsync();
            Console.WriteLine("[OK] DoSomethingAsync completed");

            Console.WriteLine();
            Console.WriteLine("* RANDOM FAILURE TEST (50%)");

            try
            {
                await service.DoSomethingWithExceptionAsync();
                Console.WriteLine("[OK] Operation succeeded");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            Console.WriteLine();
            Console.WriteLine("* RETURN VALUE TEST");

            var value = await service.GetIntAsync();
            Console.WriteLine($"[VALUE] {value}");

            Console.WriteLine();
            Console.WriteLine("* NOT IMPLEMENTED TEST");

            try
            {
                await service.NotImplementedAsync<string>();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            Console.WriteLine();
            var delay = await DelayResult("[SLEEP] 1 sec...", TimeSpan.FromSeconds(1));
            Console.WriteLine(delay);

            Console.WriteLine();
            Console.WriteLine("* FEATURE FLAG DEPENDENT METHOD");

            var paymentMethod = await service.GetPaymentMethodAsync();
            Console.WriteLine($"[PAYMENT MEHOD] {paymentMethod}");
            Console.WriteLine();

            Console.WriteLine("* CALCULATE PRICE ASYNC");
            var price = await service.CalculatePriceAsync(new Transaction { Amount = 123.45m });
            Console.WriteLine($"[PRICE] {price:N2}");

            Console.WriteLine("* CALCULATE PRICE WITH PROGRESS ASYNC");
            await RunPriceCalculationAsync(service, new Transaction { Amount = 678.90m });

            Console.WriteLine();
            Console.WriteLine("[DONE]");
        }


        private static async Task RunWithHttpClient()
        {
            using var httpClient = new HttpClient(new FakeHttpMessageHandler())
            {
                BaseAddress = new Uri("https://fake.api/")
            };

            Console.WriteLine("* TIMEOUT TEST");
            var timeoutResult = await DownloadStringWithTimeout(httpClient, "users");

            Console.WriteLine(timeoutResult ?? "[TIMEOUT]");
            Console.WriteLine();

            // small delay between tests
            var secDelay = await DelayResult("[SLEEP] 2 sec...", TimeSpan.FromSeconds(2));
            Console.WriteLine(secDelay);

            Console.WriteLine();
            Console.WriteLine("* RETRY TEST");

            try
            {
                var retryResult = await DownloadStringWithRetries(httpClient, "orders");

                Console.WriteLine(retryResult);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Final failure: {ex.Message}");
            }
        }

        private static async Task<string> DownloadStringWithTimeout(HttpClient client, string uri)
        {
            // auto-cancel after 3 seconds
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(3));
            
            // setup
            Task<string> downloadTask = client.GetStringAsync(uri);
            Task timeoutTask = Task.Delay(Timeout.InfiniteTimeSpan, cts.Token);
            Task completedTask = await Task.WhenAny(downloadTask, timeoutTask);

            // check which task completed
            if (completedTask == timeoutTask)
                return null;

            return await downloadTask;
        }

        // Retry logic with exponential backoff. Recomends using Polly in real apps.
        private static async Task<string> DownloadStringWithRetries(HttpClient client, string uri)
        {
            // retry after 1 sec., then after 2 sec., then after 4 sec.
            TimeSpan nextDelay = TimeSpan.FromSeconds(1);
            for (int i = 0; i != 3; ++i)
            {
                try
                {
                    return await client.GetStringAsync(uri);
                }
                catch
                {
                }
                await Task.Delay(nextDelay);
                nextDelay = nextDelay + nextDelay;
            }
            // try one last time and let any exception propagate
            return await client.GetStringAsync(uri);
        }

        private static async Task<T> DelayResult<T>(T result, TimeSpan delay)
        {
            await Task.Delay(delay);
            return result;
        }

        private static async Task RunPriceCalculationAsync(
            IMyAsyncInterface service,
            Transaction transaction)
        {
            using var cts = new CancellationTokenSource();

            var progress = new Progress<double>(percent =>
            {
                Console.WriteLine($"Progress: {percent:P0}");
            });

            try
            {
                double price = await service.CalculatePriceWithProgressAsync(
                    transaction,
                    progress,
                    cts.Token);

                Console.WriteLine($"Final price: {price:N2}");
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("Price calculation canceled");
            }
        }

        // Starts all tasks concurrently and processes each result upon completion, using Task.Delay timers.
        private static async Task ProcessTasksAsync()
        {
            // only for demo purposes: output to console
            Trace.Listeners.Add(new ConsoleTraceListener());

            // create a task sequence
            Task<int> task1 = DelayAndReturnAsync(2);
            Task<int> task2 = DelayAndReturnAsync(3);
            Task<int> task3 = DelayAndReturnAsync(1);
            
            Task<int>[] tasks = { task1, task2, task3 };

            var tQ = from t in tasks
                     select AwaitAndProcessAsync(t);

            Task[] processingAllTasks = tQ.ToArray();

            // or using LINQ method syntax:
            //Task[] p = new Task<int>[]
            //{
            //    DelayAndReturnAsync(2),
            //    DelayAndReturnAsync(3),
            //    DelayAndReturnAsync(1)
            //}
            //.Select(async t =>
            //{
            //    var result = await t;
            //    Trace.WriteLine(result);
            //})
            //.ToArray();

            // wait for all processing to complete
            await Task.WhenAll(processingAllTasks);
        }

        private static async Task<int> DelayAndReturnAsync(int value)
        {
            await Task.Delay(TimeSpan.FromSeconds(value));
            return value;
        }

        private async static Task AwaitAndProcessAsync(Task<int> task)
        {
            int result = await task;
            Trace.WriteLine(result);
        }

        private static async Task ConfigureAwaitContextDemoAsync()
        {
            Console.WriteLine($"Starting on thread: {Thread.CurrentThread.ManagedThreadId}");
            Console.WriteLine();

            Console.WriteLine("With captured context");
            await AwaitWithCapturedContextAsync();

            Console.WriteLine();
            Console.WriteLine("Now without captured context");
            await AwaitWithoutCapturedContextAsync();
        }

        private static async Task AwaitWithCapturedContextAsync()
        {
            Console.WriteLine($"Before await: {Thread.CurrentThread.ManagedThreadId}");

            // resumes on captured context (e.g., UI thread)
            await Task.Delay(300);
            Console.WriteLine($"After await (captured context): {Thread.CurrentThread.ManagedThreadId}");
        }

        private static async Task AwaitWithoutCapturedContextAsync()
        {
            Console.WriteLine($"Before await: {Thread.CurrentThread.ManagedThreadId}");

            // does not resume on captured context
            await Task.Delay(300).ConfigureAwait(false); // no effect in console apps (no SynchronizationContext) neither at ASP.NET Core apps
            Console.WriteLine($"After await (no captured context): {Thread.CurrentThread.ManagedThreadId}");
        }

        private static async Task TestValueTaskScenarioAsync()
        {
            Console.WriteLine("Testing ValueTask scenarios");
            Console.WriteLine();

            using var httpClient = new HttpClient(new FakeHttpMessageHandler())
            {
                BaseAddress = new Uri("https://fake.api/")
            };

            IFeatureFlagService featureFlagService = new FeatureFlagService(httpClient);
            var service = new MySyncImplementation(featureFlagService);

            var transaction = new Transaction { Amount = 250.00m };

            // warm-up load feature flag for slow path
            await featureFlagService.IsNewCheckoutEnabledAsync();

            // test fast path
            // now ValueTask should hit fast path and complete synchronously
            var sw = Stopwatch.StartNew(); // measure time

            for(int i = 0; i < 10; i++)
            {
                double price = await service.CalculatePriceSmartWithProgressAsync(transaction);
                Console.WriteLine($"Price #{i + 1}: {price:N2}");
            }

            sw.Stop();
            Console.WriteLine($"Total time for 10 calls: {sw.ElapsedMilliseconds} ms");
            Console.WriteLine("END");
        }

        private static async Task TestValueTaskWhenAllConsuptionAsync()
        {
            Console.WriteLine();
            Console.WriteLine("VALUETASK + WHENALL CONSUMPTION TEST");
            Console.WriteLine();

            using var httpClient = new HttpClient(new FakeHttpMessageHandler())
            {
                BaseAddress = new Uri("https://fake.api/")
            };

            IFeatureFlagService featureFlagService = new FeatureFlagService(httpClient);
            var service = new MySyncImplementation(featureFlagService);

            var transaction = new Transaction
            {
                Id = 42,
                Amount = 499.99m,
                Time = DateTime.Now
            };

            // warm-up load feature flag for slow path
            await featureFlagService.IsNewCheckoutEnabledAsync();

            try
            {
                // ValueTask -> AsTask -> Task.WhenAll -> await result
                string summary = await service.BuildCheckoutSummaryAsync(transaction);

                Console.WriteLine($"[RESPONSE] {summary}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] {ex.GetType().Name}: {ex.Message}");
            }

            Console.WriteLine();
            Console.WriteLine("END OF VALUETASK CONSUMPTION TEST");
        }

        #region helpers
        private sealed class Order
        {
            public int Quantity { get; set; }
        }
        private sealed class Product
        {
            public decimal Price { get; set; }
        }
        #endregion
    }
}
