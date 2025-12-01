using LeaveMeAloneFuncSkillForge.DiscriminatedUnions;
using LeaveMeAloneFuncSkillForge.Domain;
using LeaveMeAloneFuncSkillForge.Repositories.Interfaces;
using Moq;

namespace LeaveMeAloneFuncSkillForge.Test
{
    public class MaybeDuTest
    {
        [Fact]
        public void NothingExample_ShouldReturnNoFilmFoundMessage()
        {
            // Arrange
            var mockRepo = new Mock<IFilmRepository>();

            // Act
            var film = GetFilmFirstFilmByGenre("NonExistentGenre", mockRepo.Object);
            var message = film switch
            {
                Something<Film> something => $"Found film: {something.Value.Title}",
                Nothing<Film> _ => "No film found",
                _ => throw new NotImplementedException(),
            };

            // Assert
            Assert.Equal("No film found", message);
        }

        [Fact]
        public void SomethingExample_ShouldReturnFoundFilmMessage()
        {
            // Arrange
            var mockRepo = new Mock<IFilmRepository>();
            mockRepo.Setup(r => r.GetFilmsByGenre("Drama")).Returns(new List<Film>
            {
                new Film { Title = "Film I", BoxOfficeRevenue = 200 },
                new Film { Title = "Film HATE", BoxOfficeRevenue = 300 },
                new Film { Title = "Film MYSELF", BoxOfficeRevenue = 100 }
            });

            // Act
            var film = GetFilmFirstFilmByGenre("Drama", mockRepo.Object)!;
            var message = film switch
            {
                Something<Film> something => $"Found film: {something.Value.Title}",
                Nothing<Film> _ => "No film found",
                _ => throw new NotImplementedException(),
            };

            // Assert
            Assert.StartsWith("Found film: ", message);
        }

        [Fact]
        public void ErrorExample_ShouldReturnErrorMessage()
        {
            // Arrange
            var mockRepo = new Mock<IFilmRepository>();
            mockRepo
                .Setup(r => r.GetFilmsByGenre("Thriller"))
                .Throws(new Exception("Database is down"));

            // Act
            var film = GetFilmFirstFilmByGenre_ExceptionSafe("Thriller", mockRepo.Object);

            var message = film switch
            {
                Something<Film> s => $"Found film: {s.Value.Title}",
                Nothing<Film> _ => "No film found",
                Error<Film> e => $"Error happened: {e.CapturedError.Message}",
                _ => throw new NotImplementedException(),
            };

            // Assert
            Assert.Equal("Error happened: Database is down", message);
        }

        [Fact]
        public void ErrorExample_ShouldReturnErrorType()
        {
            // Arrange
            var mockRepo = new Mock<IFilmRepository>();
            mockRepo.Setup(r => r.GetFilmsByGenre("Comedy"))
                .Throws(new InvalidOperationException("Something bad"));

            // Act
            var result = GetFilmFirstFilmByGenre_ExceptionSafe("Comedy", mockRepo.Object);

            // Assert
            Assert.IsType<Error<Film>>(result);
            Assert.Equal("Something bad", ((Error<Film>)result).CapturedError.Message);
        }

        private Maybe<Film> GetFilmFirstFilmByGenre(string genre, 
            IFilmRepository filmRepository)
        {
            Maybe<Film> maybeFilm = filmRepository.GetFilmsByGenre(genre)
                .FirstOrDefault() is { } film
                    ? new Something<Film>(film)
                    : new Nothing<Film>();
            return maybeFilm;
        }

        private Maybe<Film> GetFilmFirstFilmByGenre_ExceptionSafe(string genre, 
            IFilmRepository filmRepository)
        {
            try
            {
                var films = filmRepository.GetFilmsByGenre(genre);

                return films.FirstOrDefault() is { } film
                    ? new Something<Film>(film)
                    : new Nothing<Film>();
            }
            catch (Exception ex)
            {
                return new Error<Film>(ex);
            }
        }
    }
}
