namespace LeaveMeAloneFuncSkillForge.Services
{
    public class FilmService
    {
        private readonly IFilmRepository _filmRepository;

        public FilmService(IFilmRepository filmRepository)
        {
            _filmRepository = filmRepository;
        }

        public void PrintFilmsByGenreSortedByRevenue(string genre)
        {
            var films = _filmRepository.GetFilmsByGenre(genre)
                .OrderByDescending(f => f.BoxOfficeRevenue);

            if (!films.Any())
            {
                Console.WriteLine("THERE ARE NO FILMS IN THIS GENRE");
                return;
            }
            var filmsFormattedSelect = films.Select((x, i) => $"{i}: {x.Title}");
            Console.WriteLine(string.Join(Environment.NewLine, filmsFormattedSelect));
        }
    }
}
