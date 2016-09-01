using System;
using System.Collections.Generic;
using System.Linq;
using NHSD.ElephantParade.Core.Interfaces;
using NHSD.ElephantParade.Core.Mapping;
using NHSD.ElephantParade.Core.Models;
using NHSD.ElephantParade.DAL;
using NHSD.ElephantParade.DAL.EntityModels;
using NHSD.ElephantParade.DAL.Interfaces;
using NHSD.ElephantParade.DAL.Repositories;
using NHSD.ElephantParade.Domain.Models;
using Reading = NHSD.ElephantParade.DAL.EntityModels.Reading;

namespace NHSD.ElephantParade.Core
{
    public class ReadingService : IReadingService
    {
        private IReadingRepository _readingRepository;
        private IReadingTargetRepository _readingTargetRepository;
        private IReadingExpectedRepository _readingExpectedRepository;

        public ReadingService() :
            this(new ReadingRepository(),
            new ReadingTargetRepository(),
            new ReadingExpectedRepository()) { }

        public ReadingService(IReadingRepository readingRepository, IReadingTargetRepository readingTargetRepository, IReadingExpectedRepository readingExpectedRepository)
        {
            _readingRepository = readingRepository ?? new ReadingRepository();
            _readingTargetRepository = readingTargetRepository ?? new ReadingTargetRepository();
            _readingExpectedRepository = readingExpectedRepository ?? new ReadingExpectedRepository();
        }

        #region Implementation of IReadingService

        //public void AddReading(Domain.Models.Reading reading)
        //{
        //    ConvertViewModelObjects cvmo = new ConvertViewModelObjects();

        //    var readingEntity = cvmo.ConvertFromReadingVM(reading);

        //    _readingRepository.Add(readingEntity);
        //    _readingRepository.SaveChanges();
        //}

        public SubmittedBloodPressureViewModel AddReading(BloodPressureReadingViewModel reading)
        {
            if (reading == null || reading.StudyId == null || reading.PatientId == null || reading.DateOfReading == null)
            {
                throw new ArgumentNullException("reading", "Reading or Patient details not correctly provided");
            }

            //convert blood pressure reading into entities
            Reading readingHighEntity = new Reading
            {
                DateSubmitted = DateTime.Now,
                DateOfReading = reading.DateOfReading.Value,
                PatientId = reading.PatientId,
                ReadingEntered = reading.Systolic.ToString(),
                ReadingTypeId = (int)ReadingTypes.SystolicBP,
                StudyId = reading.StudyId,
                SubmittedBy = reading.SubmittedBy,
                Valid = true
            };

            Reading readingLowEntity = new Reading
            {
                DateSubmitted = DateTime.Now,
                DateOfReading = reading.DateOfReading.Value,
                PatientId = reading.PatientId,
                ReadingEntered = reading.Diastolic.ToString(),
                ReadingTypeId = (int)ReadingTypes.DiastolicBP,
                StudyId = reading.StudyId,
                SubmittedBy = reading.SubmittedBy,
                Valid = true
            };

            //commit readings to db
            _readingRepository.Add(readingHighEntity);
            _readingRepository.Add(readingLowEntity);
            _readingRepository.SaveChanges();

            ReadingFrequency readingFrequency = GetReadingExpectedFrequency(reading.PatientId, reading.StudyId, ReadingTypes.BloodPressure);
            DateTime dateOfReadingWithoutTime = reading.DateOfReading.Value.Date; //remove time part from this given reading (make it 00:00:00) 

            if (readingFrequency == ReadingFrequency.NeverGivenReading)
            {
                GenerateReadingExpectations(reading.PatientId, reading.StudyId, ReadingTypes.BloodPressure, readingFrequency, dateOfReadingWithoutTime);
                MarkReadingAsGiven(reading.PatientId, reading.StudyId, ReadingTypes.BloodPressure, dateOfReadingWithoutTime);
            }
            else if (readingFrequency == ReadingFrequency.OnceWeekly)
            {
                MarkReadingAsGiven(reading.PatientId, reading.StudyId, ReadingTypes.BloodPressure, dateOfReadingWithoutTime);
                
                //only generate a new expectation if they've not got a future expected date
                //this will handle any 'extra' readings, which were not expected.
                var futureAlreadySetUp = PatientHasFutureExpectedReading(reading.PatientId, reading.StudyId, ReadingTypes.BloodPressure, dateOfReadingWithoutTime);
                if (!futureAlreadySetUp)
                GenerateReadingExpectations(reading.PatientId, reading.StudyId, ReadingTypes.BloodPressure, readingFrequency, dateOfReadingWithoutTime);
            }
            else if (readingFrequency == ReadingFrequency.TwiceDaily)
            {
                MarkReadingAsGiven(reading.PatientId, reading.StudyId, ReadingTypes.BloodPressure, dateOfReadingWithoutTime);
            }

            //set target for BP reading
            //get the targets 
            var targetDates = _readingTargetRepository.GetQueryable()
                                    .Where(r => r.PatientId == reading.PatientId && r.StudyId == reading.StudyId && (r.ReadingTypeId == (int)ReadingTypes.DiastolicBP || r.ReadingTypeId == (int)ReadingTypes.SystolicBP))
                                    .Select(r => new
                                    {
                                        ReadingType = (ReadingTypes)r.ReadingTypeId,
                                        r.Target,
                                        r.DateEntered
                                    }).ToList();

            var dReading = targetDates.Where(t => t.DateEntered <= reading.DateOfReading && t.ReadingType == ReadingTypes.DiastolicBP).OrderByDescending(t => t.DateEntered).FirstOrDefault();
            var sReading = targetDates.Where(t => t.DateEntered <= reading.DateOfReading && t.ReadingType == ReadingTypes.SystolicBP).OrderByDescending(t => t.DateEntered).FirstOrDefault();
                
            if (dReading != null && sReading != null)
            {
                //set target
                reading.Target = new BloodPressureTargetViewModel()
                {
                    DiastolicTarget = dReading.Target,
                    SystolicTarget = sReading.Target
                };
            }

            //populate status of blood pressure reading.
            reading = SetStatusForReading(reading);
            
            //get data to send to confirmation page
            SubmittedBloodPressureViewModel submittedBloodPressureViewModel = new SubmittedBloodPressureViewModel
            {
                DateOfNextReading = GetDateOfNextReading(reading.PatientId, reading.StudyId, ReadingTypes.BloodPressure, readingFrequency, dateOfReadingWithoutTime), 
                Readings = reading, 
                Targets = GetPatientBloodPressureTargets(reading.PatientId, reading.StudyId), 
                //Warning = GetWarningMessageForReading(reading)
            };

            return submittedBloodPressureViewModel;
        }

        /// <summary>
        /// Mark most recent (past or today) reading as given. If this reading is not expected, then no expectation will
        /// be marked as given.
        /// </summary>
        /// <param name="patientId">Identifier for patient</param>
        /// <param name="studyId">Identifier for study</param>
        /// <param name="readingType">Type of reading expected</param>
        /// <param name="dateOfReadingWithoutTime">Date reading taken</param>
        private void MarkReadingAsGiven(string patientId, string studyId, ReadingTypes readingType, DateTime dateOfReadingWithoutTime)
        {
            var expected = _readingExpectedRepository.GetMostRecentPastReadingExpected(patientId, studyId, (int)readingType, dateOfReadingWithoutTime);

            if (expected == null)
            {
                //do nothing, this is an extra reading we don't have an expectation for.
            }
            else if (!expected.ReadingGiven)
            {
                //mark expected reading as being given
                expected.ReadingGiven = true;
                _readingExpectedRepository.SaveChanges();
            }
        }

        /// <summary>
        /// Get the most recent expected reading for this patient which is not in the future.
        /// This may have been given, or may be overdue
        /// </summary>
        public ReadingExpectedViewModel GetMostRecentExpectedReading(string patientId, string studyId, ReadingTypes readingType)
        {
            var expected = _readingExpectedRepository.GetMostRecentPastReadingExpected(patientId, studyId, (int)readingType, DateTime.Now.Date);
            if (expected == null)
                return new ReadingExpectedViewModel();

            ConvertViewModelObjects cvmo = new ConvertViewModelObjects();
            return cvmo.ConvertFromReadingExpected(expected);
        }

        public bool PatientHasFutureExpectedReading(string patientId, string studyId, ReadingTypes readingType, DateTime dateOfReadingWithoutTime)
        {
            var expected = _readingExpectedRepository.GetNextReadingExpected(patientId, studyId, (int)readingType);

            if (expected == null)
                return false;

            if (expected.ExpectedDate > dateOfReadingWithoutTime)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public string GetDateOfNextReading(string patientId, string studyId, ReadingTypes readingType, ReadingFrequency readingFrequency, DateTime dateOfReadingWithoutTime)
        {
            var expected = _readingExpectedRepository.GetNextReadingExpected(patientId, studyId, (int)readingType);
            
            if (expected == null)
            {
                return Constants.ReadingDueAfterCallFromAdvisor;
            }
            else if (readingFrequency== ReadingFrequency.TwiceDaily)
            {
                if (dateOfReadingWithoutTime == expected.ExpectedDate)
                {
                    return Constants.ReadingDueAgainToday;
                }
                else
                {
                    return Constants.ReadingDueTomorrow;
                }
            }
            else if (readingFrequency == ReadingFrequency.NeverGivenReading)
            {
                return Constants.ReadingDueAgainToday;
            }
            else if (readingFrequency == ReadingFrequency.OnceWeekly)
            {
                return Constants.ReadingDueOnSpecifiedDate + expected.ExpectedDate.ToShortDateString();
            }

            //if in doubt, return message that no readings are set up.
            return Constants.ReadingDueAfterCallFromAdvisor;
        }

        public void GenerateReadingExpectations(string patientId, string studyId, ReadingTypes readingType, ReadingFrequency readingFrequency, DateTime startDate)
        {
            if (readingType == ReadingTypes.BloodPressure)
            {
                if (readingFrequency == ReadingFrequency.NeverGivenReading || readingFrequency == ReadingFrequency.TwiceDaily)
                {
                    //generate 14 records (two for each day) starting with date given.
                    var workingDate = startDate;
                    int readingTypeId = (int)readingType;
                    
                    for (int i = 0; i < 7; i++)
                    {
                        ReadingExpected re = new ReadingExpected
                        {
                            ExpectedDate = workingDate,
                            PatientId = patientId,
                            ReadingGiven = false,
                            ReadingTypeId = readingTypeId,
                            StudyId = studyId
                        };

                        //make second identical object
                        ReadingExpected reTwo = new ReadingExpected
                        {
                            ExpectedDate = re.ExpectedDate,
                            PatientId = re.PatientId,
                            ReadingGiven = re.ReadingGiven,
                            ReadingTypeId = re.ReadingTypeId,
                            StudyId = re.StudyId
                        };

                        _readingExpectedRepository.Add(re);
                        _readingExpectedRepository.Add(reTwo);

                        workingDate = workingDate.AddDays(1);
                    }

                    _readingExpectedRepository.SaveChanges();
                }
                else if (readingFrequency == ReadingFrequency.OnceWeekly)
                {
                    //set date of next reading
                    DateTime expectedDate = startDate.AddDays(7);

                    //check the patient doesn't already have an expected reading set up on this day.
                    var listOfExpectedReadings = _readingExpectedRepository.GetReadingsExpected(patientId, studyId, (int)ReadingTypes.BloodPressure);
                    var readingExpected = listOfExpectedReadings.Where(re => re.ExpectedDate == expectedDate).FirstOrDefault();

                    if (readingExpected == null)
                    {
                        //generate 1 record, 7 days in the future.
                        ReadingExpected re = new ReadingExpected();
                        re.ExpectedDate = expectedDate;
                        re.PatientId = patientId;
                        re.ReadingGiven = false;
                        re.ReadingTypeId = (int) readingType;
                        re.StudyId = studyId;

                        _readingExpectedRepository.Add(re);
                        _readingExpectedRepository.SaveChanges();
                    }
                }
            }
        }

        public void AddReadingTarget(Domain.Models.ReadingTarget target)
        {
            ConvertViewModelObjects cvmo = new ConvertViewModelObjects();

            var targetEntity = cvmo.ConvertFromReadingTargetVM(target);

            _readingTargetRepository.Add(targetEntity);
            _readingTargetRepository.SaveChanges();
        }

        //public Domain.Models.Reading GetPatientReading(Domain.Models.ReadingTypes readingType, string patientId, string studyId)
        //{
        //    ConvertViewModelObjects cvmo = new ConvertViewModelObjects();

        //    var reading = _readingRepository.GetMostRecentReading((int)readingType, patientId, studyId);

        //    return cvmo.ConvertFromReading(reading);
        //}

        public Domain.Models.ReadingTarget GetPatientTarget(Domain.Models.ReadingTypes readingType, string patientId, string studyId)
        {
            ConvertViewModelObjects cvmo = new ConvertViewModelObjects();

            var target = _readingTargetRepository.GetMostRecentPatientTarget((int)readingType, patientId, studyId);

            return cvmo.ConvertFromReadingTarget(target);
        }

        public BloodPressureTargetViewModel GetPatientBloodPressureTargets(string patientId, string studyId)
        {
            BloodPressureTargetViewModel bptvm = new BloodPressureTargetViewModel();

            var diastolic = GetPatientTarget(ReadingTypes.DiastolicBP, patientId, studyId);
            if (diastolic != null)
            {
                bptvm.DiastolicTarget = diastolic.Target;
            }
            
            var systolic = GetPatientTarget(ReadingTypes.SystolicBP, patientId, studyId);
            if (systolic != null)
            {
                bptvm.SystolicTarget = systolic.Target;
            }

            return bptvm;
        }

        public BloodPressureReadingViewModel SetStatusForReading(BloodPressureReadingViewModel reading)
        {
            if (reading.Target == null)
            {
                //ensure that warnings are still displayed
                if (Constants.SystolicHighLevel < reading.Systolic)
                {
                    reading.BPStatus = BPStatus.AboveCriticalLimit;
                    return reading;
                }

                if (Constants.DiastolicHighLevel < reading.Diastolic)
                {
                    reading.BPStatus = BPStatus.AboveCriticalLimit;
                    return reading;
                }

                if (Constants.SystolicLowLevel > reading.Systolic)
                {
                    reading.BPStatus = BPStatus.BelowCriticalLimit;
                    return reading;
                }

                return reading; //we cannot compare against targets, as none are setup
            }

            int systolicTarget = int.Parse(reading.Target.SystolicTarget);
            int diastolicTarget = int.Parse(reading.Target.DiastolicTarget);
            
            //first check for "dangerous" levels in BP (same for all patients)
            if (Constants.SystolicHighLevel < reading.Systolic && systolicTarget < reading.Systolic)
            {
                reading.BPStatus = BPStatus.AboveCriticalLimit;
                return reading;
            }

            if (Constants.DiastolicHighLevel < reading.Diastolic && diastolicTarget < reading.Diastolic)
            {
                reading.BPStatus = BPStatus.AboveCriticalLimit;
                return reading;
            }

            if (Constants.SystolicLowLevel > reading.Systolic && systolicTarget > reading.Systolic)
            {
                reading.BPStatus = BPStatus.BelowCriticalLimit;
                return reading;
            }

            //then check for "target" levels
            if (systolicTarget <= reading.Systolic)
            {
                reading.BPStatus = BPStatus.AboveTarget;
                return reading;
            }

            if (diastolicTarget <= reading.Diastolic)
            {
                reading.BPStatus = BPStatus.AboveTarget;
                return reading;
            }

            //has not hit limits, and is not over target
            reading.BPStatus = BPStatus.WithinTarget;
            return reading;
        }

        public ReadingFrequency GetReadingExpectedFrequency(string patientId, string studyId, ReadingTypes readingType)
        {
            //get two records with highest date for this patient
            var readingsExpected = _readingExpectedRepository.GetAll(e => e.PatientId == patientId 
                                                                    && e.StudyId == studyId
                                                                    && e.ReadingTypeId == (int)readingType)
                                                                    .OrderByDescending(e => e.ExpectedDate).Take(2).ToList();

            if (readingsExpected.Count != 2)
            {
                //patient has not been set up, therefore they have never given their first reading.
                return ReadingFrequency.NeverGivenReading;
            }
            else
            {
                DateTime one = readingsExpected[0].ExpectedDate;
                DateTime two = readingsExpected[1].ExpectedDate;

                //if the two expectations fall on the same day, then the patient is giving twice daily readings.
                return one == two ? ReadingFrequency.TwiceDaily : ReadingFrequency.OnceWeekly;
            }
        }

        public void MarkReadingsAsInvalid(Domain.Models.ReadingTypes readingType, string patientId, string studyId, DateTime from, DateTime to)
        {
            var readings = _readingRepository.GetValidReadings((int)readingType, patientId, studyId, from, to);

            foreach (var reading in readings)
            {
                reading.Valid = false;
            }

            _readingRepository.SaveChanges();
        }
        
        /// <summary>
        /// Convert a list of blood pressure readings taking into account each is either low or high reading
        /// </summary>
        /// <param name="readingType"></param>
        /// <param name="patientId"></param>
        /// <param name="studyId"></param>
        /// <returns></returns>
        public IList<BloodPressureReadingViewModel> GetBPReadingsEnteredForPatient(string patientId, string studyId)
        {
            ConvertViewModelObjects cvmo = new ConvertViewModelObjects();

            IList<BloodPressureReadingViewModel> BPHighAndLowReadings = new List<BloodPressureReadingViewModel>();

            var patientBPreadings = _readingRepository.GetAllReadingsForPatient(patientId, studyId);

            var highReadings = patientBPreadings.Where(r => r.ReadingTypeId == (int)ReadingTypes.SystolicBP).OrderByDescending(r => r.DateOfReading).ToList();
            var lowReadings = patientBPreadings.Where(r => r.ReadingTypeId == (int)ReadingTypes.DiastolicBP).OrderByDescending(r => r.DateOfReading).ToList();

            if (highReadings.Count != 0 && highReadings.Count == lowReadings.Count)
            {
                // Get list of high BP readings
                foreach (var highReading in highReadings)
                {
                    BPHighAndLowReadings.Add(cvmo.ConvertFromBPHighReading(highReading));
                }

                int indexCounter = 0;
                // Add list of low BP readings to BPHighAndLowReadings
                foreach (var lowReading in lowReadings)
                {
                    BPHighAndLowReadings[indexCounter].Diastolic = int.Parse(lowReading.ReadingEntered);

                    indexCounter++;
                }

                //set targets for each BP reading

                //get the targets 
                var targetDates = _readingTargetRepository.GetQueryable()
                                        .Where(r => r.PatientId == patientId && r.StudyId == studyId && (r.ReadingTypeId == (int)ReadingTypes.DiastolicBP || r.ReadingTypeId == (int)ReadingTypes.SystolicBP))
                                        .Select(r => new
                                        {
                                            ReadingType = (ReadingTypes)r.ReadingTypeId,
                                            r.Target,
                                            r.DateEntered
                                        })
                                        .ToList();
                foreach (var item in BPHighAndLowReadings)
                {
                    var dReading = targetDates.Where(t => t.DateEntered <= item.DateOfReading && t.ReadingType == ReadingTypes.DiastolicBP).OrderByDescending(t => t.DateEntered).FirstOrDefault();
                    var sReading = targetDates.Where(t => t.DateEntered <= item.DateOfReading && t.ReadingType == ReadingTypes.SystolicBP).OrderByDescending(t => t.DateEntered).FirstOrDefault();
                    if (dReading != null && sReading != null)
                    {
                        item.Target = new BloodPressureTargetViewModel()
                        {
                            DiastolicTarget = dReading.Target,
                            SystolicTarget = sReading.Target
                        };
                    }
                }

                // Loop round the list of BP readings that will consist of high and low readings,
                // by appending low readings to the list that already consist of high readings
                foreach (var highAndLowReading in BPHighAndLowReadings)
                {
                    //populate string of both systolic and diastolic readings
                    highAndLowReading.BPHighAndLowReading = string.Format("{0}/{1}", highAndLowReading.Systolic, highAndLowReading.Diastolic);

                    SetStatusForReading(highAndLowReading);
                }
            }

            return BPHighAndLowReadings.ToList();
        }

        /// <summary>
        /// Convert a list of blood pressure readings taking into account each is either low or high reading
        /// </summary>
        /// <param name="readingType"></param>
        /// <param name="patientId"></param>
        /// <param name="studyId"></param>
        /// <returns></returns>
        public IList<BloodPressureReadingViewModel> GetBPReadingsForPatient(string patientId, string studyId)
        {
            ConvertViewModelObjects cvmo = new ConvertViewModelObjects();

            IList<BloodPressureReadingViewModel> BPHighAndLowReadings = new List<BloodPressureReadingViewModel>();

            BPHighAndLowReadings = GetBPReadingsEnteredForPatient(patientId, studyId);

            // Add the list of future expected reading dates as well
            var readingsExpected = _readingExpectedRepository.GetReadingsExpected(patientId, studyId, (int)ReadingTypes.BloodPressure);

            foreach (var readingExpected in readingsExpected)
            {
                if (!readingExpected.ReadingGiven)
                {
                    var expectedBloodPressureReading = cvmo.ConvertFromReadingExpectedToBloodPressureViewModel(readingExpected);

                    // Add 'missed readings' status to the record
                    if (expectedBloodPressureReading.DateOfReading < DateTime.Today)
                    {
                        expectedBloodPressureReading.BPStatus =  BPStatus.Missed;
                    }
                    else
                    {
                        expectedBloodPressureReading.BPStatus = BPStatus.ReadingExpected;
                    }

                    BPHighAndLowReadings.Add(expectedBloodPressureReading);
                }
            }

            //Stuart - dont think the below is need so removed for now
            //// If expected reading date is before today then set overdue reading status for the last set of readings
            //var overdueReading = readingsExpected.Where(re => re.ExpectedDate < DateTime.Today).FirstOrDefault();

            //if (overdueReading != null && !overdueReading.ReadingGiven)
            //{
            //    var overdueReadingVM = BPHighAndLowReadings.Where(r => r.DateOfReading == overdueReading.ExpectedDate);
            //    overdueReadingVM.FirstOrDefault().BPStatus = Constants.ReadingOverdue;
            //}
            
            //get the targets 
            var targetDates = _readingTargetRepository.GetQueryable()
                                    .Where(r => r.PatientId == patientId && r.StudyId == studyId && (r.ReadingTypeId == (int) ReadingTypes.DiastolicBP || r.ReadingTypeId == (int) ReadingTypes.SystolicBP))                                  
                                    .Select(r=> new {
                                        ReadingType = (ReadingTypes)r.ReadingTypeId,
                                        r.Target,
                                        r.DateEntered
                                    })
                                    .ToList();

            //var mTargetDates = targetDates.Select(r => cvmo.ConvertFromReadingTarget(r)).ToList();

            foreach (var item in BPHighAndLowReadings)
            {
                var dReading = targetDates.Where(t => t.DateEntered <= item.DateOfReading && t.ReadingType == ReadingTypes.DiastolicBP).OrderByDescending(t => t.DateEntered).FirstOrDefault();
                var sReading = targetDates.Where(t => t.DateEntered <= item.DateOfReading && t.ReadingType == ReadingTypes.SystolicBP).OrderByDescending(t => t.DateEntered).FirstOrDefault();
                if(dReading!=null && sReading!=null)
                {
                    item.Target = new BloodPressureTargetViewModel()
                    {                    
                        DiastolicTarget = dReading.Target,
                        SystolicTarget = sReading.Target
                    };
                }
            }
            return BPHighAndLowReadings.OrderBy(r=>r.DateOfReading).ToList();
        }

        /// <summary>
        /// Get a count of the number of readings given - either in the last six days or six weeks depending on readingFrequency.
        /// </summary>
        private int CountAverageReadingForTheLastSixDaysOrWeeks(ReadingFrequency readingFrequency, ReadingTypes readingType, string patientId, string studyId)
        {
            switch (readingFrequency)
            {
                case ReadingFrequency.TwiceDaily:
                    {
                        // Take readings within the last 6 days
                        var twiceDailyReadings = _readingRepository.GetValidReadings((int) readingType, patientId, studyId, DateTime.Today.AddDays(-6), DateTime.Now);

                        return twiceDailyReadings.Count();
                    }
                case ReadingFrequency.OnceWeekly:
                    {
                        // Take readings within the last 42 days (i.e. in the last six weeks)
                        var onceWeeklyReadings = _readingRepository.GetValidReadings((int) readingType, patientId, studyId, DateTime.Today.AddDays(-42), DateTime.Now);

                        return onceWeeklyReadings.Count();
                    }
                default:
                    return 0;
            }
        }

        /// <summary>
        /// Get the average reading for the last six days or weeks
        /// </summary>
        /// <param name="readingFrequency">How often is the patient due to give readings</param>
        /// <param name="readingType">Which type of reading is the patient expected to give</param>
        /// <param name="patientId">Identifier for patient in question</param>
        /// <param name="studyId">Identifier for </param>
        /// <returns>String of the average readings for this patient (empty if not enough valid readings)</returns>
        public string GetAverageReadingForTheLastSixDaysOrWeeks(ReadingFrequency readingFrequency, ReadingTypes readingType, string patientId, string studyId)
        {
            IList<Reading> dailyOrWeeklyReadings = new List<Reading>();

            //get number of valid readings for average. We can only calculate average if we have more than four readings.
            var count = CountAverageReadingForTheLastSixDaysOrWeeks(readingFrequency, readingType, patientId, studyId);
            if (count < 4)
                return string.Empty;

            switch (readingFrequency)
            {
                case ReadingFrequency.TwiceDaily:
                    {
                        //Get readings within the last 6 days
                        dailyOrWeeklyReadings = _readingRepository.GetValidReadings((int)readingType, patientId, studyId, DateTime.Today.AddDays(-6), DateTime.Now);
                    }
                    break;
                case ReadingFrequency.OnceWeekly:
                    {
                        //Get readings within the last 42 days (i.e. in the last six weeks)
                        dailyOrWeeklyReadings = _readingRepository.GetValidReadings((int)readingType, patientId, studyId, DateTime.Today.AddDays(-42), DateTime.Now);
                    }
                    break;
                default:
                    return string.Empty;
            }

            // calculate the average for the readings
            try
            {
                double total = dailyOrWeeklyReadings.Sum(reading => double.Parse(reading.ReadingEntered));

                if (total > 0)
                {
                    return Math.Truncate(total / dailyOrWeeklyReadings.Count).ToString();
                }
                else
                {
                    return string.Empty;
                }
            }
            catch (FormatException)
            {
                return "There was an error for the calculation of the blood presssure reading average.";
            }
            catch (OverflowException)
            {
                return "There was an error for the calculation of the blood presssure reading average.";
            }
        }

        /// <summary>
        /// Gets the BP readings for patient within the date range that is set by the user
        /// </summary>
        /// <param name="patientId"></param>
        /// <param name="studyId"></param>
        /// <returns></returns>
        public IList<BloodPressureReadingViewModel> GetBPReadingsForPatientWithinDateRangeGiven(string patientId, string studyId, DateTime BPStartDateToSendToGP, DateTime BPEndDateToSendToGP)
        {
            ConvertViewModelObjects cvmo = new ConvertViewModelObjects();

            var BPReadingsForPatient = GetBPReadingsForPatient(patientId, studyId);
            var BPReadingsForPatientInDateRangeGiven = 
                                BPReadingsForPatient.Where(bpr => bpr.DateOfReading.Value.Date 
                                >= BPStartDateToSendToGP.Date && bpr.DateOfReading.Value.Date <= BPEndDateToSendToGP.Date).ToList();

            IList<BloodPressureReadingViewModel> BPHighAndLowReadingsForPatient = new List<BloodPressureReadingViewModel>();

            foreach (var BPReadingForPatient in BPReadingsForPatientInDateRangeGiven)
            {
                BPHighAndLowReadingsForPatient.Add(BPReadingForPatient);
            }

            return BPReadingsForPatientInDateRangeGiven;
        }

        #endregion


        public void DeleteAll(string patientId)
        {
            var readingsE =
                _readingExpectedRepository.GetQueryable()
                                          .Where(r => r.PatientId == patientId)
                                          .ToList();
            foreach (var readingExpected in readingsE)
            {
                _readingExpectedRepository.Delete(readingExpected);
            }
            _readingExpectedRepository.SaveChanges();

            var readings = _readingRepository.GetQueryable().Where(r => r.PatientId == patientId).ToList();
            foreach (var reading in readings)
            {
                _readingRepository.Delete(reading);
            }
            _readingRepository.SaveChanges();

            var readingTs = _readingTargetRepository.GetQueryable().Where(r => r.PatientId == patientId).ToList();
            foreach (var readingTarget in readingTs)
            {
                _readingTargetRepository.Delete(readingTarget);
            }
            _readingTargetRepository.SaveChanges();
        }
    }
}