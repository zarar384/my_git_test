using System.Linq.Expressions;

namespace LeaveMeAloneFuncSkillForge.Interfaces
{
    internal interface IExternalFilmService
    {
        Task<string> GetAllAsync(IEnumerable<int> ids);
        Task<string> GetFirstRespondingAsync(int urlIdA, int urlIdB);
        Task<string> GetFirstSuccessfulResponseAsync(int urlIdA, int urlIdB);

        // Use keyset pagination to retrieve films in pages
        Task<KeysetPage<Film, int>> GetFilmsPageAsync(
            int? cursor,
            int pageSize,
            CancellationToken cancellationToken = default);
    }
}
