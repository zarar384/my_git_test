using LeaveMeAloneFuncSkillForge.Interfaces;
using System.Net.Http;

namespace LeaveMeAloneFuncSkillForge.Services
{
    public sealed class FeatureFlagService: IFeatureFlagService
    {
        private readonly Random _random = new Random();
        // cashe completed task - no need to check multiple times
        private readonly Task<bool> _paymentMethodFeatureEnabledTask;
        private readonly Task<bool> _checkoutfeatureEnabledTask;

        public FeatureFlagService(HttpClient httpClient)
        {
            bool chance50 = _random.Next(2) == 0;

            // set the environment variable to simulate feature flag
            // just for demo purposes
            Environment.SetEnvironmentVariable("NEW_PAYMENT_METHOD_ENABLED", chance50 ? "true" : "false");

            // simulate loading from config at startup
            var value = Environment.GetEnvironmentVariable("NEW_PAYMENT_METHOD_ENABLED") == "true";

            _paymentMethodFeatureEnabledTask = Task.FromResult(value);

            // simulate async check for new checkout feature flag
            _checkoutfeatureEnabledTask = LoadFlagNewCheckoutAsync(httpClient);
        }

        public async Task<bool> IsNewPaymentMethodEnabledAsync()
        {
            return await _paymentMethodFeatureEnabledTask;
        }

        private static async Task<bool> LoadFlagNewCheckoutAsync(HttpClient client)
        {
            try
            {
                var response = await client.GetStringAsync("feature/new-checkout");

                // fake API returns "true" / "false"
                return true;
            }
            catch
            {
                return false; // in case of error, disable the feature
            }
        }

        public Task<bool> IsNewCheckoutEnabledAsync()
        {
            return _checkoutfeatureEnabledTask;
        }
    }
}
