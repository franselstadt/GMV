using System;
using gmvTM.Domain.Infrastructure.Persistence;
using gmvTM.Domain.Workers.Interfaces;

namespace gmvTM.Domain.Workers.Base
{
    public abstract class BaseWorker : IWorker
    {
        private readonly DatabaseContext context;

        protected DatabaseContext Context
        {
            get { return this.context; }
        }


        protected BaseWorker(DatabaseContext context)
        {
            this.context = context ?? throw new ArgumentNullException(nameof(context));
        }



       
    }
}
