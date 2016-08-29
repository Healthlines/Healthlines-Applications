using System;
using System.Collections.Generic;
using System.Linq;
using NHSD.ElephantParade.DAL.EntityModels;
using NHSD.ElephantParade.DAL.Infrastructure;
using NHSD.ElephantParade.DAL.Interfaces;

namespace NHSD.ElephantParade.DAL
{
    public class ReadingRepository : RepositoryBase<Reading>, IReadingRepository
    {
        public ReadingRepository() : this(new HealthlinesRepositoryContext()) { }

        public ReadingRepository(IRepositoryContext repositoryContext) : base(repositoryContext) { }

        #region Implementation of IReadingRepository

        public Reading GetMostRecentReading(int readingTypeId, string patientId, string studyId)
        {
            return ObjectSet.Where(r => r.PatientId == patientId &&
                r.StudyId == studyId &&
                r.ReadingTypeId == readingTypeId &&
                r.Valid).OrderByDescending(r => r.DateOfReading).FirstOrDefault();
        }

        public IList<Reading> GetValidReadings(int readingTypeId, string patientId, string studyId, DateTime from, DateTime to)
        {
            return ObjectSet.Where(r => r.PatientId == patientId &&
                                        r.StudyId == studyId &&
                                        r.ReadingTypeId == readingTypeId &&
                                        r.Valid &&
                                        r.DateOfReading > from &&
                                        r.DateOfReading < to).ToList();
        }

        public IList<Reading> GetAllReadingsForPatient(string patientId, string studyId)
        {
            return ObjectSet.Where(r => r.PatientId == patientId &&
                r.StudyId == studyId).ToList();
        }

        #endregion
    }
}