using LeaveMeAloneFuncSkillForge.Interfaces;
using System.Net.Http;

namespace LeaveMeAloneFuncSkillForge.Services
{
    public sealed class FeatureFlagService: IFeatureFlagService
    {
        private readonly HttpClient _httpClient;
        private readonly Random _random = new Random();
        // cashe completed task - no need to check multiple times
        private readonly Task<bool> _paymentMethodFeatureEnabledTask;
        private  Task<bool>? _checkoutfeatureEnabledTask;

        public FeatureFlagService(HttpClient httpClient)
        {
            _httpClient = httpClient;

            bool chance50 = _random.Next(2) == 0;

            // set the environment variable to simulate feature flag
            // just for demo purposes
            Environment.SetEnvironmentVariable("NEW_PAYMENT_METHOD_ENABLED", chance50 ? "true" : "false");

            // simulate loading from config at startup
            var value = Environment.GetEnvironmentVariable("NEW_PAYMENT_METHOD_ENABLED") == "true";

            _paymentMethodFeatureEnabledTask = Task.FromResult(value);

            // simulate async check for new checkout feature flag
            //_checkoutfeatureEnabledTask = LoadFlagNewCheckoutAsync(httpClient, cancellationToken);
        }

        public Task<bool> IsNewPaymentMethodEnabledAsync(CancellationToken cancellationToken = default)
        {
            //return await _paymentMethodFeatureEnabledTask;
            return _checkoutfeatureEnabledTask ??= LoadFlagAsync(_httpClient, "feature/new-checkout", cancellationToken);
        }

        private static async Task<bool> LoadFlagAsync
            (HttpClient client,
            string uri,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var response = await client.GetStringAsync(
                    uri,
                    cancellationToken);

                // fake API returns "true" / "false"
                return true;
            }
            catch(OperationCanceledException)
            {
                Console.WriteLine("[FeatureFlagService] Loading new checkout feature flag was canceled.");
                throw; // propagate cancellation
            }
            catch(HttpRequestException ex)
            {
                Console.WriteLine($"[FeatureFlagService] HTTP error while loading new checkout feature flag: {ex.Message}");
                return false; // in case of HTTP error, disable the feature
            }
            catch
            {
                return false; // in case of error, disable the feature
            }
        }

        public Task<bool> IsNewCheckoutEnabledAsync(CancellationToken cancellationToken = default)
        {
            return _checkoutfeatureEnabledTask ??= LoadFlagAsync(_httpClient, "feature/new-checkout", cancellationToken);
        }

        // ValueTask for fast path access
        public ValueTask<bool> IsNewCheckoutEnabledFastAsync(CancellationToken cancellationToken = default)
        {
            // fast path - return cached result if available
            if (_checkoutfeatureEnabledTask != null && 
                _checkoutfeatureEnabledTask.IsCompletedSuccessfully)
            {
                return new ValueTask<bool>(_checkoutfeatureEnabledTask.Result);
            }

            // otherwise, slow path, load asynchronously
            return new ValueTask<bool>(IsNewCheckoutEnabledAsync(cancellationToken));
        }
    }
}
