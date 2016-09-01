using NHSD.ElephantParade.DAL.EntityModels;
using NHSD.ElephantParade.DAL.Infrastructure;
using NHSD.ElephantParade.DAL.Interfaces;

namespace NHSD.ElephantParade.DAL
{
    public class PageVisitedLogRepository : RepositoryBase<PageVisitedLog>, IPageVisitedLogRepository
    {
        public PageVisitedLogRepository() : this(new HealthlinesRepositoryContext()) { }

        public PageVisitedLogRepository(IRepositoryContext repositoryContext) : base(repositoryContext) { }
    }
}
