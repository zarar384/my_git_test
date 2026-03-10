namespace LeaveMeAloneCSharp.Interfaces
{
    // async factory pattern
    public interface IApiClientFactory
    {
        Task<IApiClient> CreateClientAsync(string serviceName, CancellationToken ct = default);
    }
}