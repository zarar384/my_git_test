namespace LeaveMeAloneFuncSkillForge.Services
{
    public class FilmAnalyzer
    {
        private readonly List<Film> _films;
        private IEnumerable<Func<Film, string>> _descriptors;

        public FilmAnalyzer(List<Film> films)
        {
            _films = films;
            _descriptors = CreateDescriptors();
        }

        // lazy collection of funcs
        private IEnumerable<Func<Film, string>> CreateDescriptors()
        {
            yield return film => $"Title: {film.Title}";
            yield return film => $"Genre: {film.Genre}";
            yield return film => film.BoxOfficeRevenue > 1_000_000
                ? $"Blockbuster Revenue: ${film.BoxOfficeRevenue:N0}"
                : $"Revenue: ${film.BoxOfficeRevenue:N0}";

            yield return film => {
                Console.WriteLine($"Evaluating long computation for {film.Title}...");
                Thread.Sleep(1000); // simulation of hard operation
                return $"🎞️ Summary ready for: {film.Title}";
            };
        }
    }
}
