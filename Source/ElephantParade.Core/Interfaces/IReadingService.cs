using System;
using System.Collections.Generic;
using NHSD.ElephantParade.Core.Models;
using NHSD.ElephantParade.Domain.Models;

namespace NHSD.ElephantParade.Core.Interfaces
{
    public interface IReadingService
    {
        //void AddReading(Domain.Models.Reading reading);
        //Domain.Models.Reading GetPatientReading(ReadingTypes readingType, string patientId, string studyId);

        void AddReadingTarget(Domain.Models.ReadingTarget target);
        Domain.Models.ReadingTarget GetPatientTarget(ReadingTypes readingType, string patientId, string studyId);

        void GenerateReadingExpectations(string patientId, string studyId, ReadingTypes readingType, ReadingFrequency readingFrequency, DateTime startDate);
        ReadingFrequency GetReadingExpectedFrequency(string patientId, string studyId, ReadingTypes readingType);
        ReadingExpectedViewModel GetMostRecentExpectedReading(string patientId, string studyId, ReadingTypes readingType);

        void MarkReadingsAsInvalid(ReadingTypes readingType, string patientId, string studyId, DateTime from, DateTime to);

		string GetAverageReadingForTheLastSixDaysOrWeeks(ReadingFrequency readingFrequency, ReadingTypes readingType, string patientId, string studyId);
        string GetDateOfNextReading(string patientId, string studyId, ReadingTypes readingType, ReadingFrequency readingFrequency, DateTime dateOfReadingWithoutTime);
        bool PatientHasFutureExpectedReading(string patientId, string studyId, ReadingTypes readingType, DateTime dateOfReadingWithoutTime);

        #region Blood Pressure Specific
        IList<BloodPressureReadingViewModel> GetBPReadingsForPatientWithinDateRangeGiven(string patientId, string studyId, DateTime startDate, DateTime endDate);
        SubmittedBloodPressureViewModel AddReading(BloodPressureReadingViewModel reading);
        IList<BloodPressureReadingViewModel> GetBPReadingsForPatient(string patientId, string studyId);
        BloodPressureReadingViewModel SetStatusForReading(BloodPressureReadingViewModel reading);
        IList<BloodPressureReadingViewModel> GetBPReadingsEnteredForPatient(string patientId, string studyId);
        //string GetWarningMessageForReading(BloodPressureReadingViewModel reading);
        #endregion

        void DeleteAll(string patientId);
    }
}
