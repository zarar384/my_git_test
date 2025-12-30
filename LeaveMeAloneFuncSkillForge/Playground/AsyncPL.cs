using LeaveMeAloneFuncSkillForge.API;
using LeaveMeAloneFuncSkillForge.Interfaces;

namespace LeaveMeAloneFuncSkillForge.Playground
{
    public static class AsyncPL
    {
        public static async Task Run()
        {
            await RunBasicAsyncExamples();
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

            var results = await client1.GetDataAsync("/data/endpoint1");
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

            var results = await client1.GetDataAsync("/data/endpoint1");
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
