using LeaveMeAloneCSharp.Strategies.Interfaces;

namespace LeaveMeAloneCSharp.Strategies
{
    public class RandomPickStrategy : IFilmRecommendationStrategy
    {
        private readonly Random random = new();

        public string Name => "RandomPick";

        public async Task<List<Film>> RecommendAsync(IEnumerable<Film> films)
        {
            Console.WriteLine($"* Applying {Name} strategy...");

            await Task.Delay(200);

            return films
                .OrderBy(_ => random.Next())
                .Take(2)
                .ToList();
        }
    }
}
