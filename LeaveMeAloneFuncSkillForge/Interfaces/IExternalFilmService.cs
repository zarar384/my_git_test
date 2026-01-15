namespace LeaveMeAloneFuncSkillForge.Interfaces
{
    internal interface IExternalFilmService
    {
        Task<string> GetAllAsync(IEnumerable<int> ids);
    }
}
