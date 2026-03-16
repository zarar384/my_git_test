using LeaveMeAloneCSharp.Strategies.Interfaces;

namespace LeaveMeAloneCSharp.Strategies
{
    internal class NoDiscountStrategy : IDiscountStrategy
    {
        public decimal ApplyDiscount(decimal price)
        {
            Console.WriteLine("* Applying no discount strategy.");
            return price;
        }
    }
}
