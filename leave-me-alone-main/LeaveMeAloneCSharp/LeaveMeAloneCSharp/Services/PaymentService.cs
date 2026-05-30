namespace LeaveMeAloneCSharp.Services
{
    /// <summary>
    /// Medium-level implementation of the Strategy pattern for processing payments. 
    /// The PaymentService class maintains a collection of payment strategies, each corresponding to a different payment method (e.g., PayPal, Credit Card, Crypto).
    /// </summary>
    public class PaymentService
    {
        private readonly Dictionary<PaymentMethod, Strategies.Interfaces.IPaymentStrategy> _paymentStrategies;

        public PaymentService(IEnumerable<Strategies.Interfaces.IPaymentStrategy> paymentStrategies)
        {
            _paymentStrategies = paymentStrategies.ToDictionary(s => s.Method);
        }

        /// <summary>
        /// Processes a payment request using the appropriate strategy based on the payment method specified in the request.
        /// The method looks up the strategy for the given payment method and invokes its ProcessAsync method. 
        /// If no strategy is found for the specified method, it returns a failed PaymentResult indicating that the payment method is not supported.
        /// </summary>
        public Task<Models.PaymentResult> ProcessPaymentAsync(Models.PaymentRequest request)
        {
            if (_paymentStrategies.TryGetValue(request.Method, out var strategy))
            {
                return strategy.ProcessAsync(request);
            }
            else
            {
                return Task.FromResult(new Models.PaymentResult
                {
                    IsSuccess = false,
                    Message = $"Payment method '{request.Method}' is not supported."
                });
            }
        }
    }
}
