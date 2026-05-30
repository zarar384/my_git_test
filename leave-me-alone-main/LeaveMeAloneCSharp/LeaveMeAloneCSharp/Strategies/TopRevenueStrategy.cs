using LeaveMeAloneCSharp.Strategies.Interfaces;

namespace LeaveMeAloneCSharp.Strategies
{
    public class TopRevenueStrategy : IFilmRecommendationStrategy
    {
        public string Name => "TopRevenue";

        public async Task<List<Film>> RecommendAsync(IEnumerable<Film> films)
        {
            Console.WriteLine("* Calculating top revenue films...");
            await Task.Delay(300);

            return films
                .OrderByDescending(f => f.BoxOfficeRevenue)
                .Take(3)
                .ToList();
        }
    }
}
