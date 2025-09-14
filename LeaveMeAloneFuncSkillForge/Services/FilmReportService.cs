using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

            return report;
        }
    }
}
