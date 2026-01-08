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

        public Task<int> GetIntAsync()
        {
            return Task.FromResult(_random.Next(1, 101));
        }

        public async Task<string> GetPaymentMethodAsync()
        {
            var isNewEnabled = await _featureFlagService.IsNewPaymentMethodEnabledAsync();

            return isNewEnabled
                ? "NewPaymentMethod"
                : "OldPaymentMethod";
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
    }
}
