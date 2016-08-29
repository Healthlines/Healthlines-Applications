using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHSD.ElephantParade.DAL.Interfaces;
using NHSD.ElephantParade.DAL.EntityModels;
using NHSD.ElephantParade.DAL.Infrastructure;


namespace NHSD.ElephantParade.DAL.Repositories
{
    public class PatientMedicationRepository : RepositoryBase<PatientMedication>, IPatientMedicationRepository
    {
        public PatientMedicationRepository() : this(new HealthlinesRepositoryContext()) { }

        public PatientMedicationRepository(IRepositoryContext repositoryContext) : base(repositoryContext) { }
    }
}
