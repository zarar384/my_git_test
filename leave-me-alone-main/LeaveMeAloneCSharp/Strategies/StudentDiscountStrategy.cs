using LeaveMeAloneCSharp.Strategies.Interfaces;

namespace LeaveMeAloneCSharp.Strategies
{
    public class StudentDiscountStrategy : IDiscountStrategy
    {
        public decimal ApplyDiscount(decimal price)
        {
            Console.WriteLine("* Applying student discount strategy.");
            return price * 0.9m; // Apply a 10% discount for students
        }
    }
}
