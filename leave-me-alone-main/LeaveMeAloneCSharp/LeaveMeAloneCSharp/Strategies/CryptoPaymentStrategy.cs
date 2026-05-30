using LeaveMeAloneCSharp.Models;
using LeaveMeAloneCSharp.Strategies.Interfaces;

namespace LeaveMeAloneCSharp.Strategies
{
    public class CryptoPaymentStrategy : IPaymentStrategy
    {
        public PaymentMethod Method => PaymentMethod.Crypto;

        // just a silly way to get the method name... please God don't judge me...
        //public string Method => nameof(CryptoPaymentStrategy).Replace(nameof(IPaymentStrategy).TrimStart('I'), "");

        public Task<PaymentResult> ProcessAsync(PaymentRequest request)
        {
            // Simulate processing payment with Crypto
            Console.WriteLine($"Processing {request.Amount} {request.Currency} via {Method}...");

            // Simulate success
            var result = new PaymentResult
            {
                IsSuccess = true,
                Message = $"Payment processed successfully with {Method} for {request.Amount} {request.Currency}."
            };

            return Task.FromResult(result);
        }
    }
}
