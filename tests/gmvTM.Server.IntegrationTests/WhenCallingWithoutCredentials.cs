using Xunit;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using gmvTestConstants;

namespace gmvTM.Server.IntegrationTests
{
    [Collection(nameof(ApiIntegrationCollection))]
    public sealed class WhenCallingWithoutCredentials : WithGmvTMApi
    {
        public WhenCallingWithoutCredentials(GmvTMWebApplicationFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task ItShouldRejectApiCallsWithoutBasicAuth()
        {
            using HttpClient anonymous = Factory.CreateClient();

            HttpResponseMessage response = await anonymous.GetAsync(gmvTest.Api.StopsForRouteF);

            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task ItShouldRejectODataCallsWithoutBasicAuth()
        {
            using HttpClient anonymous = Factory.CreateClient();

            HttpResponseMessage response = await anonymous.GetAsync(gmvTest.Api.ODataStopItems);

            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }
    }
}
