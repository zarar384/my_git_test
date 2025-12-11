using LeaveMeAloneFuncSkillForge.Domain;
using LeaveMeAloneFuncSkillForge.Services.Reporting.Readers;
using LeaveMeAloneFuncSkillForge.Services.Reports.Environment;

namespace LeaveMeAloneFuncSkillForge.Test
{
    public class FullFilmReportTests
    {
        [Fact]
        public void FullFilmReport_ReturnsTwoReports()
        {
            // Arrange
            var films = new[]
            {
                new Film { Genre = "Action", BoxOfficeRevenue = 100 },
                new Film { Genre = "Action", BoxOfficeRevenue = 200 },
                new Film { Genre = "Drama", BoxOfficeRevenue = 300 }
            };

            var env = new FilmReportEnvironment
            {
                Films = films,
                TopGenres = 10,
                Currency = "USD"
            };

            // Act
            var reports = FilmReportReaders.FullFilmReport().Run(env);

            // Assert
            Assert.Equal(2, reports.Count);
        }

        [Fact]
        public void FullFilmReport_HasCorrectReportTitles()
        {
            // Arrange
            var films = new[]
            {
                new Film { Genre = "Action", BoxOfficeRevenue = 100 }
            };

            var env = new FilmReportEnvironment
            {
                Films = films,
                TopGenres = 5,
                Currency = "USD"
            };

            // Act
            var reports = FilmReportReaders.FullFilmReport().Run(env);

            // Assert
            Assert.Contains(reports, r => r.Title == "Film Count by Genre");
            Assert.Contains(reports, r => r.Title == "Total Box Office Revenue by Genre");
        }

        [Fact]
        public void FullFilmReport_RespectsTopGenresFromEnvironment()
        {
            // Arrange
            var films = new[]
            {
                new Film { Genre = "Action" },
                new Film { Genre = "Action" },
                new Film { Genre = "Drama" }
            };

            var env = new FilmReportEnvironment
            {
                Films = films,
                TopGenres = 1,
                Currency = "USD"
            };

            // Act
            var reports = FilmReportReaders.FullFilmReport().Run(env);
            var countReport = reports.Single(r => r.Title == "Film Count by Genre");

            // Assert
            Assert.Single(countReport.Rows);
            Assert.Equal("Action", countReport.Rows[0].ColumnOne);
        }

        [Fact]
        public void FullFilmReport_UsesCurrencyFromEnvironment()
        {
            // Arrange
            var films = new[]
            {
                new Film { Genre = "Action", BoxOfficeRevenue = 150 }
            };

            var env = new FilmReportEnvironment
            {
                Films = films,
                Currency = "EUR",
                TopGenres = 5
            };

            // Act
            var reports = FilmReportReaders.FullFilmReport().Run(env);
            var revenueReport = reports
                .Single(r => r.Title == "Total Box Office Revenue by Genre");

            // Assert
            Assert.Contains("EUR", revenueReport.Rows[0].ColumnTwo);
        }

        [Fact]
        public void FullFilmReport_WithEmptyFilms_ReturnsEmptyRows()
        {
            // Arrange
            var env = new FilmReportEnvironment
            {
                Films = Enumerable.Empty<Film>(),
                TopGenres = 5,
                Currency = "USD"
            };

            // Act
            var reports = FilmReportReaders.FullFilmReport().Run(env);

            // Assert
            foreach (var report in reports)
                Assert.Empty(report.Rows);
        }
    }
}
