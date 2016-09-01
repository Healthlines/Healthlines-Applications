using System.Linq;
using NHSD.ElephantParade.DAL.EntityModels;
using NHSD.ElephantParade.DAL.Infrastructure;
using NHSD.ElephantParade.DAL.Interfaces;

namespace NHSD.ElephantParade.DAL.Repositories
{
    public class GpPracticeAddressRepository : RepositoryBase<GpPracticeAddress>, IGpPracticeAddressRepository
    {
        public GpPracticeAddressRepository() : this(new HealthlinesRepositoryContext()) { }

        public GpPracticeAddressRepository(IRepositoryContext repositoryContext) : base(repositoryContext) { }
    }
}
