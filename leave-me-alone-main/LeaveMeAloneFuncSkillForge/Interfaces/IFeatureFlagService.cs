namespace LeaveMeAloneFuncSkillForge.Interfaces
{
    public interface IFeatureFlagService
    {
        Task<bool> IsNewPaymentMethodEnabledAsync();

        Task<bool> IsNewCheckoutEnabledAsync();
    }
}
