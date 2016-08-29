using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHSD.ElephantParade.DAL.Interfaces;
using NHSD.ElephantParade.DAL.EntityModels;
using NHSD.ElephantParade.DAL.Infrastructure;

namespace NHSD.ElephantParade.DAL.Repositories
{
    public class CallbackRepository : RepositoryBase<Callback>, ICallbackRepository
    {
        public CallbackRepository() : this(new HealthlinesRepositoryContext()) { }

        public CallbackRepository(IRepositoryContext repositoryContext) : base(repositoryContext) { }

        public IQueryable<CallEvent> CallEvent_List()
        {
            return base.RepositoryContext.GetObjectSet<CallEvent>().AsQueryable();
        }


        public void CallEvent_Add(CallEvent ce)
        {
            base.RepositoryContext.GetObjectSet<CallEvent>().AddObject(ce);

        }
    }
}
