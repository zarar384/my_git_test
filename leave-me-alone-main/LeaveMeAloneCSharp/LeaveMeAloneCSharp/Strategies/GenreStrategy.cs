using LeaveMeAloneCSharp.Strategies.Interfaces;

namespace LeaveMeAloneCSharp.Strategies
{
    public class GenreStrategy : IFilmRecommendationStrategy
    {
        private readonly string genre;

        public string Name => "GenreFilter";

        public GenreStrategy(string genre)
        {
            this.genre = genre;
        }

        public async Task<List<Film>> RecommendAsync(IEnumerable<Film> films)
        {
            Console.WriteLine($"* Applying {Name} strategy for genre: {genre}...");

            await Task.Delay(400);

            return films
                .Where(f => f.Genre == genre)
                .Take(3)
                .ToList();
        }
    }
}
