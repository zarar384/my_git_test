using LeaveMeAloneFuncSkillForge.Data.Context;
using LeaveMeAloneFuncSkillForge.DiscriminatedUnions;
using LeaveMeAloneFuncSkillForge.Functional.Monads;

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

        public Maybe<IEnumerable<Film>> GetFilms(RequestContext ctx, string genre)
        {
            var films = _filmRepository.GetFilmsByGenre(genre).ToList();

            return films.Any()
                ? new Something<IEnumerable<Film>>(films)
                : new Nothing<IEnumerable<Film>>();
        }

        public Maybe<IEnumerable<Film>> OnlyWithRevenue(RequestContext ctx, IEnumerable<Film> films)
        {
            var filtered = films.Where(f => f.BoxOfficeRevenue > 0).ToList();

            return filtered.Any()
                ? new Something<IEnumerable<Film>>(filtered)
                : new Nothing<IEnumerable<Film>>();
        }

        public Maybe<IEnumerable<FilmInfoDto>> TakeTop(
            RequestContext ctx,
            IEnumerable<Film> films,
            int topN)
        {
            var result = films
                .OrderByDescending(f => f.BoxOfficeRevenue)
                .Take(topN)
                .Select(f => new FilmInfoDto(f.Title, f.BoxOfficeRevenue))
                .ToList();

            return result.Any()
                ? new Something<IEnumerable<FilmInfoDto>>(result)
                : new Nothing<IEnumerable<FilmInfoDto>>();
        }

        public StateMaybe<RequestContext, IEnumerable<FilmInfoDto>> BuildFilmReport(
            string userName,
            string genre,
            int topN)
        {
            return new RequestContext(userName)
                .ToStateMaybe(genre)
                .Bind((ctx, g) => GetFilms(ctx, g))
                .Bind((ctx, films) => OnlyWithRevenue(ctx, films))
                .Bind((ctx, films) => TakeTop(ctx, films, topN));
        }

        /// <summary>
        /// Retrieves top N films of a given genre with positive revenue,
        /// returns Either with ErrorInfo on failure or FilmInfoDto list on success.
        /// </summary>
        public Either<ErrorInfo, IEnumerable<FilmInfoDto>> GetTopFilmInfo(string genre, int topN) =>
            // Validate genre
            Validation.ValidateGenre(genre)
                .Bind(validGenre =>
                    // Validate topN
                    Validation.ValidateTopN(topN)
                        .Bind(validTopN =>
                            // Get films by genre
                            GetFilmsByGenreSorted(validGenre)
                                // Map each film to Maybe<FilmInfoDto>
                                .Map(films =>
                                    films
                                        .Select(f => f.BoxOfficeRevenue > 0
                                            ? new Something<Film>(f) as Maybe<Film>
                                            : new Nothing<Film>())
                                        .Select(m => m.Map(f => new FilmInfoDto(f.Title, f.BoxOfficeRevenue)))
                                        .OfType<Something<FilmInfoDto>>() // keep only successful results
                                        .Select(s => s.Value)
                                        .Take(validTopN)
                                )
                                // Convert empty result to Left, otherwise Right
                                .Bind<ErrorInfo, IEnumerable<FilmInfoDto>, IEnumerable<FilmInfoDto>>(dto =>
                                    dto.Any()
                                        ? new Right<ErrorInfo, IEnumerable<FilmInfoDto>>(dto)
                                        : new Left<ErrorInfo, IEnumerable<FilmInfoDto>>(
                                            new ErrorInfo("NoValidFilms", "No films with valid revenue")
                                          )
                                )
                        )
                );
    }
}
