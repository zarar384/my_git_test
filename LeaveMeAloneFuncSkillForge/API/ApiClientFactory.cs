using LeaveMeAloneFuncSkillForge.Interfaces;
using LeaveMeAloneFuncSkillForge.Models;
using System.Collections.Concurrent;

namespace LeaveMeAloneFuncSkillForge.API
{
    public sealed class ApiClientFactory : IApiClientFactory
    {
        // cache clients to avoid redundant initialization
        private readonly ConcurrentDictionary<string, Lazy<Task<IApiClient>>> _clients = new();

        // async factory method, it creates clients asynchronously and ensures full initialization
        public async Task<IApiClient> CreateClientAsync(string serviceName, CancellationToken ct = default)
        {
            // lazy initialization to ensure thread-safety and avoid redundant work
            var lazyClient = _clients.GetOrAdd(serviceName, name =>
                new Lazy<Task<IApiClient>>(
                    () => CreateAndInitializeClientAsync(name, ct),
                    LazyThreadSafetyMode.ExecutionAndPublication));

            try
            {
                var client = await lazyClient.Value;
                return client;
            }
            catch
            {
                _clients.TryRemove(serviceName, out _);
                throw;
            }
        }

        private async Task<IApiClient> CreateAndInitializeClientAsync(string serviceName, CancellationToken ct)
        {
            Console.WriteLine($"[DEBUG] Starting CreateAndInitializeClientAsync for {serviceName}");

            ApiConfiguration config;
            try
            {
                config = await LoadConfigurationAsync(serviceName, ct);
                Console.WriteLine($"[DEBUG] Loaded config for {serviceName}: {config.BaseUrl}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Failed to load configuration for {serviceName}: {ex}");
                throw;
            }

            // retry logic for transient failures
            // in production, this is usually implemented via Polly.
            // Try-catch + retry logic (Polly-style)
            for (int i = 0; i < 3; i++)
            {
                try
                {
                    Console.WriteLine($"[DEBUG] Attempt {i + 1} to create ApiClient for {serviceName}");
                    var client = await ApiClient.CreateAsync(config, ct);
                    Console.WriteLine($"[DEBUG] Client created successfully for {serviceName}");
                    return client;
                }
                catch (Exception ex) when (i < 2)
                {
                    Console.WriteLine($"[WARN] Retry {i + 1} for {serviceName} failed: {ex.Message}");
                    await Task.Delay(500, ct);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[ERROR] Final failure creating client for {serviceName}: {ex}");
                    throw; 
                }
            }

            throw new InvalidOperationException($"[FATAL] Failed to create API client for {serviceName} after multiple attempts.");
        }


        // simulated config loader from external source (Azure, etc.) 
        private async Task<ApiConfiguration> LoadConfigurationAsync(string serviceName, CancellationToken ct)
        {
            await Task.Delay(300, ct);
            return new ApiConfiguration
            {
                BaseUrl = $"https://api.{serviceName}.com",
                ApiKey = "I_HATE_MYSELF_123",
                TimeoutSeconds = 5
            };
        }
    }
}
