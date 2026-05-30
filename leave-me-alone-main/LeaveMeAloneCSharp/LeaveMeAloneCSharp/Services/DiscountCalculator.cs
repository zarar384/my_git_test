using LeaveMeAloneCSharp.Strategies.Interfaces;

namespace LeaveMeAloneCSharp.Services
{
    /// <summary>
    /// Simple implementation of the Strategy pattern for calculating discounts.
    /// </summary>
    public class DiscountCalculator
    {
        private IDiscountStrategy _discountStrategy;

        public DiscountCalculator(IDiscountStrategy discountStrategy)
        {
            _discountStrategy = discountStrategy;
        }

        /// <summary>
        /// Sets the discount strategy to be used for calculating discounts. 
        /// This allows for dynamic changes in discount calculation logic at runtime, enabling flexibility 
        /// in how discounts are applied based on different strategies.
        /// </summary>
        /// <returns>The current instance of DiscountCalculator for method chaining.</returns>
        public DiscountCalculator UseStrategy(IDiscountStrategy discountStrategy)
        {
            _discountStrategy = discountStrategy ?? throw new ArgumentNullException(nameof(discountStrategy));

            return this;
        }

        /// <summary>
        /// Calculates the discounted price based on the current discount strategy.
        /// </summary>
        public decimal Calculate(decimal amount)
        {
            return _discountStrategy.ApplyDiscount(amount);
        }
    }
}
