using LeaveMeAloneCSharp.Strategies.Interfaces;

namespace LeaveMeAloneCSharp.Services
{
    // Middle-level Parallel Strategy Pattern implementation for film recommendations
    public class FilmRecommendationEngine
    {
        private readonly IEnumerable<IFilmRecommendationStrategy> _strategies;

        public FilmRecommendationEngine(IEnumerable<IFilmRecommendationStrategy> strategies)
        {
            _strategies = strategies;
        }

        /// <summary>
        /// Asynchronously recommends films based on multiple strategies in parallel.
        /// </summary>
        public async Task<List<Film>> RecommendAsync(IEnumerable<Film> films)
        {
            var task = _strategies.Select(strategy => strategy.RecommendAsync(films));

            var results = await Task.WhenAll(task);

            return results
                .SelectMany(r => r)
                .DistinctBy(f => f.Id)
                .ToList();
        }
    }
}
