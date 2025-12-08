namespace LeaveMeAloneFuncSkillForge.Services
{
    public class ReportItem
    {
        public string ColumnOne { get; set; }
        public string ColumnTwo { get; set; }
    }
    
    public class Report
    {
        public string Title { get; set; }
        public List<ReportItem> Rows { get; set; } 
            = new List<ReportItem>();
    }

    public class FilmReportService
    {
        public Report GenerateGenreCountReport(IEnumerable<Film> films)
        {
            return GenerateReport(
                films,
                f => f.Genre,
                g => g.Count().ToString(),
                "Film Count by Genre"
            );
        }

        public Report GenerateRevenueByGenreReport(IEnumerable<Film> films)
        {
            return GenerateReport(
                films,
                f => f.Genre,
                g => g.Sum(f => f.BoxOfficeRevenue).ToString("C"),
                "Total Box Office Revenue by Genre"
            );
        }

        public Report GenerateReport<T>(
            IEnumerable<Film> films,
            Func<Film, T> groupBySelector,
            Func<IGrouping<T,Film>, string> summartySelector,
            string title)
        {
            var summary = films
                .GroupBy(groupBySelector)
                .Select(g=> new ReportItem
                {
                    ColumnOne = g.Key?.ToString() ?? "Unknown",
                    ColumnTwo = summartySelector(g)
                })
                .ToList();

            var report = new Report
            {
                Title = title,
                Rows = summary
            };

            if (!report.Rows.Any())
            {
                // Handle empty case
            }

            return report;
        }
    }
}
