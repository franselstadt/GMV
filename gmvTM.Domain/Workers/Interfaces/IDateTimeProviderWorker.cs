using System;
using System.Threading;
using System.Threading.Tasks;

namespace gmvTM.Domain.Workers.Interfaces
{
    public interface IDateTimeProviderWorker : IWorker
    {
        //maybe I use epoch in bigger system just a note

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
