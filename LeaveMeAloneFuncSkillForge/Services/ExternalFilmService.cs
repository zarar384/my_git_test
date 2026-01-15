using LeaveMeAloneFuncSkillForge.Interfaces;

namespace LeaveMeAloneFuncSkillForge.Services
{
    public class ExternalFilmService : IExternalFilmService
    {
        private readonly HttpClient _httpClient;

        public ExternalFilmService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        // сombining LINQ with async is OK, but concurrency should be limited in production
        public async Task<string> GetAllAsync(IEnumerable<int> ids)
        {
            // define async operations for each film ID
            var tasks = ids.Select(id => TryGetFilmAsync(id));

            // execute all tasks in parallel
            Task<string>[] allTasks = tasks.ToArray();

            // wait for all tasks to complete 
            string[] htmlPages = await Task.WhenAll(allTasks);

            return string.Join("\n", htmlPages.Select(r=> $"[RESPONSE] {r}"));
        }


        private async Task<string> TryGetFilmAsync(int id)
        {
            try
            {
                return await GetFilmAsync(id);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Failed to get film {id}: {ex.Message}");
                return $"<div>Error loading film {id}</div>";
            }
        }

        private async Task<string> GetFilmAsync(int id)
        {
            Console.WriteLine($"[REQUEST] films/{id}");

            var response = await _httpClient.GetAsync($"films/{id}");
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            return content;
        }
    }
}
