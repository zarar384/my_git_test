using LeaveMeAloneFuncSkillForge.Interfaces;
using System.Collections.Concurrent;

namespace LeaveMeAloneFuncSkillForge.API
{
    public static class MockApiClientFactory
    {
        private static readonly ConcurrentDictionary<string, Lazy<Task<IApiClient>>> _mockClients = new();

        public static Task<IApiClient> GetClientAsync(string serviceName)
        {
            var lazyClient = _mockClients.GetOrAdd(serviceName, name =>
                new Lazy<Task<IApiClient>>(() => CreateAsync(name), LazyThreadSafetyMode.ExecutionAndPublication));

            return lazyClient.Value;
        }

        private static Task<IApiClient> CreateAsync(string serviceName)
        {
            return Task.FromResult<IApiClient>(new MockApiClient(serviceName));
        }
    }

}
