using LeaveMeAloneFuncSkillForge.Functional.Monads;
using LeaveMeAloneFuncSkillForge.Services.Reports.Environment;

namespace LeaveMeAloneFuncSkillForge.Services.Reporting.Readers
{
    public static class FilmReportReaders
    {
        public static Reader<FilmReportEnvironment, IEnumerable<Film>> GetFilms() =>
            new Reader<FilmReportEnvironment, IEnumerable<Film>>(env => env.Films);

        public static Reader<FilmReportEnvironment, IEnumerable<IGrouping<string, Film>>> GroupByGenre() =>
            GetFilms()
                .Bind(film => new Reader<FilmReportEnvironment, IEnumerable<IGrouping<string, Film>>>(
                    _ => film.GroupBy(f => f.Genre ?? "Unknown")
                    ));

        public static Reader<FilmReportEnvironment, Report> GenreCountReport() =>
            GroupByGenre()
                .Bind(groups => new Reader<FilmReportEnvironment, Report>(
                    env =>
                    {
                        var rows = groups
                        .OrderByDescending(g => g.Count())
                        .Take(env.TopGenres)
                        .Select(g => new ReportItem
                        {
                            ColumnOne = g.Key,
                            ColumnTwo = g.Count().ToString()
                        })
                        .ToList();

                        return new Report
                        {
                            Title = "Film Count by Genre",
                            Rows = rows
                        };
                    }
                ));

        public static Reader<FilmReportEnvironment, Report> RevenueByGenreReport() =>
             GroupByGenre()
                .Bind(groups => new Reader<FilmReportEnvironment, Report>(env =>
                {
                    var rows = groups
                        .Select(g => new ReportItem
                        {
                            ColumnOne = g.Key,
                            ColumnTwo = $"{g.Sum(f => f.BoxOfficeRevenue):N3} {env.Currency}"
                        })
                        .ToList();

                    return new Report
                    {
                        Title = "Total Box Office Revenue by Genre",
                        Rows = rows
                    };
                }));

        public static Reader<FilmReportEnvironment, List<Report>> FullFilmReport() =>
            GenreCountReport()
                .Bind(countReport =>
                   RevenueByGenreReport()
                        .Bind(revenueReport =>
                            new Reader<FilmReportEnvironment, List<Report>>(_ =>
                                new List<Report>
                                {
                                    countReport,
                                    revenueReport
                                }
                            )
                        )
                );
    }

}
