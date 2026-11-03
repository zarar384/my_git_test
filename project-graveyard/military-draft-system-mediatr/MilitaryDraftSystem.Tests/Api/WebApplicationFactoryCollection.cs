namespace MilitaryDraftSystem.Tests.Api
{
    /// <summary>
    /// Test collection shared by all test classes that spin up the real API
    /// host via <c>WebApplicationFactory&lt;Program&gt;</c>. xUnit runs
    /// different collections in parallel by default, and concurrently
    /// building more than one generic host via
    /// <c>HostFactoryResolver</c> races on shared static state, causing
    /// spurious "entry point exited without ever building an IHost" failures.
    /// Keeping all host-based tests in one collection serializes them.
    /// </summary>
    [CollectionDefinition(Name)]
    public sealed class WebApplicationFactoryCollection : ICollectionFixture<object>
    {
        public const string Name = "WebApplicationFactory";
    }
}
