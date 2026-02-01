namespace LeaveMeAloneFuncSkillForge.Interfaces
{
    public interface IMyAsyncInterface
    {
        Task<int> GetIntAsync();

        Task DoSomethingAsync();

        Task<T> NotImplementedAsync<T>();

        Task DoSomethingWithExceptionAsync();

        Task<string> GetPaymentMethodAsync(CancellationToken cancellationToken = default);

        Task<string> GetPaymentMethodWithExceptionAsync(CancellationToken cancellationToken = default);

        Task<double> CalculatePriceAsync(Transaction transaction);

        Task<double> CalculatePriceWithProgressAsync(
            Transaction transaction,
            IProgress<double>? progress = null,
            CancellationToken cancellationToken = default);

        ValueTask<double> CalculatePriceSmartWithProgressAsync(
            Transaction transaction,
            IProgress<double>? progress = null,
            CancellationToken cancellationToken = default);
    }
}
