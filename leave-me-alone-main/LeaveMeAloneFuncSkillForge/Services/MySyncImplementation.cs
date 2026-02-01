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
            var featureFlagTask = _featureFlagService.IsNewCheckoutEnabledAsync(cancellationToken);

            return await CalculatePriceWithProgressAsync(transaction, progress, cancellationToken, featureFlagTask);
        }
       
        public ValueTask<double> CalculatePriceSmartWithProgressAsync(
            Transaction transaction, 
            IProgress<double>? progress = null, 
            CancellationToken cancellationToken = default)
        {
            // validation sync
            if (transaction == null)
                return ValueTask.FromException<double>(
                    new ArgumentNullException(nameof(transaction)));

            if (transaction.Amount < 0)
                return ValueTask.FromException<double>(
                    new ArgumentOutOfRangeException(nameof(transaction.Amount), "Amount cannot be negative."));

            progress?.Report(0.0);
            var checkoutFeatureFlagTask = _featureFlagService.IsNewCheckoutEnabledFastAsync(cancellationToken);

            // if the feature flag task is already completed, we can proceed synchronously
            if (checkoutFeatureFlagTask.IsCompletedSuccessfully)
                return new ValueTask<double>(CalculatePriceSyncWithProgress(transaction, checkoutFeatureFlagTask.Result, progress));

            // otherwise, we need to await the feature flag task asynchronously
            return new ValueTask<double>(CalculatePriceWithProgressAsync(transaction, progress, cancellationToken, checkoutFeatureFlagTask));
        }

        #region private helpers for CalculatePriceSmartWithProgressAsync
        // synchronous fast-path
        private double CalculatePriceSyncWithProgress(
            Transaction transaction,
            bool isCheckoutNewEnabled,
            IProgress<double>? progress)
        {
            progress?.Report(0.25);

            double price = (double)transaction.Amount * _random.Next(1, 5);
            progress?.Report(0.75);

            if (isCheckoutNewEnabled)
                price *= 0.9;

            progress?.Report(1.0);
            return price;
        }

        // shared async body calculation logic
        private async Task<double> CalculatePriceBodyAsync(
            Transaction transaction,
            IProgress<double>? progress,
            CancellationToken cancellationToken,
            bool isCheckoutNewEnabled)
        {
            progress?.Report(0.25);
            await Task.Delay(300, cancellationToken);

            progress?.Report(0.5);
            await Task.Delay(300, cancellationToken);

            double price = (double)transaction.Amount * _random.Next(1, 5);
            progress?.Report(0.75);

            await Task.Delay(300, cancellationToken);

            if (isCheckoutNewEnabled)
                price *= 0.9;

            progress?.Report(1.0);
            return price;
        }

        // async wrapper for Task<bool> feature flag
        private async Task<double> CalculatePriceWithProgressAsync(
            Transaction transaction,
            IProgress<double>? progress,
            CancellationToken cancellationToken,
            Task<bool> checkoutFeatureFlagTask)
        {
            bool isCheckoutNewEnabled = await checkoutFeatureFlagTask;
            return await CalculatePriceBodyAsync(
                transaction, progress, cancellationToken, isCheckoutNewEnabled);
        }

        // async wrapper for optimized ValueTask<bool> feature flag
        private async Task<double> CalculatePriceWithProgressAsync(
            Transaction transaction,
            IProgress<double>? progress,
            CancellationToken cancellationToken,
            ValueTask<bool> checkoutFeatureFlagTask)
        {
            bool isCheckoutNewEnabled = await checkoutFeatureFlagTask;

            return await CalculatePriceBodyAsync(transaction, progress, cancellationToken, isCheckoutNewEnabled);
        }
        #endregion
    }
}
