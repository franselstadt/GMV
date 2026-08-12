using System;
using System.Threading;
using System.Threading.Tasks;

namespace gmvTM.Domain.Workers.Interfaces
{
    public interface IDateTimeProviderWorker : IWorker
    {
        //maybe use epoch...

        DateTime UtcNow
        {
            get;
        }

        DateTime AgencyNow
        {
            get;
        }
    }
}
