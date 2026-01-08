using LeaveMeAloneFuncSkillForge.Interfaces;

namespace LeaveMeAloneFuncSkillForge.API
{
    public class MockApiClient : IApiClient
    {
        private static readonly Random _random = new();
        private readonly string _name;
        public MockApiClient(string name) => _name = name;

        public Task<string> GetData(string endpoint, CancellationToken ct = default)
        {
            return Task.FromResult($"Fake data from {_name}{endpoint}");
        }

        public async Task<string> GetDataAsync(string endpoint, CancellationToken ct = default)
        {
            int delayMs = _random.Next(500, 5000);

            Console.WriteLine($"[{_name}] Simulating delay {delayMs} ms");

            await Task.Delay(delayMs, ct);

            return $"Fake data from {_name}{endpoint}";
        }
    }

}
