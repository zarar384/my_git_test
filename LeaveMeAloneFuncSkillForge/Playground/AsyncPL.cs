using LeaveMeAloneFuncSkillForge.API;
using LeaveMeAloneFuncSkillForge.Interfaces;

namespace LeaveMeAloneFuncSkillForge.Playground
{
    public static class AsyncPL
    {
        public static async Task Run()
        {
            await TestApiClientFactoryAsync();
        }

        private static async Task TestApiClientFactoryAsync()
        {
            // DI-friendly factory design
            // factory implements IApiClientFactory
            // and can easily be injected where needed, for example in ASP.NET Core services
            // (services.AddSingleton<IApiClientFactory, ApiClientFactory>();)
            IApiClientFactory factory = new ApiClientFactory();

            // create and cache clients 
            var client1 = await factory.CreateClientAsync("testClient_1");
            Console.WriteLine(client1 != null ? "Client 1 created" : "Null client");

            var client2 = await factory.CreateClientAsync("testClient_2");
            Console.WriteLine(client2 != null ? "Client 2 created" : "Null client");

            var results = await client1.GetDataAsync("/data/endpoint1");
            Console.WriteLine(results);

            // reuse cached client (no re-initialization)
            var client1Cached = await factory.CreateClientAsync("testClient_1");
            Console.WriteLine(ReferenceEquals(client1, client1Cached));
        }

        private static async Task TestMockApiClientFactoryAsync()
        {
            // DI-friendly factory design
            // factory implements IApiClientFactory
            // and can easily be injected where needed, for example in ASP.NET Core services
            // (services.AddSingleton<IApiClientFactory, ApiClientFactory>();)
            // mock factory for testing
            Func<string, Task<IApiClient>> factory = MockApiClientFactory.GetClientAsync;

            // create and cache clients
            var client1 = await factory("testClient_1");
            Console.WriteLine(client1 != null ? "Client 1 created" : "Null client");

            var client2 = await factory("testClient_2");
            Console.WriteLine(client2 != null ? "Client 2 created" : "Null client");

            var results = await client1.GetDataAsync("/data/endpoint1");
            Console.WriteLine(results); // Fake data from testClient_1 for /data/endpoint1

            // reuse cached client (if you implement caching in your mock factory)
            var client1Cached = await factory("testClient_1");
            Console.WriteLine(ReferenceEquals(client1, client1Cached));
        }
    }
}
