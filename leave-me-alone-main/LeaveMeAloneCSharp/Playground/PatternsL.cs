using LeaveMeAloneCSharp.Strategies.Interfaces;
namespace LeaveMeAloneCSharp.Playground
{
    public static class PatternsL
    {
        public static async Task Run()
        {
            await AdapterPatternEapToTapDemo();
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
            var legacyDownloader = new Utils.Adapters.LegacyFileDownloader();
            var url = "http://example.com/file.txt";
            try
            {
                // use the adapter extension method to call the EAP-based downloader in a TAP style
                string content = await Utils.LegacyFileDownloaderExtensions.DownloadFileTaskAsync(legacyDownloader, url);
                Console.WriteLine($"Downloaded Content: {content}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error downloading file: {ex.Message}");
            }

            Console.WriteLine();
            Console.WriteLine("FINISHED ADAPTER PATTERN (EAP to TAP) EXAMPLE");
        }
    }
}
