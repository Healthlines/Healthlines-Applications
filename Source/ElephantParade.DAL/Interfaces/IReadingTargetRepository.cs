using NHSD.ElephantParade.DAL.EntityModels;

namespace NHSD.ElephantParade.DAL.Interfaces
{
    public interface IReadingTargetRepository : IRepository<ReadingTarget>
    {
        ReadingTarget GetMostRecentPatientTarget(int readingTypeId, string patientId, string studyId);
    }
}
