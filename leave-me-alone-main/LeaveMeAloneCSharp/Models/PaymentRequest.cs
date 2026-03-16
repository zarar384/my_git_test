namespace LeaveMeAloneCSharp.Models
{
    public class PaymentRequest
    {
        public decimal Amount { get; init; }
        public string Currency { get; init; } = default!;
        public PaymentMethod Method { get; init; } = PaymentMethod.None;
    }
}
