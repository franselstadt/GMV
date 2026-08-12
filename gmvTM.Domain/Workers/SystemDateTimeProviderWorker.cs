using System;
using gmvTM.Domain.Workers.Interfaces;

namespace gmvTM.Domain.Workers
{

    //in a more complex system i would opt for epoch
    public sealed class SystemDateTimeProviderWorker : IDateTimeProviderWorker
    {
        private static readonly TimeZoneInfo Pacific = ResolvePacific();

        public DateTime UtcNow
        {
            get { return DateTime.UtcNow; }
        }

        public DateTime AgencyNow
        {
            get { return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, Pacific); }
        }

        private static TimeZoneInfo ResolvePacific()
        {
            string id = OperatingSystem.IsWindows() ? "Pacific Standard Time" : "America/Los_Angeles";
            return TimeZoneInfo.FindSystemTimeZoneById(id);
        }
    }
}
