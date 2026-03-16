namespace LeaveMeAloneCSharp.Strategies.Interfaces
{
    public interface IFilmRecommendationStrategy
    {
        string Name { get; }

        Task<List<Film>> RecommendAsync(IEnumerable<Film> films);
    }
}
