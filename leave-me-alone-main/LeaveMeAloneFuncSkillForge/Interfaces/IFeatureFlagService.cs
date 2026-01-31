namespace LeaveMeAloneFuncSkillForge.Interfaces
{
    public interface IFeatureFlagService
    {
        Task<bool> IsNewPaymentMethodEnabledAsync(CancellationToken cancellationToken = default);

        Task<bool> IsNewCheckoutEnabledAsync(CancellationToken cancellationToken = default);
    }
}
