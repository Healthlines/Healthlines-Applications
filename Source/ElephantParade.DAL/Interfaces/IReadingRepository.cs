using System;
using System.Collections.Generic;
using NHSD.ElephantParade.DAL.EntityModels;

namespace NHSD.ElephantParade.DAL.Interfaces
{
    public interface IReadingRepository: IRepository<Reading>
    {
        IList<Reading> GetValidReadings(int readingTypeId, string patientId, string studyId, DateTime from, DateTime to);
        Reading GetMostRecentReading(int readingTypeId, string patientId, string studyId);
        IList<Reading> GetAllReadingsForPatient(string patientId, string studyId);
    }
}
