using System;
using NHSD.ElephantParade.DAL.EntityModels;
using NHSD.ElephantParade.Core.Models;
using NHSD.ElephantParade.DAL.Interfaces;
using NHSD.ElephantParade.Domain;
using NHSD.ElephantParade.Core.Interfaces;
using NHSD.ElephantParade.Domain.Models;
using Patient = NHSD.ElephantParade.DAL.EntityModels.Patient;
using Reading = NHSD.ElephantParade.DAL.EntityModels.Reading;
using ReadingTarget = NHSD.ElephantParade.DAL.EntityModels.ReadingTarget;

namespace NHSD.ElephantParade.Core.Mapping
{
    /// <summary>
    /// Converts entity objects (used to read/write from database) into model objects that can be passed to and from controllers.
    /// Also converts model objects to entity objects so data can be added/updated in the database.
    /// This level of abstraction means that the objects passed between the page level, and the database level are decoupled.
    /// </summary>
    public class ConvertViewModelObjects
    {
        
        #region Callback Conversions
        ///<summary> Convert Callback Object  CallbackViewModel
        ///</summary>
        public CallbackViewModel ConvertFromCallback(Callback callback, IStudiesService patientService, int? previousCalls = null, int? previousFailedCalls = null)
        {
            if (callback == null)
                return null;

            var cvm = new CallbackViewModel();

            cvm.CallbackId = callback.CallbackId;
            cvm.CallbackDate = callback.CallbackDate.Date;
            cvm.CallbackStartTime = (callback.CallbackStartTime.TimeOfDay);
            if (callback.CallbackEndTime.HasValue)
            {
                cvm.CallbackEndTime = (callback.CallbackEndTime.Value.TimeOfDay);
            }
            cvm.CallbackScheduledBy = callback.CallbackScheduledBy;
            cvm.Patient = patientService.GetPatient(callback.PatientId, callback.StudyID);
            cvm.Type = (CallbackViewModel.CallbackType)callback.Type;
            cvm.Completed = callback.Completed;
            cvm.LockedTo = callback.LockedTo;
            cvm.LockedDate = callback.LockedDate;
            cvm.CallsCompleted = previousCalls;
            cvm.CallsFailed = previousFailedCalls;
            cvm.CallOutcome = callback.CallOutcome;
            return cvm;
        }
        ///<summary>
        ///convert from CallbackViewModel to CallBack
        /// </summary>
        public Callback ConvertFromCallbackViewModel(CallbackViewModel callbackVM)
        {
            return ConvertFromCallbackViewModel(callbackVM, null);

        }

        public Callback ConvertFromCallbackViewModel(CallbackViewModel callbackVM, Callback callback)
        {
            if (callbackVM == null)
                return null;

            if (callback == null)
            {
                callback = new Callback();
            }

            //check the callbackId in the model if not null then assign it to the callback as we are updating the record, its not a new record
            if (callbackVM.CallbackId != null)
                callback.CallbackId = callbackVM.CallbackId;

            if (callbackVM.Patient != null)
            {
                callback.PatientId = callbackVM.Patient.PatientId;
                callback.StudyID = callbackVM.Patient.StudyID;
            }

            callback.Type = (int)callbackVM.Type;
            callback.CallbackDate = Convert.ToDateTime(callbackVM.CallbackDate);

            DateTime startTimeDate = callback.CallbackDate;
            startTimeDate = startTimeDate.Add(callbackVM.CallbackStartTime);
            callback.CallbackStartTime = startTimeDate;
            
            if (callbackVM.CallbackEndTime.HasValue)
            {
                DateTime endTimeDate = callback.CallbackDate;
                endTimeDate = endTimeDate.Add(callbackVM.CallbackEndTime.Value);
                callback.CallbackEndTime = endTimeDate;
            }

            callback.CallbackScheduledBy = callbackVM.CallbackScheduledBy;
            callback.CallbackScheduledDate = DateTime.Now;
            callback.CallOutcome = callbackVM.CallOutcome;

            return callback;
        }

        #endregion

        #region Patient Conversions

        //private ElephantParade.Domain.Models.StudyPatient ConvertFromPatient(Patient patient,string studyId)
        //{
        //    if (patient == null)
        //        return null;

        //    StudyPatient.PreferredContactNumberType pct;
        //    Enum.TryParse(patient.PreferredContactNumber, out pct);
        //    return new ElephantParade.Domain.Models.StudyPatient
        //                                {
        //                                    PatientId = patient.PatientId.ToString(),
        //                                    StudyID = studyId,
        //                                    StudyTrialNumber = patient.StudyTrialID,
        //                                    StudySite = patient.StudyArm,
        //                                    StudyConsentedDate = patient.StudyConsentedDate,
        //                                    StudyReferralDate = patient.StudyReferralDate,
        //                                    Status = (StudyPatient.PatientStudyStatusType)patient.Status,
        //                                    Title = patient.Title,
        //                                    Forename = patient.Firstname,
        //                                    Surname = patient.Lastname,
        //                                    TelephoneNumber = patient.Telephone1,
        //                                    TelephoneNumberMobile = patient.Telephone2,
        //                                    TelephoneNumberOther = patient.Telephone3,
        //                                    PreferredContactNumber =  pct,
        //                                    PrederredContactTime = patient.ScheduleNotes,

        //                                    RegisteredDate = patient.DateAdded,
                                            
        //                                    Address = new Domain.Models.Address()
        //                                    {
        //                                        Line1 = patient.Address,
        //                                        Line2 = patient.Address2,
        //                                        County = patient.County,
        //                                        PostCode = patient.PostCode
        //                                    },

        //                                    DOB = patient.DOB,
        //                                    Gender = patient.Gender,
        //                                    Ethnicity = patient.Ethnicity,
        //                                    Email = patient.EmailAddress,

        //                                    GPPractice = new Domain.Models.GeneralPractitioner() { 
        //                                        Practice = patient.GpPractice,
        //                                        EmailAddress = patient.GpPracticeEmail,
        //                                        Name = patient.GpPracticeName,
        //                                        PrimaryCareTrust = patient.GpPracticePCT,
        //                                        TelephoneNumber = patient.GpPracticeTelephone,

        //                                    },
        //                                    NhsNumber = patient.NHSNumber,
        //                                    OnAntidepressants = patient.OnAntidepressants??false,
        //                                    BaselineGAD7 = patient.BaselineGAD7??0,
        //                                    BaselinePHQ9 = patient.BaselinePHQ9??0,
        //                                    Education = patient.Education
        //                                };
        //}
        public ElephantParade.Domain.Models.StudyPatient ConvertFromPatient(Patient patient, string studyId, IGpPracticeAddressRepository gpPracticeAddress)
        {
            var id = patient.PatientId.ToString();
            var gpAddress = gpPracticeAddress.GetSingle(p => p.PatientID == id && p.StudyID == studyId);
            return ConvertFromPatient(patient ,studyId,gpAddress);
        }

        ///<summary>
        ///Convert Patient Object to PatientViewModel
        ///</summary>
        public ElephantParade.Domain.Models.StudyPatient ConvertFromPatient(Patient patient, string studyId, ElephantParade.DAL.EntityModels.GpPracticeAddress gpPracticeAddress)
        {
            if (patient == null)
                return null;

            StudyPatient.PreferredContactNumberType pct;
            Enum.TryParse(patient.PreferredContactNumber, out pct);
            var p = new ElephantParade.Domain.Models.StudyPatient
            {
                PatientId = patient.PatientId.ToString(),
                StudyID = studyId,
                StudyTrialNumber = patient.StudyTrialID,
                StudySite = patient.StudyArm,
                StudyConsentedDate = patient.StudyConsentedDate,
                StudyReferralDate = patient.StudyReferralDate,
                Status = (StudyPatient.PatientStudyStatusType)patient.Status,
                Title = patient.Title,
                Forename = patient.Firstname,
                Surname = patient.Lastname,
                TelephoneNumber = patient.Telephone1,
                TelephoneNumberMobile = patient.Telephone2,
                TelephoneNumberOther = patient.Telephone3,
                PreferredContactNumber = pct,
                PrederredContactTime = patient.ScheduleNotes,

                RegisteredDate = patient.DateAdded,

                Address = new Domain.Models.Address()
                {
                    Line1 = patient.Address,
                    Line2 = patient.Address2,
                    County = patient.County,
                    PostCode = patient.PostCode
                },

                DOB = patient.DOB,
                Gender = patient.Gender,
                Ethnicity = patient.Ethnicity,
                Email = patient.EmailAddress,

                GPPractice = new Domain.Models.GeneralPractitioner()
                {
                    EmailAddress = patient.GpPracticeEmail,
                    Name = patient.GpPracticeName,
                    PrimaryCareTrust = patient.GpPracticePCT,
                    TelephoneNumber = patient.GpPracticeTelephone
                },
                NhsNumber = patient.NHSNumber,
                OnAntidepressants = patient.OnAntidepressants ?? false,
                BaselineGAD7 = patient.BaselineGAD7 ?? 0,
                BaselinePHQ9 = patient.BaselinePHQ9 ?? 0,
                Education = patient.Education
            };
            if (gpPracticeAddress != null)
            {
                p.GPPractice.Practice = gpPracticeAddress.GpPractice;
                p.GPPractice.Address = new Domain.Models.Address()
                                           {
                                               Line1 = gpPracticeAddress.GpPracticeAddress1,
                                               Line2 = gpPracticeAddress.GpPracticeAddress2,
                                               County = gpPracticeAddress.GpPracticeAddress3,
                                               PostCode = gpPracticeAddress.GpPostCode
                                           };
            }
            return p;
        }


        ///<summary>
        ///Convert PatientViewModel to Patient Object
        ///</summary>

        public Patient ConvertFromPatientViewModel(ElephantParade.Domain.Models.StudyPatient patientViewModel, ref GpPracticeAddress gpPracticeAddress)
        {
            return ConvertFromPatientViewModel(patientViewModel, null, ref gpPracticeAddress);
        }
        internal Patient ConvertFromPatientViewModel(StudyPatient patientViewModel, Patient patient, ref GpPracticeAddress gpPracticeAddress)
        {
            
            if (patientViewModel == null)
                return null;

            if (patient == null)
                patient = new Patient();

            patient.StudyTrialID = patientViewModel.StudyTrialNumber;
            patient.Status = (int)patientViewModel.Status;
            patient.StudyArm = patientViewModel.StudySite;
            patient.StudyConsentedDate = patientViewModel.StudyConsentedDate;
            patient.StudyReferralDate = patientViewModel.StudyReferralDate;
            patient.Title = patientViewModel.Title;
            patient.Firstname = patientViewModel.Forename;
            patient.Lastname = patientViewModel.Surname;
            patient.Telephone1 = patientViewModel.TelephoneNumber;
            patient.Telephone2 = patientViewModel.TelephoneNumberMobile;
            patient.Telephone3 = patientViewModel.TelephoneNumberOther;
            patient.PreferredContactNumber = patientViewModel.PreferredContactNumber.ToString();
            patient.ScheduleNotes = patientViewModel.PrederredContactTime;

            patient.DateAdded = patientViewModel.RegisteredDate;
            patient.Address = patientViewModel.Address.Line1;
            patient.Address2 = patientViewModel.Address.Line2;
            //patient.Address3 = patientViewModel.Address.Line3;
            patient.County = patientViewModel.Address.County;
            patient.PostCode = patientViewModel.Address.PostCode;
            
            patient.DOB = patientViewModel.DOB;
            patient.Gender = patientViewModel.Gender;
            patient.Ethnicity = patientViewModel.Ethnicity;
            patient.EmailAddress = patientViewModel.Email;
            patient.Education = patientViewModel.Education;

//            patient.GpPractice = patientViewModel.GPPractice.Practice;
            patient.GpPracticeName = patientViewModel.GPPractice.Name;
            patient.GpPracticeEmail = patientViewModel.GPPractice.EmailAddress;
            patient.GpPracticePCT = patientViewModel.GPPractice.PrimaryCareTrust;
            patient.GpPracticeTelephone = patientViewModel.GPPractice.TelephoneNumber;

            gpPracticeAddress.StudyID = patientViewModel.StudyID;
            gpPracticeAddress.GpPractice = patientViewModel.GPPractice.Practice;

            if (patientViewModel.GPPractice.Address != null)
            {
                gpPracticeAddress.GpPracticeAddress1 = patientViewModel.GPPractice.Address.Line1;
                gpPracticeAddress.GpPracticeAddress2 = patientViewModel.GPPractice.Address.Line2;
                gpPracticeAddress.GpPracticeAddress3 = patientViewModel.GPPractice.Address.County;
                gpPracticeAddress.GpPostCode = patientViewModel.GPPractice.Address.PostCode;
            }
            patient.NHSNumber = patientViewModel.NhsNumber;
            patient.BaselineGAD7 = patientViewModel.BaselineGAD7;
            patient.BaselinePHQ9 = patientViewModel.BaselinePHQ9;
            patient.OnAntidepressants = patientViewModel.OnAntidepressants;
            
            return patient;
        }

        #endregion

        #region GPPracticeAddress Conversions

        public ElephantParade.Domain.Models.Address ConvertFromGPPracticeAddress(GpPracticeAddress gpPracticeAddress)
        {
            return new ElephantParade.Domain.Models.Address()
            {
                Line1 = gpPracticeAddress.GpPracticeAddress1,
                Line2 = gpPracticeAddress.GpPracticeAddress2,
                County = gpPracticeAddress.GpPracticeAddress3,
                PostCode = gpPracticeAddress.GpPostCode
            };
        }

        public GpPracticeAddress ConvertFromGPPracticeAddressViewModel(ElephantParade.Domain.Models.Address gpPracticeAddressViewModel, Guid patientId, string studyId)
        {
            return ConvertFromGPPracticeAddressViewModel(gpPracticeAddressViewModel, null, patientId, studyId);
        }

        public GpPracticeAddress ConvertFromGPPracticeAddressViewModel(ElephantParade.Domain.Models.Address gpPracticeAddressViewModel, GpPracticeAddress gpPracticeAddress, Guid patientId, string studyId)
        {
            // CommentId is auto generated no need to pass from the ViewModel.

            if (gpPracticeAddressViewModel == null)
                return null;

            if (gpPracticeAddress == null)
                gpPracticeAddress = new GpPracticeAddress();

            gpPracticeAddress.PatientID = Convert.ToString(patientId);
            gpPracticeAddress.StudyID = studyId;
            gpPracticeAddress.GpPracticeAddress1 = gpPracticeAddressViewModel.Line1;
            gpPracticeAddress.GpPracticeAddress2 = gpPracticeAddressViewModel.Line2;
            gpPracticeAddress.GpPracticeAddress3 = gpPracticeAddressViewModel.County;
            gpPracticeAddress.GpPostCode = gpPracticeAddressViewModel.PostCode;

            return gpPracticeAddress;
        }


        #endregion

        #region PatientComments Conversions
        ///<summary>
        ///Convert PatientComments Object to PatientCommentsViewModel
        ///</summary>

        public PatientCallCommentViewModel ConvertFromPatientComments(CallbackPatientComment patientComments)
        {
            if (patientComments == null)
                return null;

            return new PatientCallCommentViewModel
            {
                CommentId = patientComments.CommentID,
                Text = patientComments.Comments,
                UserID = patientComments.CommentsEnteredBy,
                //Patient = ConvertFromPatient(patientComments.Patient),
                PatientID = patientComments.PatientId,
                StudyID = patientComments.StudyID,
                Date = patientComments.CommentsDate               
            };
        }


        ///<summary>
        ///Convert PatientCommentsViewModel to PatientComments Object
        ///</summary>

        public CallbackPatientComment ConvertFromPatientCommentsViewModel(PatientCallCommentViewModel patientCommentsViewModel)
        {
            return ConvertFromPatientCommentsViewModel(patientCommentsViewModel, null);
        }

        public CallbackPatientComment ConvertFromPatientCommentsViewModel(PatientCallCommentViewModel patientCommentsViewModel, CallbackPatientComment patientComments)
        {
            // CommentId is auto generated no need to pass from the ViewModel.

            if (patientCommentsViewModel == null)
                return null;

            if (patientComments == null)
                patientComments = new CallbackPatientComment();

            patientComments.PatientId = patientCommentsViewModel.PatientID;
            patientComments.StudyID = patientCommentsViewModel.StudyID;
            patientComments.Comments = patientCommentsViewModel.Text;
            patientComments.CommentsDate = DateTime.Now;
            patientComments.CommentsEnteredBy = patientCommentsViewModel.UserID;
            patientComments.CallbackID = patientCommentsViewModel.CallbackId;
            return patientComments;
        }
        #endregion

        #region Event Conversions
        ///<summary>
        ///PatientHistoryViewModel To PatientHistory
        ///</summary>
        public CallEvent ConvertFromCallEventViewModel(CallEventViewModel callEventVM)
        {
            return ConvertFromCallEventViewModel(callEventVM, null);
        }
        public CallEvent ConvertFromCallEventViewModel(CallEventViewModel callEventVM, CallEvent callEvent)
        {
            if (callEvent == null)
                callEvent = new CallEvent();

            if (callEventVM == null)
                return null;

            callEvent.UserID = callEventVM.UserID ;
            callEvent.Message = callEventVM.Text;
            callEvent.EventCode = callEventVM.EventCode;
            callEvent.Date = callEventVM.Date;
            callEvent.StudyID = callEventVM.StudyID;
            callEvent.PatientId = callEventVM.PatientID;
            return callEvent;
        }

        ///<summary>
        ///Converting from PatientHistory to PatientHistoryViewModel
        ///</summary>
        public CallEventViewModel ConvertFromCallEvent(CallEvent callEvent)
        {
                        
            // Check the object of databse if its null then return null object else convert it into ViewModel and then return

            if (callEvent == null)
                return null;

            // converting into the ViewModel object and then returning.
            return new CallEventViewModel
            {
                Date = callEvent.Date,
                EventCode = callEvent.EventCode,
                Text = callEvent.Message,
                PatientID = callEvent.PatientId,
                StudyID = callEvent.StudyID,
                UserID = callEvent.UserID 
            };
        }
        #endregion

        #region Medical Condition
        internal Domain.Models.MedicalConditionItem  ConvertFromPatientMedication(PatientMedication m)
        {
            if (m == null)
                return null;

            Domain.Models.MedicalConditionItem mVM = new Domain.Models.MedicalConditionItem(m.ItemID)
            {
                Dose = m.Dose,
                Frequency = m.Frequency,
                Name = m.Name,
                Type = (NHSD.ElephantParade.Domain.Models.MedicalConditionItem.ItemType)Enum.Parse(typeof(NHSD.ElephantParade.Domain.Models.MedicalConditionItem.ItemType), m.Type)
            };
            return mVM;
        }

        internal PatientMedication ConvertFromPatientMedicationVM(Domain.Models.MedicalConditionItem mVM,PatientMedication m = null)
        {
            if (m == null)
                m = new PatientMedication();

            m.ItemID =  mVM.ItemID.HasValue?mVM.ItemID.Value:0;
            m.Name = mVM.Name??string.Empty;
            m.Dose = mVM.Dose ?? string.Empty;
            m.Frequency = mVM.Frequency ?? string.Empty;
            m.Type = mVM.Type.ToString();
            return m;
        }

        #endregion

        #region Reading Conversions

        //internal Domain.Models.Reading ConvertFromReading(Reading reading)
        //{
        //    if (reading == null || reading.PatientId == null || reading.ReadingEntered == null)
        //        return null;

        //    Domain.Models.Reading readingVM = new Domain.Models.Reading
        //    {
        //        DateOfReading = reading.DateOfReading,
        //        PatientId = reading.PatientId,
        //        SubmittedBy = reading.SubmittedBy,
        //        Valid = reading.Valid
        //    };
        //    return readingVM;
        //}

        internal Reading ConvertFromReadingVM(Domain.Models.Reading readingVM)
        {
            if (readingVM == null || readingVM.PatientId == null)
                return null;

            return new Reading
            {
                DateOfReading = readingVM.DateOfReading.Value,
                PatientId = readingVM.PatientId,
                SubmittedBy = readingVM.SubmittedBy,
                Valid = readingVM.Valid
            };
        }

        // Get the Systolic (high reading) values for the Blood Pressure VM
        internal BloodPressureReadingViewModel ConvertFromBPHighReading(Reading reading)
        {
            if (reading == null)
                return null;

            BloodPressureReadingViewModel bprHighReadingVM = new BloodPressureReadingViewModel
            {
                DateOfReading = reading.DateOfReading,
                PatientId = reading.PatientId,
                StudyId = reading.StudyId,
                Systolic = int.Parse(reading.ReadingEntered)
            };

            return bprHighReadingVM;
        }

        // Get the Diastolic low reading values for the Blood Pressure VM
        internal BloodPressureReadingViewModel ConvertFromBPLowReading(Reading reading)
        {
            if (reading == null)
                return null;

            // Get the high reading values for the Blood Pressure VM
            BloodPressureReadingViewModel bprLowReadingVM = new BloodPressureReadingViewModel
            {
                DateOfReading = reading.DateOfReading,
                PatientId = reading.PatientId,
                StudyId = reading.StudyId,
                Diastolic = int.Parse(reading.ReadingEntered)
            };

            return bprLowReadingVM;
        }

        internal Domain.Models.ReadingTarget ConvertFromReadingTarget(ReadingTarget readingTarget)
        {
            if (readingTarget == null || string.IsNullOrEmpty(readingTarget.PatientId))
                return null;

            return new Domain.Models.ReadingTarget
            {
                DateSet = readingTarget.DateEntered,
                PatientId = readingTarget.PatientId,
                ReadingType = (ReadingTypes)readingTarget.ReadingTypeId,
                SubmittedBy = readingTarget.SubmittedBy,
                Target = readingTarget.Target,
                Valid = readingTarget.Valid
            };
        }

        internal ReadingTarget ConvertFromReadingTargetVM(Domain.Models.ReadingTarget readingTargetVM)
        {
            if (readingTargetVM == null || string.IsNullOrEmpty(readingTargetVM.PatientId))
                return null;

            return new ReadingTarget
            {
                DateEntered = readingTargetVM.DateSet,
                PatientId = readingTargetVM.PatientId,
                StudyId = readingTargetVM.StudyId,
                Target = readingTargetVM.Target,
                ReadingTypeId = (int)readingTargetVM.ReadingType,
                SubmittedBy = readingTargetVM.SubmittedBy,
                Valid = readingTargetVM.Valid
            };
        }

        internal BloodPressureReadingViewModel ConvertFromReadingExpectedToBloodPressureViewModel(ReadingExpected readingExpected)
        {
            if (readingExpected == null)
                return null;

            // Get the high reading values for the Blood Pressure VM
            BloodPressureReadingViewModel readingExpectedVM = new BloodPressureReadingViewModel
            {
                DateOfReading = readingExpected.ExpectedDate,
                PatientId = readingExpected.PatientId,
                StudyId = readingExpected.StudyId
            };

            return readingExpectedVM;
        }

        internal ReadingExpectedViewModel ConvertFromReadingExpected(ReadingExpected readingExpected)
        {
            if (readingExpected == null)
                return null;

            return new ReadingExpectedViewModel
            {
                Date = readingExpected.ExpectedDate,
                PatientId = readingExpected.PatientId,
                StudyId = readingExpected.StudyId,
                ReadingGiven = readingExpected.ReadingGiven
            };
        }

        #endregion

        #region File Conversions

        internal Domain.Models.PatientFileData ConvertFromFileData(File file)
        {
            Domain.Models.PatientFileData pdFile = new PatientFileData();
            ConvertFromFile(file, pdFile);

            pdFile.Data = file.FileData.Data; 
            return pdFile;
        }

        internal void ConvertFromFileData(Domain.Models.PatientFileData pfile, ref File file)
        {
            if (file == null)
                file = new File();

            if (file.FileData == null)
                file.FileData = new FileData();

            ConvertFromFile(pfile, ref file);
            file.FileData.Data = pfile.Data;            
        }

        internal void ConvertFromFile(Domain.Models.PatientFile pfile, ref File file)
        {
            if (file == null)
                file = new File();            
            file.Extension = pfile.Extension;
            file.FileLength = pfile.Length;
            file.Filename = pfile.Filename;
            file.PatientID = pfile.PatientID;
            file.StudyID = pfile.StudyID;
            file.Date = pfile.Date;
        }

        internal Domain.Models.PatientFile ConvertFromFile(File file ,Domain.Models.PatientFile pfile)
        {
            if (file == null || file.PatientID == null)
                return null;

            if(pfile==null)
                pfile = new PatientFile();

            pfile.FileID = file.FileID;
            pfile.Extension = file.Extension;
            pfile.Filename = file.Filename;
            pfile.Length = file.FileLength;
            pfile.PatientID = file.PatientID;
            pfile.StudyID = file.StudyID;
            pfile.Date = file.Date;
            return pfile;
        }

        #endregion

        #region PageVisitedLog

        internal PageVisitedLog ConvertFromPageVisitedLogViewModel(PageVisitedLogViewModel pageVisitedLogViewModel)
        {
            PageVisitedLog pvl = new PageVisitedLog
            {
                DateVisited = pageVisitedLogViewModel.DateVisited,
                IPAddress = pageVisitedLogViewModel.IPAddress,
                PageURL = pageVisitedLogViewModel.PageURL,
                Username = pageVisitedLogViewModel.Username
            };

            return pvl;
        }

        #endregion

        #region Password Reset Request Conversion

        internal Domain.Models.PasswordResetRequestViewModel ConvertFromPasswordResetRequest(PasswordResetRequest passwordResetRequest)
        {
            if (passwordResetRequest == null)
                return null;

            Domain.Models.PasswordResetRequestViewModel passwordResetRequestVM = new Domain.Models.PasswordResetRequestViewModel
            {
                UserName = passwordResetRequest.UserName,
                PasswordResetRequestId = passwordResetRequest.PasswordResetRequestId,
                Date = passwordResetRequest.DateOfRequest
            };

            return passwordResetRequestVM;
        }

        internal PasswordResetRequest ConvertFromPasswordResetRequestVM(PasswordResetRequestViewModel passwordResetRequestVM)
        {
            PasswordResetRequest prr = new PasswordResetRequest
            {
                PasswordResetRequestId = passwordResetRequestVM.PasswordResetRequestId,
                UserName = passwordResetRequestVM.UserName,
                DateOfRequest = DateTime.Now,
            };

            return prr;
        }

        #endregion

        #region Events
        internal DAL.EntityModels.PatientEvent ConvertFromEvent(Domain.Models.PatientEvent newEvent)
        {
            DAL.EntityModels.PatientEvent pEvent = new DAL.EntityModels.PatientEvent()
            {                
                Details = newEvent.Details??"",
                EventCode = (int)newEvent.EventCode,
                EventDate = newEvent.Date,
                EventUser = newEvent.User,
                PatientID = newEvent.PatientId,
                StudyID = newEvent.StudyID,
                Value = newEvent.Value1??"",
                Value2 = newEvent.Value2 ??""              
            };

            return pEvent;
        }

        internal Domain.Models.PatientEvent ConvertFromEvent(DAL.EntityModels.PatientEvent e)
        {
            Domain.Models.PatientEvent pEvent = new Domain.Models.PatientEvent()
            {
                Date = e.EventDate,
                Details = e.Details,
                EventCode =  (Domain.Models.PatientEvent.EventTypes)e.EventCode,
                EventID = e.ID,
                PatientId = e.PatientID,
                StudyID = e.StudyID,
                User = e.EventUser,
                Value1 = e.Value,
                Value2 = e.Value2
            };
            return pEvent;
        }
        #endregion

        
    }
}