using LeaveMeAloneCSharp.Models;

namespace LeaveMeAloneCSharp.Strategies.Interfaces
{
    public interface IPaymentStrategy
    {
        PaymentMethod Method { get; }

        Task<PaymentResult> ProcessAsync(PaymentRequest request);
    }
}
