using System;
using System.Collections.Generic;
using NHSD.ElephantParade.DAL.EntityModels;

namespace NHSD.ElephantParade.DAL.Interfaces
{
    public interface IReadingExpectedRepository : IRepository<ReadingExpected>
    {
        ReadingExpected GetNextReadingExpected(string patientId, string studyId, int readingTypeId);
        ReadingExpected GetMostRecentPastReadingExpected(string patientId, string studyId, int readingTypeId, DateTime date);
        IList<ReadingExpected> GetReadingsExpected(string patientId, string studyId, int readingTypeId);
    }
}
