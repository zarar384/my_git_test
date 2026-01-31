using LeaveMeAloneFuncSkillForge.Interfaces;

namespace LeaveMeAloneFuncSkillForge.Services
{
    public class MySyncImplementation : IMyAsyncInterface
    {
        private readonly Random _random = new Random();
        private readonly IFeatureFlagService _featureFlagService;

        public MySyncImplementation(IFeatureFlagService featureFlagService)
        {
            _featureFlagService = featureFlagService;
        }

        public Task DoSomethingAsync()
        {
            return Task.CompletedTask;
        }

        public Task DoSomethingWithExceptionAsync()
        {
            try
            {
                bool chance50 = _random.Next(2) == 0;
                
                // Simulate some synchronous work that throws an exception
                Task.Delay(100).Wait();

                if (chance50)
                    throw new InvalidOperationException("[EXCEPTION] Task failed due to some issue.");

                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                return Task.FromException(ex);
            }
        }

        public async Task<string> GetPaymentMethodWithExceptionAsync(CancellationToken cancellationToken = default)
        {
            bool chance50 = _random.Next(2) == 0;

            // Simulate some synchronous work that throws an exception
            await Task.Delay(100, cancellationToken);

            if (chance50)
            {
                throw new InvalidOperationException("[EXCEPTION] Task failed due to some issue.");
            }

            // Check feature flag
            var paymentMethod = await GetPaymentMethodAsync(cancellationToken);

            if(string.IsNullOrEmpty(paymentMethod))
            {
                throw new Exception("[EXCEPTION] Payment method is not available.");
            }

            return paymentMethod;
        }

        public async Task<string> GetPaymentMethodAsync(CancellationToken cancellationToken = default)
        {
            var isNewEnabled = await _featureFlagService.IsNewPaymentMethodEnabledAsync(cancellationToken);

            return isNewEnabled
                ? "NewPaymentMethod"
                : "OldPaymentMethod";
        }

        public Task<int> GetIntAsync()
        {
            return Task.FromResult(_random.Next(1, 101));
        }

        public async Task<double> CalculatePriceAsync(Transaction transaction)
        {
            var isCheckoutNewEnabled = await _featureFlagService.IsNewCheckoutEnabledAsync();
            double basePrice = (double)transaction.Amount * _random.Next(1, 5);

            if (isCheckoutNewEnabled)
            {
                // apply a 10% discount for the new checkout feature
                return basePrice * 0.9;
            }
            else
            {
                return basePrice;
            }
        }

        public Task<T> NotImplementedAsync<T>()
        {
            return Task.FromException<T>(new NotImplementedException("[WRONG] This method is not implemented."));
        }

        public async Task<double> CalculatePriceWithProgressAsync(
            Transaction transaction, 
            IProgress<double>? progress = null, 
            CancellationToken cancellationToken = default)
        {
            double basePrice = (double)transaction.Amount;
            double precent = 0;

            // validation
            await Task.Delay(300, cancellationToken);
            precent = 0.25;
            progress?.Report(precent);

            // feature flag check
            var isCheckoutNewEnabled = await _featureFlagService.IsNewCheckoutEnabledAsync();
            precent = 0.5;
            progress?.Report(precent);

            // price calculation
            await Task.Delay(300, cancellationToken);
            var calculatedPrice = basePrice * _random.Next(1, 5);
            precent = 0.75;
            progress?.Report(precent);

            // apply feature flag logic
            await Task.Delay(300, cancellationToken);

            if (isCheckoutNewEnabled)
            {
                calculatedPrice *= 0.9; // apply a 10% discount for the new checkout feature
            }

            precent = 1.0;
            progress?.Report(precent);

            return calculatedPrice;
        }
    }
}
