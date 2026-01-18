namespace LeaveMeAloneFuncSkillForge.Interfaces
{
    internal interface IExternalFilmService
    {
        Task<string> GetAllAsync(IEnumerable<int> ids);
        Task<string> GetFirstRespondingAsync(int urlIdA, int urlIdB);
        Task<string> GetFirstSuccessfulResponseAsync(int urlIdA, int urlIdB);
    }
}
