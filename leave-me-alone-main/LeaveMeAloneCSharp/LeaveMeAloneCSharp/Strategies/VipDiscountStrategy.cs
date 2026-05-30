using LeaveMeAloneCSharp.Strategies.Interfaces;

namespace LeaveMeAloneCSharp.Strategies
{
    public class VipDiscountStrategy : IDiscountStrategy
    {
        public decimal ApplyDiscount(decimal price)
        {
            Console.WriteLine("* Applying VIP discount strategy.");
            return price * 0.8m; // Apply a 20% discount for VIP customers
        }
    }
}
