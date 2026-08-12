using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using gmvTM.Domain;
using gmvTM.Domain.Collections.Interfaces;
using gmvTM.Domain.Workers.Interfaces;
using gmvTestConstants;

namespace gmvTM.Server.IntegrationTests
{
    public sealed class GmvTMWebApplicationFactory : WebApplicationFactory<Program>
    {
        private readonly string _dbPath = gmvTest.Database.NewDatabasePath();

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Development");
            builder.UseSetting(gmvDomain.Resources.ConnectionStringsDefaultSetting, gmvTest.Database.ConnectionString(_dbPath));

            builder.ConfigureTestServices(services =>
            {
                services.RemoveAll<IDateTimeProviderWorker>();
                services.AddSingleton<IDateTimeProviderWorker>(
                    new FixedDateTimeProviderWorker(gmvTest.Data.FixedTestClockUtc));
            });
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (!disposing) return;

            TryDelete(_dbPath);
            TryDelete(_dbPath + gmvTest.Database.SharedMemorySuffix);
            TryDelete(_dbPath + gmvTest.Database.WriteAheadLogSuffix);
        }

        private static void TryDelete(string path)
        {
            try
            {
                if (File.Exists(path))
                    File.Delete(path);
            }
            catch
            {
            }
        }
    }
}
