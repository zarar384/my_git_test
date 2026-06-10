using LeaveMeAloneCSharp.Colleagues;
using LeaveMeAloneCSharp.Mediators;
using LeaveMeAloneCSharp.Strategies.Interfaces;
using LeaveMeAloneCSharp.Utils.Adapters;
using Nito.AsyncEx;
using System.ComponentModel;
namespace LeaveMeAloneCSharp.Playground
{
    public static class PatternsL
    {
        public static async Task Run()
        {
            await TestBindableTaskAsync();
        }

        private static void SimpleStrategyPatternDemo()
        {
            Console.WriteLine("SIMPLE STRATEGY PATTERN EXAMPLE");
            Console.WriteLine();

            // Example usage of the Strategy pattern
            // Define some discount strategies
            var noDiscount = new Strategies.NoDiscountStrategy();
            var studentDiscount = new Strategies.StudentDiscountStrategy();
            var vipDiscount = new Strategies.VipDiscountStrategy();

            // Context: Discount calculator that uses a strategy
            var discountCalculator = new Services.DiscountCalculator(studentDiscount);

            // Calculate discounted price
            decimal originalPrice = 100m;

            // Calulate the discounted price using the student discount strategy
            decimal discountedPriceForStudent = discountCalculator.Calculate(originalPrice);

            Console.WriteLine($"Original Price: {originalPrice:C}, Discounted Price: {discountedPriceForStudent:C}");
            Console.WriteLine();

            // Now switch to VIP discount strategy
            decimal discountedPriceForVip = discountCalculator.UseStrategy(vipDiscount).Calculate(originalPrice);

            Console.WriteLine($"Original Price: {originalPrice:C}, Discounted Price for VIP: {discountedPriceForVip:C}");
            Console.WriteLine();

            // Now switch to no discount strategy
            decimal discountedPriceForNoDiscount = discountCalculator.UseStrategy(noDiscount).Calculate(originalPrice);

            Console.WriteLine($"Original Price: {originalPrice:C}, Discounted Price with No Discount: {discountedPriceForNoDiscount:C}");
            Console.WriteLine();

            Console.WriteLine("FINISHED SIMPLE STRATEGY PATTERN EXAMPLE");
        }

        private static async Task MediumLevelStrategyPatternDemo()
        {
            Console.WriteLine("MEDIUM-LEVEL STRATEGY PATTERN EXAMPLE");
            Console.WriteLine();

            // In a more complex scenario, we might have multiple strategies for different payment methods
            var strategies = new List<IPaymentStrategy>
            {
                new Strategies.CreditCardPaymentStrategy(),
                new Strategies.PayPalPaymentStrategy(),
                new Strategies.CryptoPaymentStrategy()
            };

            // Context: Payment service that uses a list of strategies to process payments
            var paymentService = new Services.PaymentService(strategies);

            // Example payment request
            var paymentRequest = new Models.PaymentRequest
            {
                Amount = 150m,
                Currency = "USD",
                Method = PaymentMethod.CreditCard
            };

            var result = await paymentService.ProcessPaymentAsync(paymentRequest);

            Console.WriteLine($"Payment Result: {(result.IsSuccess ? "Success" : "Failure")}, Message: {result.Message}");
            Console.WriteLine();

            // Now let's try with a different payment method
            paymentRequest = new Models.PaymentRequest
            {
                Amount = 200m,
                Currency = "USD",
                Method = PaymentMethod.PayPal
            };

            result = await paymentService.ProcessPaymentAsync(paymentRequest);

            Console.WriteLine($"Payment Result: {(result.IsSuccess ? "Success" : "Failure")}, Message: {result.Message}");
            Console.WriteLine();

            // And now with an unsupported payment method
            Console.WriteLine("Attempting to process payment with unsupported method...");
            paymentRequest = new Models.PaymentRequest
            {
                Amount = 300m,
                Currency = "USD",
                Method = PaymentMethod.None
            };

            result = await paymentService.ProcessPaymentAsync(paymentRequest);

            Console.WriteLine($"Payment Result: {(result.IsSuccess ? "Success" : "Failure")}, Message: {result.Message}");
            Console.WriteLine();

            Console.WriteLine("FINISHED MEDIUM-LEVEL STRATEGY PATTERN EXAMPLE");
        }

        private static async Task MediumLevelParallelStrategyPattern()
        {
            Console.WriteLine("MEDIUM-LEVEL PARALLEL STRATEGY PATTERN EXAMPLE");
            Console.WriteLine();

            // In a more complex scenario, we might have multiple strategies for film recommendations
            var strategies = new List<IFilmRecommendationStrategy>
            {
                new Strategies.TopRevenueStrategy(),
                new Strategies.GenreStrategy("Action"),
                new Strategies.RandomPickStrategy()
            };

            // Context: Film recommendation engine that uses multiple strategies in parallel
            var recommendationEngine = new Services.FilmRecommendationEngine(strategies);

            // Example list of films
            var films = new List<Film>
            {
                new Film { Id = 1, Title = "Action Movie 1", Genre = "Action", BoxOfficeRevenue = 300_000_000 },
                new Film { Id = 2, Title = "Drama Movie 1", Genre = "Drama", BoxOfficeRevenue = 100_000_000 },
                new Film { Id = 3, Title = "Comedy Movie 1", Genre = "Comedy", BoxOfficeRevenue = 200_000_000 },
                new Film { Id = 4, Title = "Action Movie 2", Genre = "Action", BoxOfficeRevenue = 400_000_000 },
                new Film { Id = 5, Title = "Action Movie 3", Genre = "Action", BoxOfficeRevenue = 500_000_000 },
            };

            var recommendations = await recommendationEngine.RecommendAsync(films);
            Console.WriteLine("Recommended Films:");

            foreach (var film in recommendations)
            {
                Console.WriteLine($"- {film.Title} ({film.Genre}, Box Office: {film.BoxOfficeRevenue:C})");
            }

            Console.WriteLine();
            Console.WriteLine("FINISHED MEDIUM-LEVEL PARALLEL STRATEGY PATTERN EXAMPLE");
        }

        // The Adapter pattern is used to convert the interface of a Event-based Asynchronous Pattern (EAP) to the Task-based Asynchronous Pattern (TAP)
        private static async Task AdapterPatternEapToTapDemo()
        {
            Console.WriteLine("ADAPTER PATTERN (EAP to TAP) EXAMPLE");
            Console.WriteLine();

            // imagine we have a legacy file downloader that uses the Event-based Asynchronous Pattern (EAP)
            var legacyDownloader = new LegacyFileDownloader();
            var url = "http://example.com/file.txt";
            try
            {
                // use the adapter extension method to call the EAP-based downloader in a TAP style
                string content = await LegacyFileDownloaderExtensions.DownloadFileTaskAsync(legacyDownloader, url);
                Console.WriteLine($"Downloaded Content: {content}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error downloading file: {ex.Message}");
            }

            Console.WriteLine();
            Console.WriteLine("FINISHED ADAPTER PATTERN (EAP to TAP) EXAMPLE");
        }

        // The Adapter pattern is used to convert the interface of a Asynchronous Programming Model (APM) to the Task-based Asynchronous Pattern (TAP)
        private static async Task AdapterPatternApmToTapDemo()
        {
            Console.WriteLine("ADAPTER PATTERN (APM to TAP) EXAMPLE");
            Console.WriteLine();

            try
            {
                // Legacy service that uses Begin/End pattern (APM)
                var legacyService = new LegacyCalculationService();

                // Use the adapter extension method to call the APM-based service in a TAP style
                int result = await legacyService.CalculateSquareAsync(10);

                Console.WriteLine($"Result: {result}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error calculating square: {ex.Message}");
            }

            Console.WriteLine();
            Console.WriteLine("FINISHED ADAPTER PATTERN (APM to TAP) EXAMPLE");
        }


        // The Adapter pattern is used to convert the interface of a custom callback-based asynchronous method to the Task-based Asynchronous Pattern (TAP)
        private static async Task AdapterPatternCustomAsyncToTapDemo()
        {
            Console.WriteLine("ADAPTER PATTERN (CUSTOM CALLBACK to TAP) EXAMPLE");
            Console.WriteLine();

            // Legacy service with non-standard async (callback-based)
            var legacyService = new LegacyHttpService();

            // Use a custom adapter to convert the callback-based method to a Task-based method
            string result = await legacyService.DownloadStringAsync("https://example.com");

            Console.WriteLine($"Downloaded content: {result}");
            Console.WriteLine();

            Console.WriteLine("FINISHED ADAPTER PATTERN (CUSTOM CALLBACK to TAP) EXAMPLE");
        }

        // A simple demonstration of the Mediator pattern in a form example,
        // where the mediator coordinates the interactions between a text box and a button
        private static void MediatorPatternFormDemo()
        {
            Console.WriteLine("MEDIATOR PATTERN FORM EXAMPLE");
            Console.WriteLine();

            // Create the mediator
            var formMediator = new FormMediator();

            // Create colleagues and register them with the mediator
            var textBox = new TextBox(formMediator);
            var button = new Button();

            formMediator.TextBox = textBox;
            formMediator.Button = button;

            // Simulate user input
            Console.WriteLine("User types 'Hello' in the text box...");
            textBox.SetText("Hello");

            Console.WriteLine($"Button enabled: {button.Enabled}");
            Console.WriteLine();

            Console.WriteLine("User clears the text box...");
            textBox.SetText("");

            Console.WriteLine($"Button enabled: {button.Enabled}");
            Console.WriteLine();

            Console.WriteLine("FINISHED MEDIATOR PATTERN FORM EXAMPLE");
        }

        //  Lazy<T>: shared resource initialized exactly once
        // factory runs on first Value access, result cached for all callers
        private static void TestLazySharedResource()
        {
            // simulate shared config loaded from disk - expensive, done once
            var config = new Lazy<AppConfig>(() =>
            {
                Thread.Sleep(50); // simulate file read
                return new AppConfig { MaxRetries = 3, TimeoutMs = 5000 };
            });

            // multiple threads all get the same instance
            var threads = Enumerable.Range(0, 5)
                .Select(_ => new Thread(() =>
                {
                    var cfg = config.Value;
                    Console.WriteLine($"[LAZY] MaxRetries={cfg.MaxRetries} thread={Thread.CurrentThread.ManagedThreadId}");
                }))
                .ToList();

            threads.ForEach(t => t.Start());
            threads.ForEach(t => t.Join());

            Console.WriteLine();
        }

        // Lazy<Task<T>>: async shared resource, initialized once
        // all callers await the same Task - factory never runs twice
        private static async Task TestLazyAsyncSharedResourceAsync()
        {
            // simulate shared auth token fetched from identity server - one request, many consumers
            var tokenSource = new Lazy<Task<string>>(async () =>
            {
                await Task.Delay(60); // simulate HTTP call to auth server
                return $"token-{Guid.NewGuid():N}";
            });

            var tasks = Enumerable.Range(0, 5).Select(async i =>
            {
                string token = await tokenSource.Value;
                Console.WriteLine($"[LAZY ASYNC] caller {i} got token: {token[..12]}...");
            });

            await Task.WhenAll(tasks);
            Console.WriteLine();
        }

        // Lazy<Task<T>> with Task.Run: factory always runs on thread pool
        // prevents context issues when callers come from different synchronization contexts
        private static async Task TestLazyAsyncOnThreadPoolAsync()
        {
            // wrap factory in Task.Run - guaranteed to run on thread pool regardless of caller context
            var schemaSource = new Lazy<Task<string>>(() => Task.Run(async () =>
            {
                await Task.Delay(40);
                return "{ \"version\": 2, \"fields\": [\"id\", \"name\", \"amount\"] }";
            }));

            string schema = await schemaSource.Value;
            Console.WriteLine($"[LAZY THREAD POOL] schema loaded: {schema[..30]}...");
            Console.WriteLine();
        }

        //  AsyncLazy<T>: retries on failure, unlike Lazy<Task<T>> which caches the faulted task
        private static async Task TestAsyncLazyWithRetryAsync()
        {
            int attempt = 0;

            // first call fails, AsyncLazy retries on next access - Lazy<Task<T>> would cache the failure
            var priceList = new AsyncLazy<decimal[]>(async () =>
            {
                attempt++;
                if (attempt == 1)
                    throw new HttpRequestException("price service temporarily unavailable");

                await Task.Delay(30);
                return new[] { 9.99m, 24.99m, 4.49m };
            }, AsyncLazyFlags.RetryOnFailure);

            try { await priceList; } catch (HttpRequestException ex) { Console.WriteLine($"[ASYNC LAZY] attempt 1 failed: {ex.Message}"); }

            decimal[] prices = await priceList;
            Console.WriteLine($"[ASYNC LAZY] attempt 2 succeeded, {prices.Length} prices loaded");
            Console.WriteLine();
        }

        // BindableTask<T>: wraps async operation for data binding
        // exposes IsNotCompleted / IsSuccessfullyCompleted / IsFaulted / Result
        // without this, UI has no way to react to async state transitions
        private static async Task TestBindableTaskAsync()
        {
            // simulate ViewModel loading user profile
            var profileLoad = new BindableTask<UserProfile>(LoadUserProfileAsync());

            Console.WriteLine($"[BINDABLE] IsNotCompleted: {profileLoad.IsNotCompleted}");

            await Task.Delay(150); // let the task finish

            Console.WriteLine($"[BINDABLE] IsSuccessfullyCompleted: {profileLoad.IsSuccessfullyCompleted}");
            Console.WriteLine($"[BINDABLE] Result: {profileLoad.Result?.Name}");

            // simulate a faulted task
            var failedLoad = new BindableTask<UserProfile>(Task.FromException<UserProfile>(
                new InvalidOperationException("user not found")));

            await Task.Delay(20);

            Console.WriteLine($"[BINDABLE] IsFaulted: {failedLoad.IsFaulted}");
            Console.WriteLine();
        }

        // bool trick: single core method serves both sync and async callers
        // avoids duplicating business logic; sync API must never hit an incomplete task
        private static async Task TestSyncAsyncDualApiAsync()
        {
            var processor = new InvoiceProcessor();

            // async caller awaits naturally
            decimal asyncResult = await processor.CalculateTaxAsync(1500m);
            Console.WriteLine($"[DUAL API] async result: {asyncResult:C}");

            // sync caller blocks safely - core guarantees task is already complete
            decimal syncResult = processor.CalculateTax(1500m);
            Console.WriteLine($"[DUAL API] sync result:  {syncResult:C}");

            Console.WriteLine();
        }

        #region helpers
        private static async Task<UserProfile> LoadUserProfileAsync()
        {
            await Task.Delay(100); // simulate db fetch
            return new UserProfile { Name = "Alice", Role = "Admin" };
        }

        private sealed class AppConfig
        {
            public int MaxRetries { get; set; }
            public int TimeoutMs { get; set; }
        }

        private sealed class UserProfile
        {
            public string Name { get; set; } = "";
            public string Role { get; set; } = "";
        }

        private sealed class BindableTask<T> : INotifyPropertyChanged
        {
            private readonly Task<T> _task;

            public BindableTask(Task<T> task)
            {
                _task = task;
                var _ = WatchAsync();
            }

            private async Task WatchAsync()
            {
                try { await _task; } catch { }

                OnPropertyChanged(nameof(IsNotCompleted));
                OnPropertyChanged(nameof(IsSuccessfullyCompleted));
                OnPropertyChanged(nameof(IsFaulted));
                OnPropertyChanged(nameof(Result));
            }

            public bool IsNotCompleted => !_task.IsCompleted;
            public bool IsSuccessfullyCompleted => _task.Status == TaskStatus.RanToCompletion;
            public bool IsFaulted => _task.IsFaulted;
            public T? Result => IsSuccessfullyCompleted ? _task.Result : default;

            public event PropertyChangedEventHandler? PropertyChanged;
            private void OnPropertyChanged(string name) =>
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        private sealed class InvoiceProcessor
        {
            // single implementation - sync flag decides which delay API to use
            private async Task<decimal> CalculateTaxCore(decimal amount, bool sync)
            {
                if (sync)
                    Thread.Sleep(20); // sync path: blocking call
                else
                    await Task.Delay(20); // async path: non-blocking

                return Math.Round(amount * 0.21m, 2);
            }

            public Task<decimal> CalculateTaxAsync(decimal amount) =>
                CalculateTaxCore(amount, sync: false);

            public decimal CalculateTax(decimal amount) =>
                CalculateTaxCore(amount, sync: true).GetAwaiter().GetResult();
        }
        #endregion
    }
}
