using System.Threading;
using System.Threading.Tasks;

namespace gmvTM.Domain.Workers.Interfaces
{
    public interface IDataSeederWorker : IWorker
    {
        public void Seed(); 
        public Task SeedAsync(CancellationToken cancellationToken = default);

        public void Reseed();
        public Task ReseedAsync(CancellationToken cancellationToken = default);
    }
}
