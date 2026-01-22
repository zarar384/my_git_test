using LeaveMeAloneFuncSkillForge.Domain;
using LeaveMeAloneFuncSkillForge.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeaveMeAloneFuncSkillForge.Test
{
    public class FilmReportServiceTests
    {
        [Fact]
        public void GenerateReport_CountByGenre_Works()
        {
            // Arrange
            var films = new List<Film>
            {
                new Film { Title = "Inception", Genre = "Sci-Fi" },
                new Film {Title = "The Dark Knight", Genre = "Action"},
                new Film {Title = "Interstellar", Genre = "Sci-Fi"},
                new Film {Title = "Pulp Fiction", Genre = "Crime"},
                new Film {Title = "Django Unchained", Genre = "Western"},
                new Film {Title = "The Matrix", Genre = "Sci-Fi"}
            };

            var service = new FilmReportService();

            // Act
            var report = service.GenerateReport(
                films,
                f => f.Genre,
                g => g.Count().ToString(),
                "Film Count by Genre"
            );

            // Assert
            Assert.Equal(4, report.Rows.Count); // 4 unique genres: Sci-Fi, Action, Crime, Western
            Assert.Contains(report.Rows, r => r.ColumnOne == "Sci-Fi" && r.ColumnTwo == "3");
            Assert.Contains(report.Rows, r => r.ColumnOne == "Action" && r.ColumnTwo == "1");
            Assert.Contains(report.Rows, r => r.ColumnOne == "Crime" && r.ColumnTwo == "1");
            Assert.Contains(report.Rows, r => r.ColumnOne == "Western" && r.ColumnTwo == "1");
        }

        [Fact]
        public void GenerateReport_SumRevenueByGenre_Works()
        {
            // Arrange
            var films = new List<Film>
            {
                new Film { Title = "Inception", Genre = "Sci-Fi", BoxOfficeRevenue = 100},
                new Film {Title = "Interstellar", Genre = "Sci-Fi", BoxOfficeRevenue = 50},
                new Film {Title = "Pulp Fiction", Genre = "Crime", BoxOfficeRevenue = 69},
            };

            var service = new FilmReportService();

            // Act
            var report = service.GenerateReport(
                films,
                f => f.Genre,
                g => g.Sum(f => f.BoxOfficeRevenue).ToString(),
                "Box Office Revenue by Genre"
            );

            // Assert
            Assert.Equal(2, report.Rows.Count); // 2 unique genres: Sci-Fi, Crime
            Assert.Contains(report.Rows, r => r.ColumnOne == "Sci-Fi" && r.ColumnTwo == "150");
            Assert.Contains(report.Rows, r => r.ColumnOne == "Crime" && r.ColumnTwo == "69");
        }
    }
}
