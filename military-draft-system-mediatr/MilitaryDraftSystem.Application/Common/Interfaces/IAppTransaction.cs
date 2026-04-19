namespace MilitaryDraftSystem.Application.Common.Interfaces
{
    public interface IAppTransaction : IAsyncDisposable
    {
        Task CommitAsync(CancellationToken ct);
        Task RollbackAsync(CancellationToken ct);
    }
}