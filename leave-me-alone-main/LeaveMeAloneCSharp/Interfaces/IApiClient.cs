namespace LeaveMeAloneCSharp.Interfaces
{
    public interface IApiClient
    {
        Task<string> GetData(string endpoint, CancellationToken ct = default);
    }
}
