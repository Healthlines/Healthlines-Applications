using System.Linq;
using NHSD.ElephantParade.DAL.EntityModels;
using NHSD.ElephantParade.DAL.Infrastructure;
using NHSD.ElephantParade.DAL.Interfaces;

namespace NHSD.ElephantParade.DAL
{
    public class ReadingTargetRepository : RepositoryBase<ReadingTarget>, IReadingTargetRepository
    {
        public ReadingTargetRepository() : this(new HealthlinesRepositoryContext()) { }

        public ReadingTargetRepository(IRepositoryContext repositoryContext) : base(repositoryContext) { }

        public ReadingTarget GetMostRecentPatientTarget(int readingTypeId, string patientId, string studyId)
        {
            return ObjectSet.Where(t => t.PatientId == patientId && 
                t.StudyId == studyId && 
                t.ReadingTypeId == readingTypeId && 
                t.Valid).OrderByDescending(t => t.DateEntered).FirstOrDefault();
        }
    }
}