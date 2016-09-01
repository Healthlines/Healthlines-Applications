
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHSD.ElephantParade.DAL.Interfaces;
using NHSD.ElephantParade.DAL.EntityModels;
using NHSD.ElephantParade.DAL.Infrastructure;


namespace NHSD.ElephantParade.DAL.Repositories
{
    public class CallEventRepository : RepositoryBase<CallEvent>, ICallEventRepository
    {
        public CallEventRepository() : this(new HealthlinesRepositoryContext()) { }

        public CallEventRepository(IRepositoryContext repositoryContext) : base(repositoryContext) { }
    }
}
