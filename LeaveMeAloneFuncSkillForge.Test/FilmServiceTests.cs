using LeaveMeAloneFuncSkillForge.DiscriminatedUnions;
using LeaveMeAloneFuncSkillForge.Domain;
using LeaveMeAloneFuncSkillForge.DTOs;
using LeaveMeAloneFuncSkillForge.Repositories.Interfaces;
using LeaveMeAloneFuncSkillForge.Services;

namespace LeaveMeAloneFuncSkillForge.Test
{
    public class FilmServiceTests
    {
        private class FakeFilmRepository : IFilmRepository
        {
            public List<Film> Films { get; set; } = new List<Film>();

            // explicit implementation for Films property
            IEnumerable<Film> IFilmRepository.Films => Films;

            public IEnumerable<Film> GetAll()
            {
                throw new NotImplementedException();
            }

            public Film GetFilmByTitle(string title)
            {
                throw new NotImplementedException();
            }

            public IEnumerable<Film> GetFilmsByGenre(string genre) =>
                Films.Where(f => f.Genre == genre);
        }

        private FilmService CreateService(List<Film> films) =>
            new FilmService(new FakeFilmRepository { Films = films });

        [Fact]
        public void GetTopFilmInfo_InvalidGenre_ReturnsLeft()
        {
            // Arrange
            var service = CreateService(new List<Film>());

            // Act
            var result = service.GetTopFilmInfo("", 5);

            // Assert
            Assert.IsType<Left<ErrorInfo, IEnumerable<FilmInfoDto>>>(result);
            var left = result as Left<ErrorInfo, IEnumerable<FilmInfoDto>>;
            Assert.Equal("InvalidGenre", left.Value.Code);
        }

        [Fact]
        public void GetTopFilmInfo_InvalidTopN_ReturnsLeft()
        {
            // Arrange
            var service = CreateService(new List<Film>());

            // Act
            var result = service.GetTopFilmInfo("Action", 0);

            // Assert
            Assert.IsType<Left<ErrorInfo, IEnumerable<FilmInfoDto>>>(result);
            var left = result as Left<ErrorInfo, IEnumerable<FilmInfoDto>>;
            Assert.Equal("InvalidTopN", left.Value.Code);
        }

        [Fact]
        public void GetTopFilmInfo_NoFilms_ReturnsLeft()
        {
            // Arrange
            var service = CreateService(new List<Film>());

            // Act
            var result = service.GetTopFilmInfo("Action", 5);

            // Assert
            Assert.IsType<Left<ErrorInfo, IEnumerable<FilmInfoDto>>>(result);
            var left = result as Left<ErrorInfo, IEnumerable<FilmInfoDto>>;
            Assert.Equal("Empty", left.Value.Code);
        }


        [Fact]
        public void GetTopFilmInfo_AllFilmsZeroRevenue_ReturnsLeft()
        {
            // Arrange
            var films = new List<Film>
            {
                new Film { Title = "A", Genre = "Action", BoxOfficeRevenue = 0 },
                new Film { Title = "B", Genre = "Action", BoxOfficeRevenue = 0 }
            };
            var service = CreateService(films);

            // Act
            var result = service.GetTopFilmInfo("Action", 5);

            // Assert
            Assert.IsType<Left<ErrorInfo, IEnumerable<FilmInfoDto>>>(result);
            var left = result as Left<ErrorInfo, IEnumerable<FilmInfoDto>>;
            Assert.Equal("NoValidFilms", left.Value.Code);
        }

        [Fact]
        public void GetTopFilmInfo_FewerFilmsThanTopN_ReturnsAllFilms()
        {
            // Arrange
            var films = new List<Film>
            {
                new Film { Title = "A", Genre = "Action", BoxOfficeRevenue = 100 },
                new Film { Title = "B", Genre = "Action", BoxOfficeRevenue = 200 }
            };
            var service = CreateService(films);

            // Act
            var result = service.GetTopFilmInfo("Action", 5);

            // Assert
            Assert.IsType<Right<ErrorInfo, IEnumerable<FilmInfoDto>>>(result);
            var right = result as Right<ErrorInfo, IEnumerable<FilmInfoDto>>;
            Assert.Equal(2, right.Value.Count());
            Assert.Equal("B", right.Value.First().Title); // sorted by BoxOfficeRevenue descending
        }

        [Fact]
        public void GetTopFilmInfo_MoreFilmsThanTopN_ReturnsTopNFilms()
        {
            // Arrange
            var films = new List<Film>
            {
                new Film { Title = "A", Genre = "Action", BoxOfficeRevenue = 100 },
                new Film { Title = "B", Genre = "Action", BoxOfficeRevenue = 200 },
                new Film { Title = "C", Genre = "Action", BoxOfficeRevenue = 300 },
                new Film { Title = "D", Genre = "Action", BoxOfficeRevenue = 50 }
            };
            var service = CreateService(films);

            // Act
            var result = service.GetTopFilmInfo("Action", 2);

            // Assert
            Assert.IsType<Right<ErrorInfo, IEnumerable<FilmInfoDto>>>(result);
            var right = result as Right<ErrorInfo, IEnumerable<FilmInfoDto>>;
            Assert.Equal(2, right.Value.Count());
            Assert.Equal(new[] { "C", "B" }, right.Value.Select(f => f.Title));
        }
    }

}
