using LeaveMeAloneFuncSkillForge.Common;
using LeaveMeAloneFuncSkillForge.Domain;
using LeaveMeAloneFuncSkillForge.Functional;
using LeaveMeAloneFuncSkillForge.Repositories;
using LeaveMeAloneFuncSkillForge.Repositories.Interfaces;
using LeaveMeAloneFuncSkillForge.Services;
using Moq;
using System.Diagnostics;
using System.Globalization;

namespace LeaveMeAloneFuncSkillForge.Test
{
    public class FilmTests
    {
        [Fact]
        public void GetFilmsByGenre_ShouldReturnOnlyActionFilms()
        {
            // Arrange
            var filmRepository = new FilmRepository();
            var films = filmRepository.GetAll();
            //films.PrintTable();

            // Act
            var actiopnFilms = FilmFuncs.GetFilmsByGenre(films, "Action");

            // Assert
            Assert.NotEmpty(actiopnFilms);
            Assert.All(actiopnFilms, film => Assert.Equal("Action", film.Genre));
        }

        [Fact]
        public void TestWriteFilms()
        {
            // Arrange
            var output = new StringWriter();
            Console.SetOut(output);

            var mockRepo = new Mock<IFilmRepository>();
            mockRepo.Setup(r => r.GetFilmsByGenre("Drama")).Returns(new List<Film>
            {
                new Film { Title = "Film I", BoxOfficeRevenue = 200 },
                new Film { Title = "Film HATE", BoxOfficeRevenue = 300 },
                new Film { Title = "Film MYSELF", BoxOfficeRevenue = 100 }
            });

            // Act
            var service = new FilmService(mockRepo.Object);
            service.PrintFilmsByGenreSortedByRevenue("Drama");

            // Assert
            var result = output.ToString();
            Assert.Contains("1: Film I", result);
        }

        [Fact]
        public void GetTop3RevenueFilmsByGenreAboveAverage_ReturnsCorrectFilms()
        {
            // Arrange
            var films = new List<Film>
            {
                new Film { Title = "A", Genre = "Action", BoxOfficeRevenue = 100 },
                new Film { Title = "B", Genre = "Action", BoxOfficeRevenue = 200 },
                new Film { Title = "C", Genre = "Action", BoxOfficeRevenue = 300 },
                new Film { Title = "D", Genre = "Action", BoxOfficeRevenue = 400 },
                new Film { Title = "E", Genre = "Action", BoxOfficeRevenue = 500 },
                new Film { Title = "F", Genre = "Drama", BoxOfficeRevenue = 150 },
                new Film { Title = "G", Genre = "Drama", BoxOfficeRevenue = 50 },
                new Film { Title = "H", Genre = "Drama", BoxOfficeRevenue = 250 },
            };

            // Act
            var result = FilmFuncs.GetTopRevenueFilmsByGenreAboveAverage(films, 3).ToList();

            // Assert
            Assert.Equal(2, result.Count); // 2 genres: Action, Drama 

            var actionGenre = result.FirstOrDefault(r => r.Genre == "Action");
            Assert.NotNull(actionGenre);
            Assert.Equal(2, actionGenre.TopFilms.Count()); // top 2 above average by genre Action

            var expectedActionTitles = new[] { "E", "D" };
            Assert.Equal(expectedActionTitles, actionGenre.TopFilms.Select(f => f.Title));

            var dramaGenre = result.FirstOrDefault(r => r.Genre == "Drama");
            Assert.NotNull(dramaGenre);
            Assert.Equal(1, dramaGenre.TopFilms.Count()); // only H above average: 150

            Assert.Equal("H", dramaGenre.TopFilms.First().Title);
        }

        [Fact]
        public void HeavyDescriptor_ShouldBeEvaluatedOnlyWhenCalled()
        {
            // Arrange
            var film = new Film
            {
                Title = "Punch-Drunk Love",
                Genre = "Drama",
                BoxOfficeRevenue = 1234567,
            };

            // Act
            var analyzer = new FilmAnalyzer(new List<Film> { film });
            var descriptorsField = typeof(FilmAnalyzer).GetField("_descriptors", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var descriptors = (IEnumerable<Func<Film, string>>)descriptorsField.GetValue(analyzer);

            var heavyFunc = descriptors.Last(); // with Thread.Sleep(1000);

            // set timer
            var stopwatch = Stopwatch.StartNew();
            var result = heavyFunc(film);
            stopwatch.Stop();

            // Assert
            Assert.Contains("Summary ready for", result);
            Assert.True(stopwatch.ElapsedMilliseconds >= 999);
        }

        [Fact]
        public void GetFormattedFilmInfos_ReturnsFormattedList_WithDefaultsForUnknown()
        {
            // Arrange
            var originalCulture = Thread.CurrentThread.CurrentCulture;
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;

            var films = new Dictionary<string, Film>
            {
                ["Inception"] = new Film { Title = "Inception", Genre = "Sci-Fi", BoxOfficeRevenue = 829.9 },
                ["The Godfather"] = new Film { Title = "The Godfather", Genre = "Crime", BoxOfficeRevenue = 246.1 },
            };

            var defaultFilm = new Film { Title = "Unknown", Genre = "Unknown", BoxOfficeRevenue = 0.0 };

            var safeLookup = films.ToLookup(defaultFilm);
            var titlesToLookup = new List<string> { "Inception", "Avatar", "The Godfather" };

            // Act
            var formatter = FilmFuncs.GetFormattedFilmInfos(safeLookup);
            var result = formatter(titlesToLookup).ToList();

            // Assert
            Assert.Contains("Inception (Sci-Fi) — $829.9M", result);
            Assert.Contains("The Godfather (Crime) — $246.1M", result);
            Assert.Equal(2, result.Count);
        }
    }
}