namespace LeaveMeAloneFuncSkillForge.Interfaces
{
    public interface IMyAsyncInterface
    {
        Task<int> GetIntAsync();

        Task DoSomethingAsync();

        Task<T> NotImplementedAsync<T>();

        Task DoSomethingWithExceptionAsync();

        Task<string> GetPaymentMethodAsync();

        Task<double> CalculatePriceAsync(Transaction transaction);
    }
}
