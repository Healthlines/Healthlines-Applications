using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using NHSD.ElephantParade.DAL.EntityModels;
using NHSD.ElephantParade.DAL.Infrastructure;
using NHSD.ElephantParade.DAL.Interfaces;

namespace NHSD.ElephantParade.DAL.Repositories
{
    public class ReadingExpectedRepository : RepositoryBase<ReadingExpected>, IReadingExpectedRepository
    {
        public ReadingExpectedRepository() : this(new HealthlinesRepositoryContext()) { }

        public ReadingExpectedRepository(IRepositoryContext repositoryContext) : base(repositoryContext) { }

        public ReadingExpected GetNextReadingExpected(string patientId, string studyId, int readingTypeId)
        {
            var query = RepositoryContext.GetObjectSet<ReadingExpected>().Where(re => re.PatientId == patientId &&
                                                                                       re.StudyId == studyId &&
                                                                                       re.ReadingTypeId == readingTypeId &&
                                                                                       !re.ReadingGiven);

            return query.OrderByDescending(re => re.ExpectedDate).FirstOrDefault();
        }


        /// <summary>
        /// Return the expected with the highest date in the past (or today). This may have been given or not given.
        /// </summary>
        public ReadingExpected GetMostRecentPastReadingExpected(string patientId, string studyId, int readingTypeId, DateTime date)
        {
            var query = RepositoryContext.GetObjectSet<ReadingExpected>().Where(re => re.PatientId == patientId &&
                                                                                       re.StudyId == studyId &&
                                                                                       re.ReadingTypeId == readingTypeId &&
                                                                                       re.ExpectedDate <= date);
            //get most recent expected reading (in the past)
            var readingExpectedDefault = query.OrderByDescending(re => re.ExpectedDate).FirstOrDefault();

            if (readingExpectedDefault == null) return null;

            //check that the most recent reading is not one of two expected readings with the same date.
            var duplicateCheck = RepositoryContext.GetObjectSet<ReadingExpected>().Where(re => re.PatientId == patientId &&
                                                                                       re.StudyId == studyId &&
                                                                                       re.ReadingTypeId == readingTypeId &&
                                                                                       re.ExpectedDate == readingExpectedDefault.ExpectedDate);

            if (duplicateCheck.Count() == 2)
            {
                //ensure that as there are two with the same date, we pull out one not given
                var readingsNotGiven = RepositoryContext.GetObjectSet<ReadingExpected>().Where(re => re.PatientId == patientId &&
                                                                                       re.StudyId == studyId &&
                                                                                       re.ReadingTypeId == readingTypeId &&
                                                                                       re.ExpectedDate == readingExpectedDefault.ExpectedDate &&
                                                                                       !re.ReadingGiven);
                if (readingsNotGiven.Count() > 0)
                {
                    //one (or more) of the readings has not been given. Return it as the result.
                    return readingsNotGiven.FirstOrDefault();
                }
                else
                {
                    //both readings have been given
                    return readingExpectedDefault;
                }
            }
            else
            {
                //there is only one reading with this date, for this patient in the database.
                return readingExpectedDefault;
            }
        }
    
        // Get list of expected readings
        public IList<ReadingExpected> GetReadingsExpected(string patientId, string studyId, int readingTypeId)
        {
            var query = RepositoryContext.GetObjectSet<ReadingExpected>().Where(re => re.PatientId == patientId &&
                                                                                       re.StudyId == studyId);

            return query.OrderByDescending(re => re.ExpectedDate).ToList();
        }
    }
}
