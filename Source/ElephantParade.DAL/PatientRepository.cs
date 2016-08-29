using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHSD.ElephantParade.DAL.Interfaces;
using NHSD.ElephantParade.DAL.EntityModels;
using NHSD.ElephantParade.DAL.Infrastructure;


namespace NHSD.ElephantParade.DAL.Repositories
{
    public class PatientRepository : RepositoryBase<Patient>, IPatientRepository
    {
        public PatientRepository() : this(new HealthlinesRepositoryContext()) { }

        public PatientRepository(IRepositoryContext repositoryContext) : base(repositoryContext) { }
    }
}
