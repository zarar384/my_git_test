using LeaveMeAloneFuncSkillForge.Data;
using LeaveMeAloneFuncSkillForge.Domain;

namespace LeaveMeAloneFuncSkillForge.Repositories
{
    public class FilmRepository
    {
        private List<Film> _films;

        public FilmRepository(int initialCount = 20)
        {
            _films = FakeDatabase.FilmFaker.Generate(initialCount);
        }

        public IEnumerable<Film> GetAll() => _films;

        public IEnumerable<Film> GetFilmsByGenre(string genre) =>
            _films.Where(f => f.Genre == genre);
    }
}
