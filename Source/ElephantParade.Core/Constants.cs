namespace NHSD.ElephantParade.Core
{
    /// <summary>
    /// Class used to store commonly accessed fields. These will be used in queries across services, and
    /// may change in future - saving them here will mean only making the change in one place.
    /// </summary>
    public static class Constants
    {
        public const int EncountersInDuke = 12;
        public const int MaxCallbackAttempts = 1000;
        public const string CompletedOutcomeReason = "Final callback";
        public const string DeclinedOutcomeReason = "Patient wishing to withdraw from project";
        public const string ClosedOutcomeReason = "Closed";
        public const string ActionTextComment = "Added Comment: ";
        public const string ActionTextCallback = "Scheduled Callback: ";
        public const string ActionTextQuestion = "Called, with outcome: ";
        public const string DeclinedInappropriateQuestions = "Declined – Inappropriate questions";
        public const string No = "N";
        public const string Yes = "Y";
        public const string Locked = "Locked";
        public const string Unlocked = "Unlocked";
        public const string UpdatePhoneNumber = "Updated Telephone Number";
        public const string EncounterIncompleteText = "Encounter incomplete - rescheduled";
        public const string EncounterCompleteText = "Encounter complete";

        #region blood pressure warnings / warning levels
        public const int SystolicHighLevel = 179;
        public const int DiastolicHighLevel = 109;
        public const int SystolicLowLevel = 101;
        //public const string BloodPressureHighWarningText = "Please repeat your BP on two more occasions half an hour apart. If the readings remain at or above 180 (systolic) or 110 (diastolic) please contact your GP. If you have symptoms of headache, nosebleeds, visual symptoms, palpitations, or feel unwell, please contact your GP or NHS 111 immediately for advice and assessment. If you have none of these symptoms please contact your GP the same day for advice.";
        //public const string BloodPressureLowWarningText = "Please repeat your BP on two more occasions half an hour apart. If the readings remain at 100 or below (systolic) please contact your GP. If you have symptoms of chest pain, feeling faint or short of breath, or feel unwell please contact your GP or NHS 111 immediately for advice and assessment. If you have none of these symptoms please contact your GP the same day for advice.";
        //public const string BloodPressureHighWarningStatusText = "Over High Warning Threshold";
        //public const string BloodPressureLowWarningStatusText = "Under Low Warning Threshold";
        //public const string BloodPressureHighTargetStatusText = "Over Target Threshold";
        public const string ReadingDueAgainToday = "Please enter another reading later today";
        public const string ReadingDueTomorrow = "Please enter another reading tomorrow morning";
        public const string ReadingDueAfterCallFromAdvisor = "Your next reading will be due after you are called by a Healthlines Advisor";
        public const string ReadingDueOnSpecifiedDate = "The next reading is due on: "; //this is to be used on both advisor and patient areas, so needs to be generic - e.g. no "you" or "patient".
        public const string ReadingOverdue = "Reading overdue";
        public const string PatientSetDailyReadings = "The patient will now be expected to give a blood pressure reading twice a day (starting tomorrow)";
        public const string PatientSetWeeklyReadings = "The patient will now be expected to give a blood pressure reading once a week. ";
        //public const string ReadingMissed = "Reading missed";
        //public const string ReadingExpected = "Reading expected";
        public const string DateTimeBPError = "Date and time of reading not entered correctly";
        #endregion

        #region Text on blood pressure readings screen
        public const string DailyBPReadingsAverageText = "Average reading within the last six days";
        public const string WeeklyBPReadingsAverageText = "Average reading within the last six weeks";
        public const string NoBPReadingsAverageText = "No average reading as none were entered";
        public const string NoBPReadingsAverageForLessThan4ReadingsText = "No average reading as there were less than 4 readings entered";
        #endregion
    }
}
