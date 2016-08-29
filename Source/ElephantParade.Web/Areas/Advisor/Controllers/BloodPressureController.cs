using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NHSD.ElephantParade.Core;
using NHSD.ElephantParade.Core.Interfaces;
using NHSD.ElephantParade.Core.Models;
using NHSD.ElephantParade.Web.Controllers;
using System.IO;
using NHSD.ElephantParade.DocumentGenerator;
using NHSD.ElephantParade.DocumentGenerator.Interfaces;
using NHSD.ElephantParade.Domain.Models;
using NHSD.ElephantParade.Web.Areas.Advisor.Models;

namespace NHSD.ElephantParade.Web.Areas.Advisor.Controllers
{
    public class BloodPressureController : BaseController
    {
        #region Fields

        //Member Variables
        private IStudiesService _studiesService;        
        private IReadingService _readingService;
        private IDocumentService _documentService;

        #endregion
        
        #region Ctor

        public BloodPressureController(IStudiesService studiesService,            
            IReadingService readingService, IDocumentService documentService)       
        {
            _studiesService = studiesService;
            _readingService = readingService;
            _documentService = documentService;
        }

        #endregion

        // Get the required info and display the set new BP targets page
        [Authorize(Roles = "Administrator,Advisor")]
        [HttpGet]
        public ActionResult SetNewBPTargets(string returnUrl, string patientID, string studyID)
        {
            BloodPressureTargetViewModel bptVM = new BloodPressureTargetViewModel();
            //bptVM.CallbackId = callbackID;
            //ViewBag.CallbackId = callbackID;
            bptVM.Patient = new StudyPatient();
            bptVM.Patient.PatientId = patientID;
            bptVM.Patient.StudyID = studyID;

            ViewBag.ReturnUrl = returnUrl;

            return View(bptVM);
        }

        // Validate against user input and set the values to the database if it valid, otherwise display error message
        [Authorize(Roles = "Administrator,Advisor")]
        [HttpPost]
        public ActionResult SetNewBPTargets(string returnUrl, string patientID, string studyID, BloodPressureTargetViewModel bptVM)
        {
            if (!ModelState.IsValid)
            {
                //bptVM.CallbackId = callbackID;
                ViewBag.ReturnUrl = returnUrl;
                bptVM.Patient = new StudyPatient();
                bptVM.Patient.PatientId = patientID;
                bptVM.Patient.StudyID = studyID;
                return View("SetNewBPTargets", bptVM);
            }
            else
            {
                var HlIdentity = User.Identity as NHSD.ElephantParade.Web.Authentication.HealthLinesParticipantIdentity;

                ReadingTarget readingTarget = new ReadingTarget();
                readingTarget.PatientId = patientID;
                readingTarget.StudyId = studyID;
                readingTarget.SubmittedBy = HlIdentity.Name;
                readingTarget.Valid = true;
                readingTarget.DateSet = DateTime.Now;

                // Set the diastolic target first
                readingTarget.ReadingType = ReadingTypes.DiastolicBP;
                readingTarget.Target = bptVM.DiastolicTarget;
                _readingService.AddReadingTarget(readingTarget);
                ViewBag.DiastolicBloodPressure = readingTarget.Target;

                // Then set the systolic target
                readingTarget.ReadingType = ReadingTypes.SystolicBP;
                readingTarget.Target = bptVM.SystolicTarget;
                _readingService.AddReadingTarget(readingTarget);
                ViewBag.SystolicBloodPressure = readingTarget.Target;
                if (!string.IsNullOrWhiteSpace(returnUrl))
                    return Redirect(returnUrl);
                else
                    return RedirectToAction("BloodPressureArea", new { patientId = patientID, studyId = studyID });
            }
        }

        // The action when the new medication is clicked, which will mark all previous readings as invalid
        [Authorize(Roles = "Administrator,Advisor")]
        [HttpPost]
        public ActionResult NewMedication(string patientId, string studyId)
        {
            // Mark previous systolic and diastolic readings as invalid
            _readingService.MarkReadingsAsInvalid(ReadingTypes.SystolicBP, patientId, studyId, DateTime.Today.AddYears(-100), DateTime.Now);
            _readingService.MarkReadingsAsInvalid(ReadingTypes.DiastolicBP, patientId, studyId, DateTime.Today.AddYears(-100), DateTime.Now);

            // Genrate new set of reading expectation dates
            _readingService.GenerateReadingExpectations(patientId, studyId, ReadingTypes.BloodPressure, ReadingFrequency.OnceWeekly, DateTime.Today);
            
            // Not returning anything as the AJAX call to refresh the patient readings will be called on success
            return null;
        }
        [Authorize(Roles = "Administrator,Advisor")]
        [HttpGet]
        public ActionResult ChangePatientReadingFrequency(string patientId, string studyId)
        {
            ReadingFrequency frequency = _readingService.GetReadingExpectedFrequency(patientId, studyId, ReadingTypes.BloodPressure);
            
            //does patient still have expected readings in the future that they need to give
            var futureExpectedReading = _readingService.PatientHasFutureExpectedReading(patientId, studyId, ReadingTypes.BloodPressure, DateTime.Today);
            
            if (frequency == ReadingFrequency.TwiceDaily && !futureExpectedReading)
            {
                var rfcvm = new ReadingFrequencyChangeViewModel{ PatientId = patientId, StudyId = studyId };

                return PartialView("_ChangePatientReadingFrequency", rfcvm);
            }
            else
            {
                //do not show view, as this patient cannot have their reading frequency changed.
                return new EmptyResult();
            }
        }
        [Authorize(Roles = "Administrator,Advisor")]
        [HttpPost]
        public ActionResult ChangePatientReadingFrequency(ReadingFrequencyChangeViewModel model)
        {
            //design taken from this example
            //http://stackoverflow.com/questions/9320036/using-two-button-in-a-form-calling-different-actions

            if (model.ActionType == "Daily")
            {
                _readingService.GenerateReadingExpectations(model.PatientId, model.StudyId, ReadingTypes.BloodPressure, ReadingFrequency.TwiceDaily, DateTime.Now.AddDays(1).Date);
                ViewBag.SuccessMessage = Constants.PatientSetDailyReadings;
            }
            else if (model.ActionType == "Weekly")
            {
                _readingService.GenerateReadingExpectations(model.PatientId, model.StudyId, ReadingTypes.BloodPressure, ReadingFrequency.OnceWeekly, DateTime.Now.Date);
                string nextReading = _readingService.GetDateOfNextReading(model.PatientId, model.StudyId, ReadingTypes.BloodPressure, ReadingFrequency.OnceWeekly, DateTime.Now.Date);
                ViewBag.SuccessMessage = Constants.PatientSetWeeklyReadings + nextReading;
            }

            return PartialView("_ChangePatientReadingFrequency");
        }
        [Authorize(Roles = "Administrator,Advisor")]
        [HttpPost]
        public ActionResult ViewBPReport(string patientId, string studyId, BloodPressureReadingsWrapper bloodPressureReadingsWrapper)
        {
            var readingsInDateRange = _readingService.GetBPReadingsForPatientWithinDateRangeGiven(
                                    patientId, studyId, bloodPressureReadingsWrapper.BPStartDateToSendToGP, 
                                    bloodPressureReadingsWrapper.BPEndDateToSendToGP);
            
            MemoryStream memStream = new MemoryStream();

            StudyPatient patient = _studiesService.GetPatient(patientId, studyId);
            _documentService.GenerateBloodPressurePDF(patient, readingsInDateRange, User.Identity.Name, null, memStream, false, Server.MapPath(Url.Content("~/content/themes/healthlines/images/letterlogo.png")));

            return File(memStream.ToArray(), "application/octet-stream", "CVDGpBloodPressureReadings_" + DateTime.Now.ToShortDateString() + ".pdf");
        }

        #region PartialView Actions

        // Get the details for the user to enter BP targets
        [Authorize(Roles = "Administrator,Advisor")]
        [HttpGet]
        public ActionResult BloodPressureArea(string patientID, string studyID, string callbackId)
        {
            BloodPressureReadingsWrapper bloodPressureReadingWrapper = new BloodPressureReadingsWrapper();
            bloodPressureReadingWrapper.BloodPressureReadings = _readingService.GetBPReadingsForPatient(patientID, studyID);

            var readingExpectedFrequency = _readingService.GetReadingExpectedFrequency(patientID, studyID, ReadingTypes.BloodPressure);

            bloodPressureReadingWrapper.AverageSystolicBPReading = _readingService.GetAverageReadingForTheLastSixDaysOrWeeks(readingExpectedFrequency, ReadingTypes.SystolicBP, patientID, studyID);
            bloodPressureReadingWrapper.AverageDiastolicBPReading = _readingService.GetAverageReadingForTheLastSixDaysOrWeeks(readingExpectedFrequency, ReadingTypes.DiastolicBP, patientID, studyID);


            if (bloodPressureReadingWrapper.AverageSystolicBPReading == string.Empty) 
            {
                // ensure the user gets a sensible message if there are not enough readings.
                bloodPressureReadingWrapper.AverageBPReadingFrequencyText = Constants.NoBPReadingsAverageForLessThan4ReadingsText;
            }
            else if (readingExpectedFrequency == ReadingFrequency.TwiceDaily)
            {
                bloodPressureReadingWrapper.AverageBPReadingFrequencyText = Constants.DailyBPReadingsAverageText;
            }
            else if (readingExpectedFrequency == ReadingFrequency.OnceWeekly)
            {
                bloodPressureReadingWrapper.AverageBPReadingFrequencyText = Constants.WeeklyBPReadingsAverageText;
            }
            else
                bloodPressureReadingWrapper.AverageBPReadingFrequencyText = Constants.NoBPReadingsAverageText;


            if (!string.IsNullOrEmpty(callbackId))
            {
                ViewBag.ReturnUrl = Url.Action("CallDetails", "CallBack", new { id = @Request.QueryString["CallbackID"] });
            }
            else
                ViewBag.ReturnUrl = null;

            return View("BloodPressureArea", bloodPressureReadingWrapper);
        }

        // Get the details for the user to enter BP targets
        [Authorize(Roles = "Administrator,Advisor")]
        [HttpGet]
        public ActionResult BloodPressureTargets(string patientID, string studyID)
        {
            ReadingTarget readingTarget = new ReadingTarget();
            BloodPressureTargetViewModel bloodPressureTargetViewModel = new BloodPressureTargetViewModel();

            // Get both the diastolic and systolic pressures for the blood pressure readings
            readingTarget = _readingService.GetPatientTarget(ReadingTypes.DiastolicBP, patientID, studyID);
            if (readingTarget != null)
                bloodPressureTargetViewModel.DiastolicTarget = readingTarget.Target;

            readingTarget = _readingService.GetPatientTarget(ReadingTypes.SystolicBP, patientID, studyID);
            if (readingTarget != null)
                bloodPressureTargetViewModel.SystolicTarget = readingTarget.Target;
        
            return PartialView("_CurrentTargetBPs", bloodPressureTargetViewModel);
        }

        // Get the 
        [Authorize(Roles = "Administrator,Advisor")]
        [HttpGet]
        public ActionResult SendPatientBloodPressuresToGP(string patientId, string studyId)
        {
            BloodPressureReadingsWrapper bloodPressureReadingWrapper = new BloodPressureReadingsWrapper();
            return PartialView("_SendPatientBloodPressuresToGP", bloodPressureReadingWrapper);
        }

        // Get the Patient Reading table
        [Authorize(Roles = "Administrator,Advisor")]
        [HttpGet]
        public ActionResult BloodPressureReadingsForPatient(string patientId, string studyId)
        {
            BloodPressureReadingsWrapper bloodPressureReadingWrapper = new BloodPressureReadingsWrapper();
            bloodPressureReadingWrapper.BloodPressureReadings = _readingService.GetBPReadingsForPatient(patientId, studyId);

            var readingExpectedFrequency = _readingService.GetReadingExpectedFrequency(patientId, studyId, ReadingTypes.BloodPressure);

            bloodPressureReadingWrapper.AverageSystolicBPReading = _readingService.GetAverageReadingForTheLastSixDaysOrWeeks(readingExpectedFrequency, ReadingTypes.SystolicBP, patientId, studyId);
            bloodPressureReadingWrapper.AverageDiastolicBPReading = _readingService.GetAverageReadingForTheLastSixDaysOrWeeks(readingExpectedFrequency, ReadingTypes.DiastolicBP, patientId, studyId);

            if (readingExpectedFrequency == ReadingFrequency.TwiceDaily)
            {
                bloodPressureReadingWrapper.AverageBPReadingFrequencyText = Constants.DailyBPReadingsAverageText;
            }
            else if (readingExpectedFrequency == ReadingFrequency.OnceWeekly)
            {
                bloodPressureReadingWrapper.AverageBPReadingFrequencyText = Constants.WeeklyBPReadingsAverageText;
            }
            else
                bloodPressureReadingWrapper.AverageBPReadingFrequencyText = Constants.NoBPReadingsAverageText;

            return PartialView("_PatientReadings", bloodPressureReadingWrapper);
        }
        
        #endregion
    }
}
