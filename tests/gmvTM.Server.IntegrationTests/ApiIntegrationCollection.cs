using Xunit;

namespace gmvTM.Server.IntegrationTests
{
    [CollectionDefinition(nameof(ApiIntegrationCollection))]
    public sealed class ApiIntegrationCollection : ICollectionFixture<GmvTMWebApplicationFactory>
    {
    }
}
