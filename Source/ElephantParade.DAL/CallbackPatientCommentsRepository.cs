using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHSD.ElephantParade.DAL.Interfaces;
using NHSD.ElephantParade.DAL.EntityModels;
using NHSD.ElephantParade.DAL.Infrastructure;


namespace NHSD.ElephantParade.DAL.Repositories
{
    public class CallbackCommentsRepository : RepositoryBase<CallbackPatientComment>, ICallbackCommentsRepository
    {
        public CallbackCommentsRepository() : this(new HealthlinesRepositoryContext()) { }

        public CallbackCommentsRepository(IRepositoryContext repositoryContext) : base(repositoryContext) { }
    }
}
