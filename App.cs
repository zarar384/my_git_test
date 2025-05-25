using LeaveMeAloneFuncSkillForge.Functional;
using LeaveMeAloneFuncSkillForge.Repositories;
using LeaveMeAloneFuncSkillForge.Utils;

namespace LeaveMeAloneFuncSkillForge
{
    public static class App
    {
        public static void RunApp()
        {
            var filmRepository = new FilmRepository();
            var films = filmRepository.GetAll();
            films = FilmFilters.GetFilmsByGenre(films, "Action");

            films.PrintTable();
        }
    }
}
