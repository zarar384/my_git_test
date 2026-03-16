using LeaveMeAloneCSharp.Models;
using LeaveMeAloneCSharp.Strategies.Interfaces;

namespace LeaveMeAloneCSharp.Strategies
{
    public class PayPalPaymentStrategy : IPaymentStrategy
    {
        public PaymentMethod Method => PaymentMethod.PayPal;

        // just a silly way to get the method name... please God don't judge me...
        //public string Method => nameof(PayPalPaymentStrategy).Replace(nameof(IPaymentStrategy).TrimStart('I'), "");


        public Task<PaymentResult> ProcessAsync(PaymentRequest request)
        {
            // Simulate processing payment with PayPal
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
