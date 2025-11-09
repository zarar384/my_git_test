using LeaveMeAloneFuncSkillForge.Interfaces;
using LeaveMeAloneFuncSkillForge.Models;

namespace LeaveMeAloneFuncSkillForge.API
{
    internal sealed class ApiClient : IApiClient, IAsyncDisposable
    {
        private readonly ApiConfiguration _config;
        private readonly HttpClient _httpClient;
        private string _token = string.Empty;

        private ApiClient(ApiConfiguration config)
        {
            _config = config;
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri(_config.BaseUrl),
                Timeout = TimeSpan.FromSeconds(_config.TimeoutSeconds)
            };
        }

        // perform async initialization here
        // because constructors can't be async (cannot use 'await' in constructors)
        public static async Task<ApiClient> CreateAsync(ApiConfiguration config, CancellationToken ct = default)
        {
            var client = new ApiClient(config);
            await client.InitAsync(ct);
            return client;
        }

        private async Task InitAsync(CancellationToken ct = default)
        {
            // simulate async initialization, e.g., fetching an auth token
            await Task.Delay(100, ct);
            _token = await FetchAuthTokenAsync(ct);

            // load config from remote service (asyn init logic) if needed
            await Task.Delay(200, ct);  
        }

        private async Task<string> FetchAuthTokenAsync(CancellationToken ct = default)
        {
            // simulate fetching an auth token
            await Task.Delay(100, ct);
            return Guid.NewGuid().ToString("N");
        }

        public ValueTask DisposeAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<string> GetDataAsync(string endpoint, CancellationToken ct = default)
        {
            // real HTTP call example (commented out for this example)
            //using var request = new HttpRequestMessage(HttpMethod.Get, endpoint);
            //request.Headers.Add("Authorization", $"Bearer {_token}");
            //var response = await _httpClient.SendAsync(request, ct);
            //response.EnsureSuccessStatusCode();
            //var json = await response.Content.ReadAsStringAsync(ct);
            //return json;

            // simulate data fetching
            await Task.Delay(50, ct); 
            return $"Fake data from {_config.BaseUrl} for {endpoint}";
        }
    }
}
