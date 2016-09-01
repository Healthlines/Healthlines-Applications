using System;
using System.Collections.Generic;
using System.Configuration;
using System.Configuration.Provider;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using NHSD.ElephantParade.Core;
using NHSD.ElephantParade.Core.Interfaces;
using NHSD.ElephantParade.Core.Models;
using NHSD.ElephantParade.Domain.Models;
using NHSD.ElephantParade.Domain.Models.Extensions;
using NHSD.ElephantParade.Web.Areas.Administration.Helpers;
using NHSD.ElephantParade.Web.Areas.Administration.Models;

namespace NHSD.ElephantParade.Web.Areas.Administration.Controllers
{
    public class AdministrationController : Controller
    {
        #region Service instantiations
        //TA: no need to initialize objects as null. It's null if you don't initialize it with any value
        private readonly IStudiesService _studiesService;
        private readonly ICallbackService _callbackService;
        private readonly IReadingService _readingService;
        private readonly IMembershipService _membershipService;
        private readonly INonSecureEmailService _emailService;

        public AdministrationController(
            IStudiesService studiesService, 
            ICallbackService callbackService, 
            IReadingService readingService,
            IMembershipService membershipService,
            INonSecureEmailService emailService)
        {
            _studiesService = studiesService;
            _callbackService = callbackService;
            _membershipService = membershipService;
            _readingService = readingService;
            _emailService = emailService;
        }
        #endregion

        

        [Authorize(Roles = "Administrator, Uploader")]
        public ActionResult StudyPatientDataUpload()
        {
            return View("StudyPatientDataUpload");
        }

        [Authorize(Roles = "Administrator, Uploader")]
        [HttpPost]
        //UserNameFilter used to seperate httpcontext from controller to allow for controller unit testing
        [UserNameFilter]
        // Instaed of using HttpPostedFileBase, there could be an input stream object that get passed in when the 'Upload' 
        public ActionResult StudyPatientDataUpload(string userName, HttpPostedFileBase file) //TA: 500+ lines for one method is not good coding practice! This needs to be split up.
        {
            // Verify that the user selected a file
            if (file != null && file.ContentLength > 0)
            {
                List<Exception> errors;
                ImportResults importResults = new ImportResults();
                 
                //parse spreadsheet 
                IList<PatientImportData> patientImportDataList = new StudyReaderXlsx(_studiesService.DepressionStudyID, _studiesService.CVDStudyID).Read(file.InputStream, out errors);
                
                try
                {
                    foreach (var patientImportData in patientImportDataList)
                    {
                        // check if StudyPatient already exists before adding to the database
                        if (_studiesService.DoesPatientExistInTheDatabase(patientImportData.Patient))
                            //ToDo: replace with custom exception object to allow for multi language
                            errors.Add(new Exception(string.Format("Patient already exists for email address {1}: StudyID: {0} ",
                                                                            patientImportData.Patient.StudyID, patientImportData.Patient.Email)));
                        else if (!String.IsNullOrEmpty(_membershipService.GetUserNameByEmail(patientImportData.Patient.Email)))
                            errors.Add(new Exception(string.Format("Membership already exists for email address {1}: StudyID: {0} ",
                                                                            patientImportData.Patient.StudyID, patientImportData.Patient.Email)));
                        else
                        {
                            StudyPatient newPatient = null;
                            CallbackViewModel callbackVm = null;
                            PatientCallCommentViewModel patientCallCommentVm = null;
                            try
                            {
                                //create the new patient
                                newPatient = _studiesService.AddPatient(patientImportData.Patient, userName);//User.Identity.Name);
                               
                                // Add the callback for the patient
                                callbackVm = new CallbackViewModel();
                                callbackVm.Patient = newPatient;
                                callbackVm.CallbackScheduledDate = DateTime.Now;
                                callbackVm.CallbackDate = DateTime.Now.AddDays(3).Date.AddHours(9);
                                callbackVm.CallbackScheduledBy = userName;//User.Identity.Name;
                                callbackVm.Type = CallbackViewModel.CallbackType.Scheduled;
                                callbackVm.Completed = false;
                                _callbackService.CallBack_Add(callbackVm, Constants.EncounterIncompleteText, false);

                                // Add the callback comments for the patient
                                patientCallCommentVm = new PatientCallCommentViewModel();
                                patientCallCommentVm.PatientID = newPatient.PatientId;
                                patientCallCommentVm.StudyID = patientImportData.Patient.StudyID;
                                patientCallCommentVm.EventCode = PatientEvent.EventTypes.PatientAdded.ToString();
                                patientCallCommentVm.Text = patientImportData.Notes;
                                patientCallCommentVm.Date = DateTime.Today;
                                patientCallCommentVm.UserID = userName;//User.Identity.Name;
                                _callbackService.PatientComments_Add(patientCallCommentVm);
                                
                                if (patientImportData.Patient.StudyID == _studiesService.DepressionService.StudyID)
                                {
                                    CreateDepressionAccount(newPatient);
                                    // Add the patient successfully entered to the 'patients added' list
                                    importResults.Created.Add(newPatient);
                                }
                                else if (patientImportData.Patient.StudyID == _studiesService.CvdService.StudyID)
                                {
                                    CreateBpTargets(userName,newPatient.PatientId, patientImportData);
                                    CreatePatientScores(userName,Convert.ToInt32(newPatient.PatientId),patientImportData);
                                    CreateEncounters(Convert.ToInt32(newPatient.PatientId));
                                    CreateCvdAccount(newPatient);
                                    // Add the patient successfully entered to the 'patients added' list
                                    importResults.Created.Add(newPatient);
                                }
                                else //This should not be possible as should have been validated by StudyReaderXlsx so stop here
                                {
                                    throw new Exception(string.Format("Unsupported Study for email address: {1} and StudyID:{0}. Supported studies are 'Depression' or 'CVD'",
                                                    patientImportData.Patient.StudyID, patientImportData.Patient.Email));
                                }
                            }
                            catch (Exception ex )
                            {
                                if (newPatient != null)
                                {
                                    try
                                    {
                                        _studiesService.DeletePatient(newPatient, userName);//User.Identity.Name);
                                        errors.Add(new Exception(string.Format("Error importing patient {0}. Review details and try again.",newPatient.Email)));
                                    }
                                    catch (Exception deleteEx)
                                    {
                                        throw new Exception(string.Format("An unrecoverable error occured during the processing of patient {0}. " +
                                                                          "Please contact an administrator regarding this issue.", newPatient.PatientId), deleteEx);
                                    }

                                    //delete callbacks
                                    if (callbackVm != null && callbackVm.CallbackId!=Guid.Empty)
                                        _callbackService.CallBack_Delete(callbackVm.CallbackId);
                                    //delete comments
                                    if (patientCallCommentVm != null && patientCallCommentVm.CommentId>0)
                                        _callbackService.PatientComments_Delete(patientCallCommentVm.CommentId);
                                    //delete BP targets
                                    if(newPatient.StudyID==_studiesService.CVDStudyID)
                                        _readingService.DeleteAll(newPatient.PatientId);
                                    _membershipService.DeleteMembershipUserByEmail(newPatient.Email);
                                }
                                else
                                {
                                    errors.Add(ex);
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    errors.Add(ex);
                }
            
                importResults.ImportErrors = errors.Where(e => !(e is StudyReaderXlsx.InvalidWorkBook) && !(e is StudyReaderXlsx.InvalidEntry)).Select(e => e.Message).ToArray();
                importResults.FileErrors = errors.Where(e => e is StudyReaderXlsx.InvalidWorkBook).Select(e => e.Message).ToArray().ToArray();
                importResults.FileErrors = importResults.FileErrors.Union(
                                                            errors.Where(e => e is StudyReaderXlsx.InvalidEntry).
                                                            Select(e => "Row:" + ((StudyReaderXlsx.InvalidEntry)e).RowNumber.ToString(CultureInfo.InvariantCulture) + " - " + e.Message)
                                                            ).ToArray();
                importResults.Filename = file.FileName;
                return View(importResults);
            }
            return View();
        }



        [Authorize(Roles = "Administrator")]
        [HttpGet]
        public ActionResult CreateUser()
        {
            return View();
        }

        [Authorize(Roles = "Administrator")]
        [HttpPost]
        public ActionResult CreateUser(UserViewModel user)
        {
            //validation of data provided.
            if (user.Supervisor && user.Administrator)
                ModelState.AddModelError("Supervisor", "Please only select either Supervisor, or Administrator");
            if (!Validation.IsEmailAddress(user.EmailAddress))
                ModelState.AddModelError("EmailAddress", "Please enter a valid email address");
            if (!Validation.IsUserName(user.UserName))
                ModelState.AddModelError("UserName", "Please enter a valid user name");

            if (ModelState.IsValid)
            {
                //create user
                try
                {
                    //no need to give a password as this user will connect using their AD account.
                    user.Password = null;
                    
                    //this form is used for creating advisors - so will always be true
                    user.Advisor = true;
                    MembershipCreateStatus status = _membershipService.CreateMembershipUser(user);

                    if (status == MembershipCreateStatus.Success)
                    {
                        return View("UserCreated");
                    }
                    else
                    {
                        ModelState.AddModelError("UserName", "Unable to create user in the system. Please enter a valid user name.");
                        return View();
                    }
                }
                catch (MembershipCreateUserException createUserException)
                {
                    //could not add user.
                    ModelState.AddModelError("UserName", "Error occurred: " + createUserException.Message);
                }
                catch (ProviderException providerException)
                {
                    //most likely issue - user already added
                    ModelState.AddModelError("UserName", "Error occurred: " + providerException.Message);
                }
            }

            return View();
        }

        [Authorize(Roles = "Administrator")]
        public ActionResult DeleteUser()
        {
            //disaply list of users we can delete
            return View(_membershipService.ExistingADUsers());
        }

        [Authorize(Roles = "Administrator")]
        public ActionResult ConfirmDelete(string userName)
        {
            //perform deletion
            _membershipService.DeleteMembershipUserByUserName(userName);

            return View("UserDeleted");
        }

        [Authorize(Roles = "Administrator")]
        public ActionResult Users(string search)
        {
            MembershipUserCollection users;
            if (String.IsNullOrWhiteSpace(search))
            {
                int totalRecords;
                var tmp = _membershipService.ExistingADUsers();
                users = Membership.GetAllUsers(0, 250, out totalRecords);
            }
            else
            {
                users = Membership.FindUsersByName(search);            
            }
            return View(users.Cast<MembershipUser>());

        }
        [Authorize(Roles = "Administrator")]
        [HttpPost]
        public ActionResult SetPassword(string username, string password)
        {
            var user = Membership.GetUser(username);

            if (user != null)
            {
                user.UnlockUser();
                var oldP = user.ResetPassword();
                if (user.ChangePassword(oldP, password))
                {
                    return View("UserPasswordSet");
                }
                return Content("Error");
            }
            return Content("Error");
        }

        [Authorize(Roles = "Administrator")]
        [HttpGet]
        public ActionResult UnlockUser(string username)
        {
            var user = Membership.GetUser(username);
            if (user != null) user.UnlockUser();
            return RedirectToAction("Users", new { search = username });
        }
        

        #region Private Methods

        /// <summary>
        /// Creates a membership account with Depression role and emails the patient with account details
        /// </summary>
        /// <param name="newPatient"></param>
        private void CreateDepressionAccount(StudyPatient newPatient)
        {
            _membershipService.CreateDepressionAccount(newPatient);
        }

        /// <summary>
        /// Creates a membership account with CVD role and emails the patient with account details
        /// </summary>
        /// <param name="newPatient"></param>
        private void CreateCvdAccount(StudyPatient newPatient)
        {
            _membershipService.CreateCVDAccount(newPatient);
        }

        /// <summary>
        /// Create the target BP's
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="patientId"></param>
        /// <param name="patientImportData"></param>
        private void CreateBpTargets(string userName,String patientId, PatientImportData patientImportData)
        {
            #region Add BP Targets
            // Add BP target for the patient to the CADS database
            ReadingTarget readingTarget = new ReadingTarget();
            readingTarget.PatientId = patientId;
            readingTarget.StudyId = patientImportData.Patient.StudyID;
            readingTarget.SubmittedBy = userName;
            readingTarget.Valid = true;
            readingTarget.DateSet = DateTime.Now;
            readingTarget.Target = patientImportData.TargetDiastolic.ToString("#.##");
            readingTarget.ReadingType = ReadingTypes.DiastolicBP;

            _readingService.AddReadingTarget(readingTarget);

            readingTarget.PatientId = patientId;
            readingTarget.StudyId = patientImportData.Patient.StudyID;
            readingTarget.SubmittedBy = userName;
            readingTarget.Valid = true;
            readingTarget.DateSet = DateTime.Now;
            readingTarget.Target = patientImportData.TargetSystolic.ToString("#.##");
            readingTarget.ReadingType = ReadingTypes.SystolicBP;

            _readingService.AddReadingTarget(readingTarget);
            #endregion

        }

        /// <summary>
        /// Create the PatientScores
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="patientId"></param>
        /// <param name="patientImportData"></param>
        private void CreatePatientScores(string userName,int patientId, PatientImportData patientImportData)
        {

            #region Add Participant scores

            // Add the particiant scores (e.g. BMI, systolic, diastolic, height and weight) to the database
            ParticipantScore participantScore = new ParticipantScore();
            participantScore.ParticipantId = patientId;
            participantScore.ScoreDate = DateTime.Now;

            participantScore.ScoreId = ParticipantScore.ParticipantScoreIdType.Systolic.ToString();
            participantScore.ScoreValue = patientImportData.BaselineSystolicBP.ToString("#.##");
            _studiesService.CvdService.AddParticipantScore(participantScore, userName);

            participantScore.ScoreId = ParticipantScore.ParticipantScoreIdType.Diastolic.ToString();
            participantScore.ScoreValue = patientImportData.BaselineDiastolicBP.ToString("#.##");
            _studiesService.CvdService.AddParticipantScore(participantScore, userName);

            participantScore.ScoreId = ParticipantScore.ParticipantScoreIdType.Smoker.ToString();
            participantScore.ScoreValue = patientImportData.BaselineSmokingStatus;
            _studiesService.CvdService.AddParticipantScore(participantScore, userName);

            participantScore.ScoreId = EnumExtensions.GetDescription<ParticipantScore.ParticipantScoreIdType>(ParticipantScore.ParticipantScoreIdType.TotalCholesterolRatio);
            participantScore.ScoreValue = patientImportData.TotalCholesterolRatio.ToString("#.##");
            _studiesService.CvdService.AddParticipantScore(participantScore, userName);

            participantScore.ScoreId = ParticipantScore.ParticipantScoreIdType.QRISK.ToString();
            participantScore.ScoreValue = patientImportData.BaselineQriskScore;
            _studiesService.CvdService.AddParticipantScore(participantScore, userName);

            participantScore.ScoreId = ParticipantScore.ParticipantScoreIdType.Diabetes.ToString();
            participantScore.ScoreValue = patientImportData.Diabetes;
            _studiesService.CvdService.AddParticipantScore(participantScore, userName);

            participantScore.ParticipantId = patientId;
            participantScore.ScoreId = ParticipantScore.ParticipantScoreIdType.BMI.ToString();
            participantScore.ScoreValue = patientImportData.BaselineBMI;
            _studiesService.CvdService.AddParticipantScore(participantScore, userName);

            participantScore.ScoreId = ParticipantScore.ParticipantScoreIdType.CKD.ToString();
            participantScore.ScoreValue = patientImportData.ChronicKidneyDisease;
            _studiesService.CvdService.AddParticipantScore(participantScore, userName);

            participantScore.ScoreId = ParticipantScore.ParticipantScoreIdType.AF.ToString();
            participantScore.ScoreValue = patientImportData.AtrialFibrillation;
            _studiesService.CvdService.AddParticipantScore(participantScore, userName);

            participantScore.ParticipantId = patientId;
            participantScore.ScoreId = EnumExtensions.GetDescription<ParticipantScore.ParticipantScoreIdType>(ParticipantScore.ParticipantScoreIdType.OnBPTreatment);
            participantScore.ScoreValue = patientImportData.OnBPMeds;
            _studiesService.CvdService.AddParticipantScore(participantScore, userName);

            participantScore.ScoreId = ParticipantScore.ParticipantScoreIdType.Weight.ToString();
            participantScore.ScoreValue = patientImportData.BaselineWeight;
            _studiesService.CvdService.AddParticipantScore(participantScore, userName);

            participantScore.ScoreId = ParticipantScore.ParticipantScoreIdType.Height.ToString();
            participantScore.ScoreValue = patientImportData.BaselineHeight;
            _studiesService.CvdService.AddParticipantScore(participantScore, userName);

            participantScore.ScoreId = EnumExtensions.GetDescription<ParticipantScore.ParticipantScoreIdType>(ParticipantScore.ParticipantScoreIdType.TargetSystolic);
            participantScore.ScoreValue = patientImportData.TargetSystolic.ToString("#.##");
            _studiesService.CvdService.AddParticipantScore(participantScore, userName);

            participantScore.ScoreId = EnumExtensions.GetDescription<ParticipantScore.ParticipantScoreIdType>(ParticipantScore.ParticipantScoreIdType.TargetDiastolic);
            participantScore.ScoreValue = patientImportData.TargetDiastolic.ToString("#.##");
            _studiesService.CvdService.AddParticipantScore(participantScore, userName);

            #endregion

        }

        /// <summary>
        /// Create the default enounters available within Duke
        /// </summary>
        /// <param name="patientId"></param>
        private void CreateEncounters(int patientId)
        {
            for (int encounterNumber = 1; encounterNumber <= Constants.EncountersInDuke; encounterNumber++)
            {
                // Add the default intervention schedules as required by Duke users
                InterventionSchedule interventionSchedule = new InterventionSchedule();
                interventionSchedule.ParticipantID = patientId;
                interventionSchedule.WindowOpen = DateTime.Today.AddYears(-2);
                interventionSchedule.WindowClose = DateTime.Today.AddYears(+2);
                interventionSchedule.EventType = "Encounter" + encounterNumber;
                interventionSchedule.Status = "Not Started.";
                interventionSchedule.Seq = Convert.ToSByte(encounterNumber);
                interventionSchedule.encounterID = encounterNumber;

                _studiesService.CvdService.AddInterventionSchedule(interventionSchedule);
            }
        }

        #endregion
    }
}