using LeaveMeAloneFuncSkillForge.Functional;
using LeaveMeAloneFuncSkillForge.Repositories;

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
    }
}
