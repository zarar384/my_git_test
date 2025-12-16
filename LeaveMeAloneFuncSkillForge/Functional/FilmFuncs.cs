namespace LeaveMeAloneFuncSkillForge.Functional
{
    public static class FilmFuncs
    {
        public static IEnumerable<Film> GetFilmsByGenre(
            IEnumerable<Film> source,
            string genre) =>
            source.Where(x => x.Genre == genre);

        public static Func<IEnumerable<Film>, int, IEnumerable<(string Genre, IEnumerable<Film> TopFilms)>>
            GetTopRevenueFilmsByGenreAboveAverage =>
                (films, count) => films
                    .GroupBy(x => x.Genre)
                    .Select(g =>
                    {
                        var average = g.Average(f => f.BoxOfficeRevenue);

                        var topFilms = g
                            .Where(f => f.BoxOfficeRevenue > average)
                            .OrderByDescending(f => f.BoxOfficeRevenue)
                            .Take(count);

                        return (Genre: g.Key, TopFilms: topFilms);
                    })
                    .Where(g => g.TopFilms.Any());

        public static Func<IEnumerable<string?>, IEnumerable<string>> GetFormattedFilmInfos(Func<string, Film?> lookup)
        => titles =>
            titles
            .Where(title => !string.IsNullOrWhiteSpace(title))             
            .Select(title => lookup(title!))                               
            .Where(film => film is not null && !string.IsNullOrEmpty(film.Title)) 
            .Select(film => $"{film!.Title} ({film.Genre}) — ${film.BoxOfficeRevenue:N1}M");
    }
}
