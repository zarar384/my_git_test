namespace LeaveMeAloneCSharp.Strategies.Interfaces
{
    public interface IDiscountStrategy
    {
        decimal ApplyDiscount(decimal price);
    }
}
