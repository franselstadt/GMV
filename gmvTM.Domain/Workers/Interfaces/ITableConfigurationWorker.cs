using Microsoft.EntityFrameworkCore;

namespace gmvTM.Domain.Workers.Interfaces
{
    public interface ITableConfigurationWorker : IWorker
    {
        void Configure(ModelBuilder modelBuilder);
    }
}
