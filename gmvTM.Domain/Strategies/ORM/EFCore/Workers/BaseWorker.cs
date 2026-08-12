using System;
using gmvTM.Domain.Strategies.ORM.EFCore.Infrastructure;
using gmvTM.Domain.Workers.Interfaces;

namespace gmvTM.Domain.Strategies.ORM.EFCore.Workers
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
