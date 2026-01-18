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

            return string.Join("\n", htmlPages.Select(r => $"[RESPONSE] {r}"));
        }

        public async Task<string> GetFirstRespondingAsync(int urlIdA, int urlIdB)
        {
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));

            // for example, one film from two different servers
            var tasksA = TryGetFilmAsync(urlIdA, cts.Token);
            var tasksB = TryGetFilmAsync(urlIdB, cts.Token);

            // wait for the first task to complete
            var completedTask = await Task.WhenAny(tasksA, tasksB);

            cts.Cancel(); // cancel the slower task

            return await completedTask;
        }

        public async Task<string> GetFirstSuccessfulResponseAsync(
            int urlIdA,
            int urlIdB)
        {
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));

            var tasks = new List<Task<string>>
            {
                GetFilmAsync(urlIdA, cts.Token),
                GetFilmAsync(urlIdB, cts.Token)
                // other pseudo servers
            };

            while (tasks.Count > 0)
            {
                var completedTask = await Task.WhenAny(tasks);

                try
                {
                    var result = await completedTask;
                    cts.Cancel(); // cancel the slower tasks
                    return result;
                }
                catch
                {
                    // log and try the next one
                    Console.WriteLine("[INFO] A request failed, trying the next one...");
                    tasks.Remove(completedTask); 
                }
            }

            return "<div>ERROR: All requests failed.</div>";
        }


        private async Task<string> TryGetFilmAsync(
            int id,
            CancellationToken cancellationToken = default)
        {
            try
            {
                return await GetFilmAsync(id, cancellationToken);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Failed to get film {id}: {ex.Message}");
                return $"<div>Error loading film {id}</div>";
            }
        }

        private async Task<string> GetFilmAsync(
            int id,
            CancellationToken cancellationToken = default)
        {
            Console.WriteLine($"[REQUEST] films/{id}");

            var response = await _httpClient.GetAsync($"films/{id}", cancellationToken);

            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            return content;
        }
    }
}
