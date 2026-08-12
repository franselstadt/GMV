using System;
using gmvTM.Domain.Collections.Interfaces;
using gmvTM.Domain.Workers.Interfaces;

namespace gmvTM.Server.IntegrationTests
{
    internal sealed class FixedDateTimeProviderWorker : IDateTimeProviderWorker
    {
        public FixedDateTimeProviderWorker(DateTime agencyNow)
        {
            AgencyNow = DateTime.SpecifyKind(agencyNow, DateTimeKind.Unspecified);
            UtcNow = DateTime.SpecifyKind(agencyNow, DateTimeKind.Utc);
        }

        public DateTime UtcNow { get; }
        public DateTime AgencyNow { get; }
    }
}
