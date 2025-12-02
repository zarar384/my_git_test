using LeaveMeAloneFuncSkillForge.DiscriminatedUnions;
using LeaveMeAloneFuncSkillForge.Domain;
using LeaveMeAloneFuncSkillForge.Common;
using LeaveMeAloneFuncSkillForge.Repositories.Interfaces;
using LeaveMeAloneFuncSkillForge.Services;
using Moq;

namespace LeaveMeAloneFuncSkillForge.Test
{
    public class EitherTests
    {
        [Fact]
        public void GetFilmsByGenreSorted_ShouldReturnRight_WhenFilmsExist()
        {
            // Arrange
            var mockRepo = new Mock<IFilmRepository>();
            mockRepo.Setup(r => r.GetFilmsByGenre("Drama"))
                .Returns(new List<Film>
                {
                    new Film { Title = "A", BoxOfficeRevenue = 100 },
                    new Film { Title = "B", BoxOfficeRevenue = 300 },
                    new Film { Title = "C", BoxOfficeRevenue = 200 }
                });

            var service = new FilmService(mockRepo.Object);

            // Act
            var result = service.GetFilmsByGenreSorted("Drama");

            // Assert
            var right = Assert.IsType<Right<ErrorInfo, IEnumerable<Film>>>(result);
            var films = right.Value.ToList();

            Assert.Equal(3, films.Count);
            Assert.Equal("B", films[0].Title); // highest revenue first
        }

        [Fact]
        public void GetFilmsByGenreSorted_ShouldReturnLeft_WhenNoFilms()
        {
            // Arrange
            var mockRepo = new Mock<IFilmRepository>();
            mockRepo.Setup(r => r.GetFilmsByGenre("Fantasy"))
                .Returns(new List<Film>()); // empty list

            var service = new FilmService(mockRepo.Object);

            // Act
            var result = service.GetFilmsByGenreSorted("Fantasy");

            // Assert
            var left = Assert.IsType<Left<ErrorInfo, IEnumerable<Film>>>(result);
            Assert.Equal("Empty", left.Value.Code);
            Assert.Equal("There are no films in this genre", left.Value.Message);
        }

        [Fact]
        public void GetFilmsByGenreSorted_ShouldReturnLeft_WhenExceptionThrown()
        {
            // Arrange
            var mockRepo = new Mock<IFilmRepository>();
            mockRepo.Setup(r => r.GetFilmsByGenre("Horror"))
                .Throws(new Exception("DB connection failed"));

            var service = new FilmService(mockRepo.Object);

            // Act
            var result = service.GetFilmsByGenreSorted("Horror");

            // Assert
            var left = Assert.IsType<Left<ErrorInfo, IEnumerable<Film>>>(result);
            Assert.Equal("Exception", left.Value.Code);
            Assert.Equal("DB connection failed", left.Value.Message);
        }

        [Fact]
        public void StartWithPositiveInt_BindAndMapThroughMaybe_ReturnsExpectedString()
        {
            // Arrange
            Either<string, int> start = new Right<string, int>(5);

            // Act
            // chain of Bind and Map with Maybe inside
            var result = start
                // double if positive
                .Bind<string, int, int>(x =>
                    x > 0 ? new Right<string, int>(x * 2) : new Left<string, int>("Negative or zero"))

                // convert to Maybe, wrap only even numbers
                .Map(x =>
                    x % 2 == 0 ? new Something<int>(x) as Maybe<int> : new Nothing<int>())

                // operate on Maybe inside Either
                .Map(maybe =>
                    maybe.Bind<int, int>(v =>
                        v > 5 ? new Something<int>(v + 10) : new Nothing<int>()))

                // map to string message
                .Map(m =>
                    m switch
                    {
                        Something<int> s => $"Success: {s.Value}",
                        Nothing<int> => "No valid value",
                        Error<int> e => $"Error: {e.CapturedError}",
                        _ => "Unknown"
                    });

            var final = result switch
            {
                Right<string, string> r => r.Value,
                Left<string, string> l => l.Value,
                _ => "Unexpected"
            };

            // Assert
            Assert.Equal("Success: 20", final);
        }
    }
}
