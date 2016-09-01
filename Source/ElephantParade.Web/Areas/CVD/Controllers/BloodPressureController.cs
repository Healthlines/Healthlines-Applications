using System;
using System.Web.Mvc;
using NHSD.ElephantParade.Core;
using NHSD.ElephantParade.Core.Interfaces;
using NHSD.ElephantParade.Core.Models;
using NHSD.ElephantParade.Domain.Models;
using NHSD.ElephantParade.Web.Areas.Advisor.Models;

namespace NHSD.ElephantParade.Web.Areas.CVD.Controllers
{
    public class BloodPressureController : Controller
    {
        private IReadingService _readingService;

        public BloodPressureController() : this(new ReadingService())
        {
            
        }

        public BloodPressureController(IReadingService readingService)
        {
            _readingService = readingService ?? new ReadingService();
        }
        //
        // GET: /CVD/BloodPressure/

        public ActionResult Index()
        {
            return View();
        }

        // Get all the blood pressure readings (history) for the patient
        [Authorize(Roles = "CVD,Depression")]
        public ActionResult History()
        {
            var hlIdentity = User.Identity as NHSD.ElephantParade.Web.Authentication.HealthLinesParticipantIdentity;

            if (hlIdentity == null || string.IsNullOrEmpty(hlIdentity.StudyID) || string.IsNullOrEmpty(hlIdentity.PatientId))
            {
                throw new ArgumentException("User not set up correctly in healthlines", "hlIdentity");
            }

            var bpReadingsEnteredForPatient = _readingService.GetBPReadingsEnteredForPatient(hlIdentity.PatientId, hlIdentity.StudyID);

            BloodPressureReadingsWrapper bloodPressureReadingWrapper = new BloodPressureReadingsWrapper();
            bloodPressureReadingWrapper.BloodPressureReadings = _readingService.GetBPReadingsForPatient(hlIdentity.PatientId, hlIdentity.StudyID);

            var readingExpectedFrequency = _readingService.GetReadingExpectedFrequency(hlIdentity.PatientId, hlIdentity.StudyID, ReadingTypes.BloodPressure);

            bloodPressureReadingWrapper.AverageSystolicBPReading = _readingService.GetAverageReadingForTheLastSixDaysOrWeeks(readingExpectedFrequency, ReadingTypes.SystolicBP, hlIdentity.PatientId, hlIdentity.StudyID);
            bloodPressureReadingWrapper.AverageDiastolicBPReading = _readingService.GetAverageReadingForTheLastSixDaysOrWeeks(readingExpectedFrequency, ReadingTypes.DiastolicBP, hlIdentity.PatientId, hlIdentity.StudyID);
            bloodPressureReadingWrapper.ValidReadingsCount = bpReadingsEnteredForPatient.Count;

            if (bloodPressureReadingWrapper.AverageSystolicBPReading == string.Empty)
            {
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
                bloodPressureReadingWrapper.AverageBPReadingFrequencyText = Constants.NoBPReadingsAverageForLessThan4ReadingsText;

            return PartialView("_BPHistory", bloodPressureReadingWrapper);
        }


        [Authorize(Roles = "CVD,Depression")]
        public ActionResult NewReading()
        {
            return PartialView("_BPNewReadingMain");
        }

        [Authorize(Roles = "CVD,Depression")]
        public ActionResult Target()
        {
            var hlIdentity = User.Identity as Authentication.HealthLinesParticipantIdentity;

            if (hlIdentity == null || string.IsNullOrEmpty(hlIdentity.StudyID) || string.IsNullOrEmpty(hlIdentity.PatientId))
            {
                throw new ArgumentException("User not set up correctly in healthlines", "hlIdentity");
            }

            ReadingTarget readingTarget = new ReadingTarget();
            BloodPressureTargetViewModel bloodPressureTargetViewModel = new BloodPressureTargetViewModel();

            // Get both the diastolic and systolic pressures for the blood pressure readings
            readingTarget = _readingService.GetPatientTarget(ReadingTypes.DiastolicBP, hlIdentity.PatientId, hlIdentity.StudyID);
            if (readingTarget != null)
                bloodPressureTargetViewModel.DiastolicTarget = readingTarget.Target;

            readingTarget = _readingService.GetPatientTarget(ReadingTypes.SystolicBP, hlIdentity.PatientId, hlIdentity.StudyID);
            if (readingTarget != null)
                bloodPressureTargetViewModel.SystolicTarget = readingTarget.Target;

            return PartialView("_BPTarget", bloodPressureTargetViewModel);
        }

        [Authorize(Roles = "CVD,Depression")]
        [HttpPost]
        public ActionResult SubmitNewReading(FormCollection formCollection, BloodPressureReadingViewModel bpvm)
        {
            var hlIdentity = User.Identity as Authentication.HealthLinesParticipantIdentity;

            if (hlIdentity == null || string.IsNullOrEmpty(hlIdentity.StudyID) || string.IsNullOrEmpty(hlIdentity.PatientId))
            {
                throw new ArgumentException("User not set up correctly in healthlines", "hlIdentity");
            }
            
            //retrieve time element from it's own textbox
            string timeString = formCollection["TimeReadingTaken"];

            //as datetime is split into two boxes (one for date, one for time), combine into DateTime field.
            try
            {
                if (bpvm.DateOfReading != null)
                {
                    DateTime readingDate = bpvm.DateOfReading.Value;
                    if (timeString != null)
                        bpvm.DateOfReading = readingDate.Add(TimeSpan.Parse(timeString));

                    if (bpvm.DateOfReading > DateTime.Now)
                    {
                        bpvm.DateOfReading = null;
                        ModelState.AddModelError(Constants.DateTimeBPError, Constants.DateTimeBPError);
                    }
                }
                else //null reference
                {
                    bpvm.DateOfReading = null;
                    ModelState.AddModelError(Constants.DateTimeBPError, Constants.DateTimeBPError);
                }
            }
            catch(NullReferenceException)
            {
                bpvm.DateOfReading = null;
                ModelState.AddModelError(Constants.DateTimeBPError, Constants.DateTimeBPError);
            }
            catch(FormatException)
            {
                bpvm.DateOfReading = null;
                ModelState.AddModelError(Constants.DateTimeBPError, Constants.DateTimeBPError);
            }
            catch (OverflowException)
            {
                bpvm.DateOfReading = null;
                ModelState.AddModelError(Constants.DateTimeBPError, Constants.DateTimeBPError);
            }
            
            bpvm.StudyId = hlIdentity.StudyID;
            bpvm.PatientId = hlIdentity.PatientId;
            bpvm.SubmittedBy = hlIdentity.Name;

            if (ModelState.IsValid)
            {
                var readingSuccessObject = _readingService.AddReading(bpvm);
                return PartialView("_BPNewReadingSuccess", readingSuccessObject);
            }

            return PartialView("_BPNewReading", bpvm);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="patientId"></param>
        /// <param name="studyId"></param>
        /// <returns></returns>
        [Authorize(Roles = "Administrator,Advisor")]
        [HttpGet]
        public JsonResult BPDataForPatient(string patientId, string studyId)
        {            
            if (User.IsInRole("Advisor"))
                return Json(_readingService.GetBPReadingsForPatient(patientId, studyId), JsonRequestBehavior.AllowGet);
            else
            {
                var hlIdentity = User.Identity as Authentication.HealthLinesParticipantIdentity;

                if (hlIdentity == null )                
                    throw new ArgumentException("User not set up correctly in healthlines", "hlIdentity"); 

                return Json(_readingService.GetBPReadingsForPatient(hlIdentity.PatientId, hlIdentity.StudyID), JsonRequestBehavior.AllowGet);
            }
        }

        [Authorize(Roles = "CVD,Depression")]
        public ActionResult DownloadTakingReadingInfo(string fileName)
        {
            return File(fileName, "application/octet-stream", "HealthlinesServiceTakingYourBlood PressureInformation.pdf");
        }

    }
}
