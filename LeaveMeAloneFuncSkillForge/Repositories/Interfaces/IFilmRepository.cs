namespace LeaveMeAloneFuncSkillForge.Repositories.Interfaces
{
    public interface IFilmRepository
    {
        IEnumerable<Film> Films { get; }
        IEnumerable<Film> GetAll();
        IEnumerable<Film> GetFilmsByGenre(string genre);
        Film GetFilmByTitle(string title);
    }
}
