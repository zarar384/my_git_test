namespace LeaveMeAloneFuncSkillForge.Repositories.Interfaces
{
    public interface IFilmRepository
    {
        IEnumerable<Film> GetAll();
        IEnumerable<Film> GetFilmsByGenre(string genre);
    }
}
