using LeaveMeAloneFuncSkillForge.DiscriminatedUnions;
using LeaveMeAloneFuncSkillForge.Domain;
using LeaveMeAloneFuncSkillForge.Repositories.Interfaces;
using Moq;

namespace LeaveMeAloneFuncSkillForge.Test
{
    public class ResultDuTests
    {
        [Fact]
        public void SuccessEmptyExample_ShouldReturnUnknownMessage()
        {
            // Arrange
            var mockRepo = new Mock<IFilmRepository>();

            // Act
            var film = GetFilmByTitle("NonExistentGenre", mockRepo.Object);
            var message = film switch
            {
                Success<Film> s when s.Value == null => $"Unknown Film!",
                Success<Film> s2 => $"Found film: {s2.Value.Title} Genre {s2.Value.Genre}",
                Failure<Film> e => $"An error occurred: {e.Error}",
                _ => throw new NotImplementedException(),
            };

            // Assert
            Assert.Equal("Unknown Film!", message);
        }

        [Fact]
        public void SuccessExample_ShouldReturnFoundFilmMessage()
        {
            // Arrange
            var mockRepo = new Mock<IFilmRepository>();
            mockRepo.Setup(r => r.GetFilmByTitle("The Godfather")).Returns(
                new Film { Title = "The Godfather", BoxOfficeRevenue = 200 });

            // Act
            var film = GetFilmByTitle("The Godfather", mockRepo.Object)!;
            var message = film switch
            {
                Success<Film> s when s.Value == null => $"Unknown Film!",
                Success<Film> s2 => $"Found film: {s2.Value.Title} Genre {s2.Value.Genre}",
                Failure<Film> e => $"An error occurred: {e.Error}",
                _ => throw new NotImplementedException(),
            };

            // Assert
            Assert.StartsWith("Found film: ", message);
        }

        [Fact]
        public void FailureExample_ShouldReturnExceptioMessage()
        {
            // Arrange
            var mockRepo = new Mock<IFilmRepository>();
            var films = new List<Film>
            {
                new Film { Title = "The Godfather", BoxOfficeRevenue = 200 },
                new Film { Title = "Kill Bill", BoxOfficeRevenue = 300 },
                new Film { Title = "Kill Bill", BoxOfficeRevenue = 100 }
            };

            mockRepo.Setup(r => r.GetFilmByTitle(It.IsAny<string>()))
                    .Returns((string title) => films.SingleOrDefault(f => f.Title == title)!);

            // Act
            var film = GetFilmByTitle("Kill Bill", mockRepo.Object)!;
            var message = film switch
            {
                Success<Film> s when s.Value == null => $"Unknown Film!",
                Success<Film> s2 => $"Found film: {s2.Value.Title} Genre {s2.Value.Genre}",
                Failure<Film> e => $"An error occurred: {e.Error}",
                _ => throw new NotImplementedException(),
            };

            // Assert
            Assert.StartsWith("An error occurred: ", message);
        }

        private Result<Film> GetFilmByTitle(string title, IFilmRepository filmRepository)
        {
            try
            {
                var maybeFilm = filmRepository.GetFilmByTitle(title);

                return new Success<Film>(maybeFilm);
            }
            catch (InvalidOperationException e)
            {
                return new Failure<Film>(e);
            }
        }
    }
}
