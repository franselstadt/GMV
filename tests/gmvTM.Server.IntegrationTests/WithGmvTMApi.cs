using System.Net.Http;
using System.Net.Http.Headers;
using gmvTestConstants;

namespace gmvTM.Server.IntegrationTests
{
    public abstract class WithGmvTMApi
    {
        protected HttpClient Subject { get; }
        protected GmvTMWebApplicationFactory Factory { get; }

        protected WithGmvTMApi(GmvTMWebApplicationFactory factory)
        {
            Factory = factory;
            Subject = factory.CreateClient();
            Subject.DefaultRequestHeaders.Authorization = AuthenticationHeaderValue.Parse(gmvTest.Auth.BasicAuthHeader);
        }
    }
}
