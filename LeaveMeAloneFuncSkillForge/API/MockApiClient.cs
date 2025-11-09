using LeaveMeAloneFuncSkillForge.Interfaces;

namespace LeaveMeAloneFuncSkillForge.API
{
    public class MockApiClient : IApiClient
    {
        private readonly string _name;
        public MockApiClient(string name) => _name = name;

        public Task<string> GetDataAsync(string endpoint, CancellationToken ct = default)
        {
            return Task.FromResult($"Fake data from {_name}{endpoint}");
        }
    }

}
