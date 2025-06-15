using LeaveMeAloneFuncSkillForge.Domain;
using LeaveMeAloneFuncSkillForge.Functional;
using LeaveMeAloneFuncSkillForge.Repositories;
using LeaveMeAloneFuncSkillForge.Repositories.Interfaces;
using LeaveMeAloneFuncSkillForge.Services;
using Moq;

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
            var actiopnFilms = FilmFilters.GetFilmsByGenre(films, "Action");

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
    }
}
