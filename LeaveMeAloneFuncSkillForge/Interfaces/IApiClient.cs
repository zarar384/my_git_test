namespace LeaveMeAloneFuncSkillForge.Interfaces
{
    public interface IApiClient
    {
        Task<string> GetDataAsync(string endpoint, CancellationToken ct = default);
    }
}
