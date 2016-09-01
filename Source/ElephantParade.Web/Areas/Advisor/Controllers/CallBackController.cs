using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using NHSD.ElephantParade.Core.Interfaces;
using NHSD.ElephantParade.Core;
using NHSD.ElephantParade.Core.Models;
using NHSD.ElephantParade.Web.Areas.Advisor.Helpers;
using NHSD.ElephantParade.Web.Helpers;
using NHSD.ElephantParade.Web.Controllers;
using NHSD.ElephantParade.Domain.Models;
using NHSD.ElephantParade.Web.Areas.Advisor.Models;

namespace NHSD.ElephantParade.Web.Areas.Advisor.Controllers
{
    public class CallBackController : BaseController
    {
        #region Fields
        //Member Variables
        private IStudiesService _studiesService;        
        private ICallbackService _callbackService;
        private IReadingService _readingService;
        private IContentSectionStatusService _contentSectionStatusService;
        //constants
        private const int pagingRecordsPerPage = 10;
        private PaginatedViewModel<CallbackViewModel> scheduledCallbacks;
        private PaginatedViewModel<CallbackViewModel> newPatientsCallbacks;
        private PaginatedViewModel<CallbackViewModel> lockedCallbacks;
        private PaginatedViewModel<CallbackViewModel> lockedToUserCallbacks;
        #endregion
        
        #region Ctor
        public CallBackController(IStudiesService studiesService,            
            ICallbackService callbackService, IReadingService readingService, IContentSectionStatusService contentSectionStatusService)       
        {
            _studiesService = studiesService;
            _callbackService = callbackService;
            _readingService = readingService;
            _contentSectionStatusService = contentSectionStatusService;
        }
        #endregion

        #region Callback List Actions

        // Displaying the List of Patients
        /// <summary>
        /// 
        /// </summary>
        [Authorize(Roles = "Administrator,Advisor,Supervisor")]
        [HttpGet]
        public ActionResult PatientList()
        {
            var patientId = Request.QueryString["PatientId"];
            var studyId = Request.QueryString["StudyId"];

            scheduledCallbacks = _callbackService.Callbacks_List(1);
            ViewData.Add("NumberOfScheduledCallbacks", scheduledCallbacks.TotalRecordsCount);

            newPatientsCallbacks = _callbackService.Callbacks_List_PatientsNotCalled(1);
            ViewData.Add("NumberOfNewPatientsCallbacks", newPatientsCallbacks.TotalRecordsCount);

            lockedCallbacks = _callbackService.Callbacks_List_Locked(1);
            ViewData.Add("NumberOfCallbacksLocked", lockedCallbacks.TotalRecordsCount);

            lockedToUserCallbacks = _callbackService.Callbacks_ListLockedToUser(1);
            ViewData.Add("NumberOfCallbacksLockedToUser", lockedToUserCallbacks.TotalRecordsCount);

            return View();
        }

        [Authorize(Roles = "Administrator,Advisor,Supervisor")]
        [HttpGet]
        [NoCache]
        public JsonResult ScheduledCallbacks(int? page = 1)
        {
            scheduledCallbacks = _callbackService.Callbacks_List(PaginationHelper.PageIndexFromPage(page));

            if (scheduledCallbacks.Data.Count > 0)
            {
                UrlHelper urlHelper = new UrlHelper(ControllerContext.RequestContext);

                StringBuilder htmlStringBuilder = new StringBuilder(
                    @"<table id='ScheduledCallbacksTable' class='grid'>
                    <thead>
                        <tr>
                            <th>Study</th><th>Call Type</th><th>Name</th><th>Callback Date</th><th>Callback Time</th><th>Calls Completed</th><th>&nbsp;</th>
                        </tr>
                    </thead>
                    <tbody>");
                foreach (var item in scheduledCallbacks.Data)
                {
                    //determine if we need to mark the row as red
                    string trClass = (item.CallbackDate != null && item.CallbackDate < DateTime.Now.Date) ? "class='important'" : string.Empty;

                    htmlStringBuilder.Append("<tr "); htmlStringBuilder.Append(trClass); htmlStringBuilder.Append(">");
                    htmlStringBuilder.Append("<td>");
                    htmlStringBuilder.Append(item.Patient != null ? item.Patient.StudyID : string.Empty);
                    htmlStringBuilder.Append("</td>");
                    htmlStringBuilder.Append("<td>");
                    htmlStringBuilder.Append(item.Type.ToString());
                    htmlStringBuilder.Append("</td>");
                    htmlStringBuilder.Append("<td>");
                    htmlStringBuilder.Append(item.Patient != null ? item.Patient.DisplayName : string.Empty);
                    htmlStringBuilder.Append("</td>");
                    htmlStringBuilder.Append("<td>");
                    htmlStringBuilder.Append(item.CallbackDate != null ? item.CallbackDate.Value.ToShortDateString() : string.Empty);
                    htmlStringBuilder.Append("</td>");
                    htmlStringBuilder.Append("<td>");
                    string callbackTimes = string.Empty;

                    if (item.CallbackEndTime == null)
                        callbackTimes = item.CallbackStartTime.ToString();
                    else
                    {
                        callbackTimes = item.CallbackStartTime.ToString();
                        callbackTimes += " - ";
                        callbackTimes += item.CallbackEndTime.ToString();
                    }
                    htmlStringBuilder.Append(callbackTimes);
                    htmlStringBuilder.Append("</td>");
                    htmlStringBuilder.Append("<td>");
                    htmlStringBuilder.Append(item.CallsCompleted.ToString());
                    htmlStringBuilder.Append("</td>");
                    htmlStringBuilder.Append("<td>");
                    htmlStringBuilder.Append("<a href='" + urlHelper.Action("CallDetails", "CallBack", new { id = item.CallbackId }) + "'>View Record</a>");
                    htmlStringBuilder.Append("</td>");
                    htmlStringBuilder.Append("</tr>");
                }
                htmlStringBuilder.Append("</tbody>");
                htmlStringBuilder.Append("</table>");

                return Json(new
                {
                    Data = new HtmlString(htmlStringBuilder.ToString()).ToHtmlString(), //TA: I like this line of code a lot. Convert stringbuilder to string and instantiate HtmlString object, then convert HtmlString object to html encoded string.
                    TotalPageCount = PaginationHelper.GetNumberOfPages(scheduledCallbacks.TotalRecordsCount, pagingRecordsPerPage),
                    CurrentPage = page
                }, JsonRequestBehavior.AllowGet);
            }
            else
                return Json(new { Data = string.Empty, TotalPageCount = 0, CurrentPage = 0 }, JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = "Administrator,Advisor,Supervisor")]
        [HttpGet]
        [NoCache]
        public JsonResult NewPatientsList(int? page = 1)
        {
            newPatientsCallbacks = _callbackService.Callbacks_List_PatientsNotCalled(PaginationHelper.PageIndexFromPage(page));

            if (newPatientsCallbacks.Data.Count > 0)
            {
                UrlHelper urlHelper = new UrlHelper(ControllerContext.RequestContext);

                StringBuilder htmlStringBuilder = new StringBuilder(
                    @"<table id='NewPatientsListTable' class='grid'>
                    <thead>
                        <tr>
                            <th>Study</th><th>Call Type</th><th>Name</th><th>Callback Date</th><th>Callback Time</th><th>&nbsp;</th>
                        </tr>
                    </thead>
                    <tbody>");
                foreach (var item in newPatientsCallbacks.Data)
                {
                    //determine if we need to mark the row as red
                    string trClass = (item.CallbackDate != null && item.CallbackDate < DateTime.Now.Date) ? "class='important'" : string.Empty;

                    htmlStringBuilder.Append("<tr "); htmlStringBuilder.Append(trClass); htmlStringBuilder.Append(">");
                    htmlStringBuilder.Append("<td>");
                    htmlStringBuilder.Append(item.Patient != null ? item.Patient.StudyID : string.Empty);
                    htmlStringBuilder.Append("</td>");
                    htmlStringBuilder.Append("<td>");
                    htmlStringBuilder.Append(item.Type.ToString());
                    htmlStringBuilder.Append("</td>");
                    htmlStringBuilder.Append("<td>");
                    htmlStringBuilder.Append(item.Patient != null ? item.Patient.DisplayName : string.Empty);
                    htmlStringBuilder.Append("</td>");
                    htmlStringBuilder.Append("<td>");
                    htmlStringBuilder.Append(item.CallbackDate != null ? item.CallbackDate.Value.ToShortDateString() : string.Empty);
                    htmlStringBuilder.Append("</td>");
                    htmlStringBuilder.Append("<td>");
                    string callbackTimes = string.Empty;

                    if (item.CallbackEndTime == null)
                        callbackTimes = item.CallbackStartTime.ToString();
                    else
                    {
                        callbackTimes = item.CallbackStartTime.ToString();
                        callbackTimes += " - ";
                        callbackTimes += item.CallbackEndTime.ToString();
                    }
                    htmlStringBuilder.Append(callbackTimes);
                    htmlStringBuilder.Append("</td>");
                    htmlStringBuilder.Append("<td>");
                    htmlStringBuilder.Append("<a href='" + urlHelper.Action("CallDetails", "CallBack", new { id = item.CallbackId }) + "'>View Record</a>");
                    htmlStringBuilder.Append("</td>");
                    htmlStringBuilder.Append("</tr>");
                }
                htmlStringBuilder.Append("</tbody>");
                htmlStringBuilder.Append("</table>");

                return Json(new
                {
                    Data = new HtmlString(htmlStringBuilder.ToString()).ToHtmlString(), //TA: I like this line of code a lot. Convert stringbuilder to string and instantiate HtmlString object, then convert HtmlString object to html encoded string.
                    TotalPageCount = PaginationHelper.GetNumberOfPages(newPatientsCallbacks.TotalRecordsCount, pagingRecordsPerPage),
                    CurrentPage = page
                }, JsonRequestBehavior.AllowGet);
            }
            else
                return Json(new { Data = string.Empty, TotalPageCount = 0, CurrentPage = 0 }, JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = "Administrator,Advisor,Supervisor")]
        [HttpGet]
        [NoCache]
        public JsonResult LockedCallbacksList(int? page = 1)
        {
            lockedCallbacks = _callbackService.Callbacks_List_Locked(PaginationHelper.PageIndexFromPage(page));

            if (lockedCallbacks.Data.Count > 0)
            {
                UrlHelper urlHelper = new UrlHelper(ControllerContext.RequestContext);

                StringBuilder htmlStringBuilder = new StringBuilder(
                    @"<table id='lockedCallbacksListTable' class='grid'>
                    <thead>
                        <tr>
                            <th>Name</th><th>Condition Study</th><th>Locked to</th><th>Locked Date</th><th>&nbsp;</th>
                        </tr>
                    </thead>
                    <tbody>");

                foreach (var item in lockedCallbacks.Data)
                {
                    //determine if we need to mark the row as red
                    string trClass = (item.CallbackDate != null && item.CallbackDate < DateTime.Now.Date) ? "class='important'" : string.Empty;

                    htmlStringBuilder.Append("<td>");
                    htmlStringBuilder.Append(item.Patient != null ? item.Patient.DisplayName : string.Empty);
                    htmlStringBuilder.Append("</td>");
                    htmlStringBuilder.Append("<td>");
                    htmlStringBuilder.Append(item.Type.ToString());
                    htmlStringBuilder.Append("</td>");
                    htmlStringBuilder.Append("<td>");
                    htmlStringBuilder.Append(item.LockedTo.ToString());
                    htmlStringBuilder.Append("</td>");
                    htmlStringBuilder.Append("<td>");
                    htmlStringBuilder.Append(item.LockedDate.Value.ToShortDateString());
                    htmlStringBuilder.Append("</td>");

                    if (User.IsInRole("Administrator") || User.IsInRole("Supervisor"))
                    {
                        htmlStringBuilder.Append("<td>");
                        htmlStringBuilder.Append("<a title='click to unlock' href='" + urlHelper.Action("CloseAndUnlockRecord", "Callback", new { CallbackId = item.CallbackId }) + "'>Unlock</a>");
                        htmlStringBuilder.Append("</td>");
                    }
                    htmlStringBuilder.Append("</tr>");
                }
                htmlStringBuilder.Append("</tbody>");
                htmlStringBuilder.Append("</table>");

                return Json(new
                {
                    Data = new HtmlString(htmlStringBuilder.ToString()).ToHtmlString(), //TA: I like this line of code a lot. Convert stringbuilder to string and instantiate HtmlString object, then convert HtmlString object to html encoded string.
                    TotalPageCount = PaginationHelper.GetNumberOfPages(lockedCallbacks.TotalRecordsCount, pagingRecordsPerPage),
                    CurrentPage = page
                }, JsonRequestBehavior.AllowGet);
            }
            else
                return Json(new { Data = string.Empty, TotalPageCount = 0, CurrentPage = 0 }, JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = "Administrator,Advisor,Supervisor")]
        [HttpGet]
        [NoCache]
        public JsonResult LockedCallbacksToUserList(int? page = 1)
        {
            lockedToUserCallbacks = _callbackService.Callbacks_ListLockedToUser(PaginationHelper.PageIndexFromPage(1));

            if (lockedToUserCallbacks.Data.Count > 0)
            {
                UrlHelper urlHelper = new UrlHelper(ControllerContext.RequestContext);

                StringBuilder htmlStringBuilder = new StringBuilder(
                    @"<table id='lockedCallbacksToUserListTable' class='grid'>
                        <thead>
                            <tr>
                                <th>Name</th><th>Condition Study</th><th>Locked to</th><th>Locked Date</th><th>&nbsp;</th>
                            </tr>
                        </thead>
                        <tbody>");

                foreach (var item in lockedToUserCallbacks.Data)
                {
                    //determine if we need to mark the row as red
                    string trClass = (item.CallbackDate != null && item.CallbackDate < DateTime.Now.Date) ? "class='important'" : string.Empty;

                    htmlStringBuilder.Append("<td>");
                    htmlStringBuilder.Append(item.Patient != null ? item.Patient.DisplayName : string.Empty);
                    htmlStringBuilder.Append("</td>");
                    htmlStringBuilder.Append("<td>");
                    htmlStringBuilder.Append(item.Type.ToString());
                    htmlStringBuilder.Append("</td>");
                    htmlStringBuilder.Append("<td>");
                    htmlStringBuilder.Append(item.LockedTo.ToString());
                    htmlStringBuilder.Append("</td>");
                    htmlStringBuilder.Append("<td>");
                    htmlStringBuilder.Append(item.LockedDate.Value.ToShortDateString());
                    htmlStringBuilder.Append("</td>");
                    htmlStringBuilder.Append("<td>");
                    htmlStringBuilder.Append("<a href='" + urlHelper.Action("CallDetails", "CallBack", new { id = item.CallbackId }) + "'>View Record</a>");
                    htmlStringBuilder.Append("</td>");
                    htmlStringBuilder.Append("</tr>");
                }
                htmlStringBuilder.Append("</tbody>");
                htmlStringBuilder.Append("</table>");

                return Json(new
                {
                    Data = new HtmlString(htmlStringBuilder.ToString()).ToHtmlString(), //TA: I like this line of code a lot. Convert stringbuilder to string and instantiate HtmlString object, then convert HtmlString object to html encoded string.
                    TotalPageCount = PaginationHelper.GetNumberOfPages(lockedToUserCallbacks.TotalRecordsCount, pagingRecordsPerPage),
                    CurrentPage = page
                }, JsonRequestBehavior.AllowGet);
            }
            else
                return Json(new { Data = string.Empty, TotalPageCount = 0, CurrentPage = 0 }, JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = "Administrator,Advisor,Supervisor")]
        [HttpGet]
        [NoCache]
        public JsonResult SinglePatientCallbacksList(string patientId, string studyId, int? page = 1)
        {
            var singlePatientScheduledCallbacks = _callbackService.Callbacks_List(patientId, studyId, PaginationHelper.PageIndexFromPage(page));
            ViewData.Add("ScheduledCallbacksForSinglePatient", singlePatientScheduledCallbacks.TotalRecordsCount);

            if (singlePatientScheduledCallbacks.TotalRecordsCount > 0)
            {
                UrlHelper urlHelper = new UrlHelper(ControllerContext.RequestContext);

                StringBuilder htmlStringBuilder = new StringBuilder(
                    @"<table id='ScheduledSingleCallbackTable' class='grid'>
                        <thead>
                            <tr>
                                <th>Study</th><th>Call Type</th><th>Name</th><th>Callback Date</th><th>Callback Time</th><th>Calls Completed</th><th>&nbsp;</th>
                            </tr>
                        </thead>
                        <tbody>");

                foreach (var item in singlePatientScheduledCallbacks.Data)
                {
                    //determine if we need to mark the row as red
                    string trClass = (item.CallbackDate != null && item.CallbackDate < DateTime.Now.Date) ? "class='important'" : string.Empty;

                    htmlStringBuilder.Append("<tr "); htmlStringBuilder.Append(trClass); htmlStringBuilder.Append(">");
                    htmlStringBuilder.Append("<td>");
                    htmlStringBuilder.Append(item.Patient != null ? item.Patient.StudyID : string.Empty);
                    htmlStringBuilder.Append("</td>");
                    htmlStringBuilder.Append("<td>");
                    htmlStringBuilder.Append(item.Type.ToString());
                    htmlStringBuilder.Append("</td>");
                    htmlStringBuilder.Append("<td>");
                    htmlStringBuilder.Append(item.Patient != null ? item.Patient.DisplayName : string.Empty);
                    htmlStringBuilder.Append("</td>");
                    htmlStringBuilder.Append("<td>");
                    htmlStringBuilder.Append(item.CallbackDate != null ? item.CallbackDate.Value.ToShortDateString() : string.Empty);
                    htmlStringBuilder.Append("</td>");
                    htmlStringBuilder.Append("<td>");
                    string callbackTimes = string.Empty;

                    if (item.CallbackEndTime == null)
                        callbackTimes = item.CallbackStartTime.ToString();
                    else
                    {
                        callbackTimes = item.CallbackStartTime.ToString();
                        callbackTimes += " - ";
                        callbackTimes += item.CallbackEndTime.ToString();
                    }
                    htmlStringBuilder.Append(callbackTimes);
                    htmlStringBuilder.Append("</td>");
                    htmlStringBuilder.Append("<td>");
                    htmlStringBuilder.Append(item.CallsCompleted.ToString());
                    htmlStringBuilder.Append("</td>");
                    htmlStringBuilder.Append("<td>");
                    htmlStringBuilder.Append("<a href='" + urlHelper.Action("CallDetails", "CallBack", new { id = item.CallbackId }) + "'>View Record</a>");
                    htmlStringBuilder.Append("</td>");
                    htmlStringBuilder.Append("</tr>");
                }
                htmlStringBuilder.Append("</tbody>");
                htmlStringBuilder.Append("</table>");

                return Json(new
                {
                    Data = new HtmlString(htmlStringBuilder.ToString()).ToHtmlString(), //TA: I like this line of code a lot. Convert stringbuilder to string and instantiate HtmlString object, then convert HtmlString object to html encoded string.
                    TotalPageCount = PaginationHelper.GetNumberOfPages(singlePatientScheduledCallbacks.TotalRecordsCount, pagingRecordsPerPage),
                    CurrentPage = page
                }, JsonRequestBehavior.AllowGet);
            }
            else
                return Json(new { Data = string.Empty, TotalPageCount = 0, CurrentPage = 0 }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SinglePatientCallbacks(string patientId, string studyId)
        {
            var singlePatientScheduledCallbacks = _callbackService.Callbacks_List(1);
            ViewData.Add("ScheduledCallbacksForSinglePatient", singlePatientScheduledCallbacks.TotalRecordsCount);

            return PartialView("_SinglePatientCallbacks");
        }

        #endregion

        #region CallDetails Actions
        //Displaying the Individual Patient Details
        [Authorize(Roles = "Administrator,Advisor")]
        [HttpGet]
        public ActionResult CallDetails(Guid id)
        {
            CallbackViewModel callback = _callbackService.Callbacks_Get(id);

            if (callback != null)
            {    
                //check if we lock the callback record to current user
                if (callback.LockedDate != null  && callback.LockedTo.ToLower() != LoginHelper.GetLoggedInUser().ToLower())
                {
                    //ToDo: readonly view
                    throw new InvalidOperationException("This record is locked to another user.");
                }
                else if (callback.LockedDate == null)
                {                
                    _callbackService.LockCallbackRecord(id, LoginHelper.GetLoggedInUser());
                }

                ViewBag.DisplayPatientCallHistory = _callbackService.CallEvent_ListForPatient(callback.Patient.PatientId, callback.Patient.StudyID);

                return View(callback);
            }
            else
            {
                return RedirectToAction("PatientList");
            }
        }

        //gets callback event history
        [Authorize(Roles = "Administrator,Advisor")]
        [HttpGet]
        public ActionResult PatientCallEventHistory(Guid callbackID,string patientID,string studyID)
        {
            var eventData = _callbackService.CallEvent_ListForPatientAndCall(patientID, studyID, callbackID).OrderByDescending(c => c.Date).ToList();
            return PartialView("_PatientCallEventHistory", eventData);
        }

        //gets patient call comments history
        [Authorize(Roles = "Administrator,Advisor")]
        [HttpGet]
        public ActionResult PatientCommentsHistory(string patientID, string studyID)
        {
            var eventData = _callbackService.PatientComments_List(patientID, studyID).OrderByDescending(c => c.Date).ToList();
            return PartialView("_PatientCommentsHistory", eventData);
        }

        //adds a callback comment for a patient
        [Authorize(Roles = "Administrator,Advisor")]
        [HttpPost]
        public ActionResult UpdatePatientComments(PatientCallCommentViewModel patientCommentViewModel)
        {
            //get patient 
            var patient = _studiesService.GetPatient(patientCommentViewModel.PatientID, patientCommentViewModel.StudyID);

            if (patient == null)
                throw new ArgumentException(String.Format("No patient found for study:{0} patientID:{1}", patientCommentViewModel.PatientID, patientCommentViewModel.StudyID));

            patientCommentViewModel.UserID = LoginHelper.GetLoggedInUser();
            patientCommentViewModel.CallbackId = patientCommentViewModel.CallbackId == null || patientCommentViewModel.CallbackId == Guid.Empty ? new Guid(RouteData.Values["id"].ToString()) : patientCommentViewModel.CallbackId;
            if (patientCommentViewModel.CallbackId != null)
                patientCommentViewModel.Callback = _callbackService.Callbacks_Get(patientCommentViewModel.CallbackId.Value);

            if (ViewData.ModelState.IsValid)
            {
                _callbackService.PatientComments_Add(patientCommentViewModel);
                patientCommentViewModel.Text = "";
            }

            return PartialView("_AddComment", patientCommentViewModel);
        }

        [Authorize(Roles = "Administrator,Advisor")]        
        [HttpPost]
        public ActionResult DeletePatientComments(int commentID)
        {
            var comment = _callbackService.PatientComments_Get(commentID);

            //only allow the user who added the comment to remove it.
            if (comment != null && comment.UserID.ToLower() == LoginHelper.GetLoggedInUser())
            {
                _callbackService.PatientComments_Delete(commentID);
                return Json(new { success = true });
            }
            return Json(new {success = false});
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="callbackId"></param>
        /// <returns></returns>
        [Authorize(Roles = "Administrator,Advisor")]
        public ActionResult ArrangeCallBackQuestionsCompleted(string callbackId)
        {
            if (!string.IsNullOrWhiteSpace(callbackId))
            {
                var cbvm = _callbackService.Callbacks_Get(Guid.Parse(callbackId));
                //check if the callback was requested by the patient (unscheduled callback)
                if (cbvm.Type == CallbackViewModel.CallbackType.Patient)                
                    ViewData["UnscheduledCallback"] = true;
                
            }
            return PartialView("_ArrangeCallBackQuestionsComplete", new CallbackViewModel());
        }

        [Authorize(Roles = "Administrator,Advisor")]
        [HttpPost]
        public ActionResult ArrangeCallBackQuestionsCompletedNew(FormCollection form, CallbackViewModel callbackViewModel, string patientId, string studyID, string callbackId)
        {
            return PartialView("_ArrangeCallBackQuestionsComplete", AddCallback(form, callbackViewModel, patientId, studyID, callbackId));
        }

        //arrange a new callback 
        [Authorize(Roles = "Administrator,Advisor")]
        [HttpPost]
        public ActionResult ArrangeCallBack(FormCollection form, CallbackViewModel callbackViewModel, string patientId, string studyID, string callbackId)
        {
            return PartialView("_ArrangeCallBackPartialView", AddCallback(form, callbackViewModel, patientId, studyID, callbackId));
        }

        [Authorize(Roles = "Administrator,Advisor")]
        public CallbackViewModel AddCallback(FormCollection form, CallbackViewModel callbackViewModel, string patientId, string studyID, string callbackId)
        {
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
                ViewData.ModelState.AddModelError("Call Back Date Cannot be Empty", string.Empty);
                carryOnFlag = false;
            }

            //Check for the callback starttime
            if (!string.IsNullOrEmpty(form["AvailibilityFrom"]))
            {

                callbackViewModel.CallbackStartTime = TimeSpan.Parse(form["AvailibilityFrom"]);
            }
            else
            {
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

            // Check the call handler has selected a call outcome before rescheduling
            if (string.IsNullOrEmpty(form["CallOutcome"]))
            {
                ViewData["CallOutcomeRequiredError"] = "Please select the call outcome before rescheduling.";
                carryOnFlag = false;
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

            // If all the Fields are filled then submit the page, else display the page with data 
            if (carryOnFlag && ModelState.IsValid)
            {
                if(callbackViewModel.CallOutcome.ToLower().Trim() == Constants.EncounterCompleteText.ToLower())
                {
                    Guid callid;
                    if(callbackId!=null && Guid.TryParse( callbackId,out callid ))
                    {
                        string outcome = callbackViewModel.CallOutcome;
                        callbackViewModel.CallOutcome = String.Empty;
                        callbackViewModel.CallbackId = Guid.Empty;
                        _callbackService.CallBack_AddAndCompletePrevious(callbackViewModel, callid, outcome);
                    }
                    else
                    {
                        ViewData["ArrangeCallBackState"] = "UserInputFailed";
                        ViewData["CallbackScheduled"] = "";
                        return callbackViewModel;
                    }
                }
                else
                {
                    callbackViewModel.CallbackId = Guid.Empty;
                    string outcome = callbackViewModel.CallOutcome;
                    callbackViewModel.CallOutcome = String.Empty;
                    // Intstead put the incomplete text to the old records that has been rescheduled (i.e. not active record)
                    //_callbackService.MarkPatientCallbacksWithPreviousCallsAsRescheduled(patientId, studyID);
                    _callbackService.CallBack_Add(callbackViewModel, outcome);
                }
                ViewData["CallbackScheduled"] = "Call Back Scheduled Successfully";
                return callbackViewModel;
            }
            else
            {
                ViewData["ArrangeCallBackState"] = "UserInputFailed";
                ViewData["CallbackScheduled"] = "";
                return callbackViewModel;
            }
        }

        //unlocks callback entry for others
        [Authorize(Roles = "Administrator,Supervisor,Advisor")]
        [HttpGet]
        public ActionResult CloseAndUnlockRecord(Guid CallbackID)
        {
            var callback = _callbackService.Callbacks_GetAnyType(CallbackID);
            if (callback == null)
                return RedirectToRoute("PatientList");

            //allow administrator to unlock any record, also unlock records not assigned to anyone. Allow advisors to unlock records locked to themselves
            if (User.IsInRole("Administrator") || User.IsInRole("Supervisor") || string.IsNullOrEmpty(callback.LockedTo) || callback.LockedTo.ToLower() == LoginHelper.GetLoggedInUser())
            {
                _callbackService.UnLockCallbackRecord(CallbackID, LoginHelper.GetLoggedInUser());
            }
            return RedirectToRoute("PatientList");
        }
        
        #endregion

        #region Patient Actions

        #region Patient Details 
        // returns details of the patient 
        [Authorize(Roles = "Administrator,Advisor")]
        [HttpGet]
        public ActionResult PatientDetailsOnly(string PatientId, string studyID)
        {
            if (studyID != null && PatientId != null)
            {
                var pvm = _studiesService.GetPatient(PatientId, studyID);
                return PartialView("_PatientDetailsPartialView", pvm);
            }
            else
            {
                TempData["Message"] = "PatientId is Null";
                return RedirectToAction("Error", "Error");
            }
        }

        // Updates the patient details but only those 
        [Authorize(Roles = "Administrator,Advisor")]
        [HttpPost]
        public ActionResult UpdatePatientDetails(ElephantParade.Domain.Models.StudyPatient pVM)
        {
            if (ViewData.ModelState.IsValid)
            {
                //Get the PatientObject
                var pObj = _studiesService.GetPatient(pVM.PatientId, pVM.StudyID);

                //update only the values we want 

                //Get the Values from the form 
                pObj.Title = pVM.Title;
                pObj.Forename = pVM.Forename;
                pObj.Surname = pVM.Surname;
                pObj.Ethnicity = pVM.Ethnicity;
                pObj.Gender = pVM.Gender;
                pObj.DOB = pVM.DOB;
                pObj.PreferredContactNumber = pVM.PreferredContactNumber;
                if (pVM.Address != null)
                    pObj.Address = pVM.Address;
                pObj.TelephoneNumber = pVM.TelephoneNumber;
                pObj.TelephoneNumberMobile = pVM.TelephoneNumberMobile;
                pObj.TelephoneNumberOther = pVM.TelephoneNumberOther;
                pObj.PrederredContactTime = pVM.PrederredContactTime;
                pObj.PreferredContactNumber = pVM.PreferredContactNumber;
                pObj.Email = pVM.Email;
                pObj.OnAntidepressants = pVM.OnAntidepressants;
                //Update to DB
                _studiesService.UpdatePatient(pObj, LoginHelper.GetLoggedInUser());

                // Get the updated data and populate it in panel
                pObj = _studiesService.GetPatient(pVM.PatientId, pVM.StudyID);
                ViewBag.SubmitMessage = "Patient details updated";
                return PartialView("_PatientDemographics", pObj);
            }
            else
            {
                //return new HttpStatusCodeResult(500, errorMessage);
                ViewBag.SubmitMessage = "Update failed. Changes to patient details were not saved.";
                return PartialView("_PatientDemographics", pVM);
            }
        }

        //Updated the patients status
        [Authorize(Roles = "Administrator,Advisor")]
        [HttpPost]
        public ActionResult UpdatePatientStatus(string patientID, string studyID, string patientStatus)
        {
            NHSD.ElephantParade.Domain.Models.StudyPatient.PatientStudyStatusType newstatus;
            newstatus = (NHSD.ElephantParade.Domain.Models.StudyPatient.PatientStudyStatusType)Enum.Parse(typeof(NHSD.ElephantParade.Domain.Models.StudyPatient.PatientStudyStatusType), patientStatus, true);
            
            var pObj = _studiesService.GetPatient(patientID, studyID);
            pObj.Status = newstatus;

            _studiesService.UpdatePatient(pObj, LoginHelper.GetLoggedInUser());

            return Content("");
        }

        // This is event for button which will be used to save the telephone number of the Patient
        [Authorize(Roles = "Administrator,Advisor")]
        [HttpPost]
        public ActionResult UpdateGPDetails(string patientID, string studyID, ElephantParade.Domain.Models.GeneralPractitioner gpVM)
        {
            if (ViewData.ModelState.IsValid)
            {
                //Get the PatientObject
                var pObj = _studiesService.GetPatient(patientID, studyID);

                //update only the values we want 
                pObj.GPPractice.Name = gpVM.Name;
                pObj.GPPractice.Practice = gpVM.Practice;
                pObj.GPPractice.Address = gpVM.Address;
                pObj.GPPractice.EmailAddress = gpVM.EmailAddress;
                pObj.GPPractice.PrimaryCareTrust = gpVM.PrimaryCareTrust;
                pObj.GPPractice.TelephoneNumber = gpVM.TelephoneNumber;

                //Update to DB
                _studiesService.UpdatePatient(pObj, LoginHelper.GetLoggedInUser());

                ViewBag.SubmitMessage = "GP details updated";
                return PartialView("_GPDetails", pObj.GPPractice);
            }
            else
            {
                ViewBag.SubmitMessage = "Update Failed. Changes to GP details not saved";
                return PartialView("_GPDetails", gpVM);
            }
        }

        #endregion

        #region Medical Condition actions
        [Authorize(Roles = "Administrator,Advisor")]        
        public ActionResult MedicalConditions(string patientID, string studyID)
        {
            MedicationWrapper m = new Models.MedicationWrapper();
            PatientMedicalConditions medData = _studiesService.MedicalConditions_ListByPatient(studyID, patientID);
            m.PatientMedicalConditions = medData;
            return PartialView("_Medication", m);
        }


        //Created or updated medicat condition item or a patient
        [Authorize(Roles = "Administrator,Advisor")]
        [HttpPost]
        //[OutputCache(NoStore = true, Duration = 1, VaryByParam = "*")]
        public ActionResult MedicalConditionAdd(string patientID, string studyID, MedicationWrapper item)
        {
            if (ViewData.ModelState.IsValid)
            {
                _studiesService.MedicalConditions_Save(studyID, patientID, item.NewMedicalConditionItem);                
                return Json(new { success = true });                    
            }
            else
            {
                string errorMessage = "";
                foreach (var modelStateValue in ViewData.ModelState.Values)
                {
                    foreach (var error in modelStateValue.Errors)
                    {
                        // ToDo something useful with these properties
                        errorMessage = error.ErrorMessage;
                        var exception = error.Exception;
                    }
                }
                //return new HttpStatusCodeResult(500, errorMessage);
                return Json(new { success = false, errormessage = errorMessage });
            }
        }

        //Created or updated medicat condition item or a patient
        [Authorize(Roles = "Administrator,Advisor")]
        [HttpPost]
        //[OutputCache(NoStore = true, Duration = 1, VaryByParam = "*")]
        public ActionResult MedicalConditionSave(string patientID, string studyID, MedicalConditionItem item)
        {
            if (ViewData.ModelState.IsValid)
            {
                _studiesService.MedicalConditions_Save(studyID, patientID, item);                
                return Json(new { success = true });                    
            }
            return PartialView("_MedicalConditionItem", item);
        }


        [Authorize(Roles = "Administrator,Advisor")]
        [HttpGet]
        public ActionResult MedicalConditionDelete(string studyID,int itemID)
        {
            _studiesService.MedicalConditions_Delete(studyID, itemID);

            return Content("item deleted");
        }
        #endregion

        #endregion

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
