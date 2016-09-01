using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Elmah;
using NHSD.ElephantParade.Core.Interfaces;
using NHSD.ElephantParade.Core;
using NHSD.ElephantParade.Core.Models;
using NHSD.ElephantParade.Web.Areas.Advisor.Helpers;
using NHSD.ElephantParade.Web.Helpers;
using NHSD.ElephantParade.Web.Controllers;
using NHSD.ElephantParade.Domain.Models;
using NHSD.ElephantParade.Web.Authentication;

namespace NHSD.ElephantParade.Web.Controllers
{
    public class PatientCallbackController : BaseController
    {
        //
        // GET: /Callback/
        #region Fields
        //Member Variables
        private IStudiesService _studiesService;
        private ICallbackService _callbackService;
        private IContentSectionStatusService _contentSectionStatusService;
        #endregion

        #region Ctor
        public PatientCallbackController(IStudiesService studiesService,            
            ICallbackService callbackService,
            IContentSectionStatusService contentSectionStatusService)       
        {
            _studiesService = studiesService;
            _callbackService = callbackService;
            _contentSectionStatusService = contentSectionStatusService;
        }
        #endregion

        [HttpGet]
        public ActionResult PatientArrangeCallback()
        {
            return View();
        }

        //Displaying the Individual Patient Details
        [Authorize(Roles = "CVD, Depression")]
        [HttpPost]
        public ActionResult ArrangeCallBack(FormCollection form, CallbackViewModel callbackViewModel, string patientId, string studyID)
        {
            callbackViewModel.Type = CallbackViewModel.CallbackType.Patient;

            //check authorised to update the specified patient details
            if (User.Identity is HealthLinesParticipantIdentity && ((HealthLinesParticipantIdentity)User.Identity).PatientId != null)
            {
                if (((HealthLinesParticipantIdentity)User.Identity).PatientId != patientId || ((HealthLinesParticipantIdentity)User.Identity).StudyID != studyID)
                    throw new UnauthorizedAccessException("Not authorised to update this patients details.");
            }

            //Varibale to store the any validation errors
            bool carryOnFlag = true;

            //Check for callback date
            if (form["CallbackDate"] != string.Empty)
            {
                string date = form["CallbackDate"];

                DateTime changedFormatDate;
                DateTime.TryParse(date, out changedFormatDate);
                callbackViewModel.CallbackDate = changedFormatDate;

                var disabledDates = _contentSectionStatusService.SectionDisabledDates(ContentSectionTypes.PatientCallback, null);

                if (disabledDates.Contains(callbackViewModel.CallbackDate.Value))
                {
                    ViewData.ModelState.AddModelError("Unavailable Callback Date Entered", "Unavailable Callback Date Entered");
                    carryOnFlag = false;
                }
            }
            else
            {
                ViewData.ModelState.AddModelError("Callback Date Cannot be Empty", string.Empty);
                carryOnFlag = false;
            }

            //Check for the callback starttime
            if (!string.IsNullOrEmpty(form["AvailibilityFrom"]))
            {
                callbackViewModel.CallbackStartTime = TimeSpan.Parse(form["AvailibilityFrom"]);
            }
            else
            {
                ViewData.ModelState.AddModelError("Available From Cannot be Empty", string.Empty);
                carryOnFlag = false;
            }

            //Check for the callback end time, if not selected a value then pass null value
            if (!string.IsNullOrEmpty(form["AvailibilityTo"]))
            {
                callbackViewModel.CallbackEndTime = TimeSpan.Parse(form["AvailibilityTo"]);
            }
            else
            {
                callbackViewModel.CallbackEndTime = null;

            }

            //Check the Patient Guid if Null then callback is for this patient
            if (patientId != null)
            {
                var patientDetails = _studiesService.GetPatient(patientId, studyID);
                callbackViewModel.Patient = patientDetails;
            }
            else
            {
                ViewData.ModelState.AddModelError("PatientId is not Passed", "Error with Patient record");
                carryOnFlag = false;
            }


            

            callbackViewModel.CallbackScheduledBy = LoginHelper.GetLoggedInUser();
            callbackViewModel.Type = CallbackViewModel.CallbackType.Patient;

            // If all the Fields are filled then submit the page, else display the page with data 
            if (carryOnFlag && ModelState.IsValid)
            {
                //var patientObj = _patientService.GetPatient(patientId,studyID);
                try
                {
                    _callbackService.CallBack_Add(callbackViewModel);
                }
                catch (System.Net.Mail.SmtpException ex)
                {
                    //do nothing id email failed
                    ErrorSignal.FromCurrentContext().Raise(ex);
                }
                ViewData["CallbackScheduled"] = "Callback Scheduled Successfully";
                return PartialView("_ArrangeCallBackPartialView", callbackViewModel);
            }
            else
            {
                ViewData["ArrangeCallBackState"] = "UserInputFailed";

                ViewData["CallbackScheduled"] = "";
                return PartialView("_ArrangeCallBackPartialView", callbackViewModel);
            }
        }

        public ActionResult Index()
        {
            return View();
        }

        //
        // GET: /Callback/Details/5

        public ActionResult Details(int id)
        {
            return View();
        }

        //
        // GET: /Callback/Create

        public ActionResult Create()
        {
            return View();
        } 

        //
        // POST: /Callback/Create

        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
        
        //
        // GET: /Callback/Edit/5
 
        public ActionResult Edit(int id)
        {
            return View();
        }

        //
        // POST: /Callback/Edit/5

        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here
 
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /Callback/Delete/5
 
        public ActionResult Delete(int id)
        {
            return View();
        }

        //
        // POST: /Callback/Delete/5

        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here
 
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        /// <summary>
        /// Returns a list of dates to be disabled (prevented from being selected) for Patient Callback.
        /// </summary>
        /// <returns>JSON result of array of dates to be disabled.</returns>
        public ActionResult GetDisabledDates(string studyId)
        {
            var result = CallbackHelper.GetDisabledDates(studyId, _contentSectionStatusService);

            return Json(new { disabledDatesArray = result }, JsonRequestBehavior.AllowGet);
        }
    }
}
