using LeaveMeAloneFuncSkillForge.Services.Reports.Environment;

namespace LeaveMeAloneFuncSkillForge.Services.Reporting.Readers
{
    public static class FilmReportReaders
    {
        public static Reader<ReportEnvironment, IEnumerable<Film>> GetFilms() =>
            new Reader<ReportEnvironment, IEnumerable<Film>>(env => env.Films);

        public static Reader<ReportEnvironment, IEnumerable<IGrouping<string, Film>>> GroupByGenre() =>
            GetFilms()
                .Bind(film => new Reader<ReportEnvironment, IEnumerable<IGrouping<string, Film>>>(
                    _ => film.GroupBy(f => f.Genre ?? "Unknown")
                    ));

        public static Reader<ReportEnvironment, Report> GenreCountReport() =>
            GroupByGenre()
                .Bind(groups => new Reader<ReportEnvironment, Report>(
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

        public static Reader<ReportEnvironment, Report> RevenueByGenreReport() =>
             GroupByGenre()
                .Bind(groups => new Reader<ReportEnvironment, Report>(env =>
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

        public static Reader<ReportEnvironment, List<Report>> FullFilmReport() =>
            GenreCountReport()
                .Bind(countReport =>
                   RevenueByGenreReport()
                        .Bind(revenueReport =>
                            new Reader<ReportEnvironment, List<Report>>(_ =>
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
