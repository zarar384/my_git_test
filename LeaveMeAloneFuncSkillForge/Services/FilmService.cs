using LeaveMeAloneFuncSkillForge.DiscriminatedUnions;

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

        public Either<ErrorInfo, IEnumerable<Film>> GetFilmsByGenreSorted(string genre)
        {
            try
            {
                var films = _filmRepository
                    .GetFilmsByGenre(genre)
                    .OrderByDescending(f => f.BoxOfficeRevenue)
                    .ToList();

                if (films.Count == 0)
                {
                    return new Left<ErrorInfo, IEnumerable<Film>>(
                        new ErrorInfo("Empty", "There are no films in this genre")
                    );
                }

                return new Right<ErrorInfo, IEnumerable<Film>>(films);
            }
            catch (Exception ex)
            {
                // is needed to catch unexpected exceptions, for example, database connection issues
                return new Left<ErrorInfo, IEnumerable<Film>>(
                    new ErrorInfo("Exception", ex.Message)
                );
            }
        }
    }
}
