namespace LeaveMeAloneFuncSkillForge.Repositories
{
    public class FilmRepository: IFilmRepository
    {
        public IEnumerable<Film> Films { get; }

        public FilmRepository(int initialCount = 20)
        {
            Films = FakeDatabase.FilmFaker.Generate(initialCount);
        }


        public IEnumerable<Film> GetAll() => Films;

        public Film GetFilmByTitle(string title)
         => Films.SingleOrDefault(f => f.Title == title)!;

        public IEnumerable<Film> GetFilmsByGenre(string genre) =>
            Films.Where(f => f.Genre == genre);
    }
}
