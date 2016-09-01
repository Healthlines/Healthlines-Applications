using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Questionnaires.Core.Services;
using Questionnaires.Core.Services.Models;
using Questionnaires.Web.Models;
using Questionnaires.Web.Helpers;
using System.IO;
using NHSD.ElephantParade.DocumentGenerator.Interfaces;
using NHSD.ElephantParade.Web.Areas.Advisor.Helpers;
using NHSD.ElephantParade.Web.Authentication;
using NHSD.ElephantParade.Web.Controllers;
using NHSD.ElephantParade.Core.Interfaces;
using NHSD.ElephantParade.DocumentGenerator;
using NHSD.ElephantParade.Domain.Models;
using NHSD.ElephantParade.Web.Areas.Advisor.Models;
using Questionnaires.Core.Services.Exceptions;

namespace NHSD.ElephantParade.Web.Areas.Advisor.Controllers
{
    public class QuestionnaireSessionController : BaseController
    {
        #region fields
        private IQuestionnaireService _questionnaireService;
        private IStudiesService _studiesService;
        private IDocumentService _documentService;
        private ICallbackService _callbackService;
        private IContentSectionStatusService _contentSectionStatusService;
        #endregion

        #region Ctor
        public QuestionnaireSessionController(IQuestionnaireService questionnaireService, IStudiesService studiesService,IDocumentService documentService,ICallbackService callbackService, IContentSectionStatusService contentSectionStatusService)
        {
            _questionnaireService = questionnaireService;
            _studiesService = studiesService;
            _documentService = documentService;
            _callbackService = callbackService;
            _contentSectionStatusService = contentSectionStatusService;
        }
        #endregion

        [Authorize(Roles = "Administrator,Advisor")]
        public ActionResult PatientQuestionnairesGetEncountersOnly(string patientID, string studyID)
        {
            if ((patientID == null || studyID == null) && User.Identity is HealthLinesParticipantIdentity)
            {
                patientID = ((HealthLinesParticipantIdentity)User.Identity).PatientId;
                studyID = ((HealthLinesParticipantIdentity)User.Identity).StudyID;
            }
            else if (patientID == null)
                return Content("");

            ViewBag.PatientID = patientID;
            ViewBag.StudyID = _studiesService.CVDStudyID;

            if (studyID == _studiesService.CVDStudyID)
            {
                //CVD
                return Content("");
            }
            else if (studyID == _studiesService.DepressionStudyID)
            {
                //depression
                var participantID = _studiesService.DepressionService.QuestionnaireParticipantIDGet(patientID);
                QuestionnaireSetsWrapper vm = new QuestionnaireSetsWrapper()
                {
                    ParticipantID = participantID,
                    //PatientLetterActions = patientLetterActions,
                    QuestionnaireSessions = _studiesService.DepressionService.QuestionnaireSessionList(patientID),
                    Questionnaires = _questionnaireService.QuestionnaireList().OrderBy(q => q.Name).ToList()
                };
                return PartialView("_QuestionnairesEncountersOnly", vm);
            }
            else
                //unknown study so do nothing as questionnaires are not supported
                return Content("");
        }

        /// <summary>
        /// returns a list of completed and available questionniares
        /// </summary>
        /// <param name="patientID"></param>
        /// <param name="studyID"></param>
        /// <param name="callbackID"></param>
        /// <returns></returns>
        [Authorize(Roles = "Administrator,Advisor")]
        public ActionResult PatientQuestionnairesGet(string patientID, string studyID,string callbackID)
        {
            if ((patientID == null || studyID == null) && User.Identity is HealthLinesParticipantIdentity)
            {
                patientID = ((HealthLinesParticipantIdentity)User.Identity).PatientId;
                studyID = ((HealthLinesParticipantIdentity)User.Identity).StudyID;
            }
            else if (patientID == null)
                return RedirectToAction("Index", "Depression");

            ViewBag.callbackID = callbackID;
            ViewBag.PatientID = patientID;
          //  ViewBag.StudyID = _studiesService.CVDStudyID;

            if (studyID == _studiesService.CVDStudyID)
            {
                //CVD                
                // This is used to pass data to the partial view for setting BP targets
                ViewData["Callback"] = _callbackService.Callbacks_Get(new Guid(callbackID));             
                return PartialView("_CVD", _studiesService.CvdService.QuestionnaireSessionList(patientID));
            }
            else if (studyID == _studiesService.DepressionStudyID)
            {
                //depression
                var participantID = _studiesService.DepressionService.QuestionnaireParticipantIDGet(patientID);                
                QuestionnaireSetsWrapper vm = new QuestionnaireSetsWrapper(){
                    ParticipantID = participantID,                   
                    //PatientLetterActions = patientLetterActions,
                    QuestionnaireSessions = _studiesService.DepressionService.QuestionnaireSessionList(patientID),
                    Questionnaires = _questionnaireService.QuestionnaireList().OrderBy(q=>q.Name).ToList()
                };
                return PartialView("_Questionnaires", vm);
            }
            else            
                //unknown study so do nothing as questionnaires are not supported
                return Content("");            
        }

        /// <summary>
        /// Starts a new questionnaire answer session
        /// </summary>
        /// <param name="questionnaireID"></param>
        /// <param name="participantID"></param>
        /// <returns></returns>
        [Authorize(Roles = "Administrator,Advisor")]
        public ActionResult Start(int questionnaireID, string participantID)
        {
            //check if we have an open session for the requested Questionnarie  close them
            IList<AnswerSet> answerSets = _questionnaireService.AnswerSetList(participantID);          
            if (answerSets != null)
            {
                answerSets = answerSets.Where(a => a.QuestionnaireID == questionnaireID && a.Status == AnswerSet.State.Open).ToList();                
                foreach (var item in answerSets)                
                    _questionnaireService.AnswerSetClose(item.AnswerSetID, "Starting again");                
            }

            //start a new session for the requested questionnaire
            AnswerSet answerSet = _questionnaireService.AnswerSetStart(participantID, User.Identity.Name, DateTime.Now, questionnaireID);

            return RedirectToAction("Resume", "QuestionnaireSession", new { id = answerSet.AnswerSetID });
        }

        
        /// <summary>
        /// Resumes an existing questionnaire
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = "Administrator,Advisor")]
        public ActionResult Resume(int id)
        {
            //Response.Cache.SetCacheability(HttpCacheability.NoCache);

            AnswerSet answerSet = _questionnaireService.AnswerSetGet(id);
            if(answerSet == null)
                return Error("Invalid Session","The Session you are trying to resume is not found.");

            IPageable<QuestionSetPageItem> pageItems;
            try
            {
                pageItems = _questionnaireService.AnswerSetGetActiveQuestion(id);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                return Error("Error Resuming Session", "An error occured reading the session data. The error has been logged.");
            }

            if (pageItems.Page.Count() > 0)
            {
                ViewBag.QuestionSetID = pageItems.Page.First().QuestionSetID;
                ViewBag.AnswerSetID = id;

                //ToDo: make patient detials not assume always depression patient format

                //get the patient for the questionnaire. At present it will always be from depression
                ViewData["Patient"] = _studiesService.DepressionService.GetPatient(_studiesService.DepressionService.QuestionnairePatientIDGet(answerSet.ParticipantID));
                ViewBag.QuestionnaireTitle = answerSet.QuestionnaireTitle;

                return View("Start", pageItems);
            }
            else
            {                
                //_questionnaireService.AnswerSetClose(answerSet.AnswerSetID, "Auto Closed - no more questions"); 
                return Completed(answerSet);
            }
        }  

        /// <summary>
        /// saves the answers to a questionnaire page and continues
        /// </summary>
        /// <param name="formCollection"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = "Administrator,Advisor")]
        [HttpPost]
        public ActionResult Resume(FormCollection formCollection, int id)
        {
            //Response.Cache.SetCacheability(HttpCacheability.NoCache);

            IPageable<QuestionSetPageItem> pageItems;
            try
            {
                pageItems = _questionnaireService.AnswerSetGetActiveQuestion(id);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                return Error("Error Saveing Session", "An error occured reading the session data. The error has been logged.");
            }

            var form = formCollection.AllKeys.ToDictionary(k => k, v => formCollection[v]);
            IList<Answer> answers = null;
            try
            {
                answers = QuestionReader.Read(pageItems.Page, form, User.Identity.Name);
            }
            catch (KeyNotFoundException ex)
            {
                string script = "<html><script>alert('No answer was provided for the current question. You must answer questions in the correct order.'); ";
                script = script + "window.location = '" + Url.Action("Resume", new { id = id }) + "';</script></html>";
                return Content(script);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                return Error("Error Saving Session", "An error occured reading the provided answers. The error has been logged.");
            }

            pageItems = _questionnaireService.AnswerAddByPage(id, answers);

            //return a redirect request instead of the page results. This will prevent the browser storing the page post in the history and stopping the user performing duplicate posts using the browser back button
            return RedirectToAction("Resume", new { id = id });            
        }

        /// <summary>
        /// move the questionnarie back a step
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = "Administrator,Advisor")]
        public ActionResult Back(int id)
        {
            _questionnaireService.SetPreviousQuestion(id);
            return RedirectToAction("Resume", new { id = id });
        }
        /// <summary>
        /// Closes a questionnaire before it is finished
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = "Administrator,Advisor")]
        public ActionResult Close(int id)
        {            
            AnswerSet answerSet = _questionnaireService.AnswerSetGet(id);
            _questionnaireService.AnswerSetClose(answerSet.AnswerSetID, "User Closed");            
            return Completed( answerSet);
        }

        /// <summary>
        /// Closes a questionnaire before it is finished
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = "Administrator,Advisor")]
        public ActionResult Suspend(int id)
        {
            AnswerSet answerSet = _questionnaireService.AnswerSetGet(id);
            //_questionnaireService.AnswerSetClose(answerSet.AnswerSetID, "User Closed");
            return Completed(answerSet);
        }

        /// <summary>
        /// downloads the result of a questionnaire
        /// </summary>
        /// <param name="id"></param>
        /// <param name="studyID"></param>
        /// <returns></returns>
        [Authorize(Roles = "Administrator,Advisor")]
        public ActionResult Download(string id, string studyID)
        {
            int aID = 0;
            if ((studyID == null && int.TryParse(id, out aID))
                || (studyID != null && studyID == _studiesService.DepressionStudyID))
            {
                int.TryParse(id, out aID);               
                var results = _studiesService.DepressionService.QuestionnaireResultGet(id);
                return Download(results);
            }
            else
            {
                var results = _studiesService.CvdService.QuestionnaireResultGet(id);
                return Download(results);
            }
        }

        private ActionResult Error(string errorTitle , string errorMessage)
        {
            @ViewBag.ErrorTitle = errorTitle;
            @ViewBag.ErrorMessage = errorMessage;
            return View("Error");
        }

        /// <summary>
        /// converts a questionnaire results set into a pdf document and returns it for download
        /// </summary>
        /// <param name="results"></param>
        /// <returns></returns>
        private ActionResult Download(QuestionnaireResults results)
        {
            //create the filename
            string invalid = new string(Path.GetInvalidFileNameChars());
            string fileName = string.Format("{0}_{1}.pdf", results.QuestionnaireTitle, results.StartDate.ToShortDateString());
            foreach (char c in invalid)
            {
                fileName = fileName.Replace(c.ToString(), "");
            }

            MemoryStream document = new MemoryStream();
            //generate the pdf
            _documentService.GenerateQuestionnairePDF(results, document, false, Server.MapPath(Url.Content("~/content/themes/healthlines/images/letterlogo.png")));
            //reset the memory stream position
            document.Position = 0;
            var cd = new System.Net.Mime.ContentDisposition
            {
                FileName = fileName,
                // always prompt the user for downloading, set to true if you want 
                // the browser to try to show the file inline
                Inline = false,
            };
            Response.AppendHeader("Content-Disposition", cd.ToString());

            byte[] buffer = new byte[document.Length];
            document.Read(buffer, 0, (int)document.Length);
            return File(buffer, "application/octet-stream");
        }


        private ActionResult Completed(AnswerSet answerset)
        {
            Guid callbackID = Guid.Parse(RouteData.Values["CallbackId"].ToString());
            var callback = _callbackService.Callbacks_Get(callbackID);
            //Assumed we are depression for the moment
            var actions = _studiesService.DepressionService.QuestionnaireActionsGet(answerset.AnswerSetID.ToString());

            CloseQuestionnaireWrapper m = new CloseQuestionnaireWrapper()
            {
                AnswerSet = answerset,
                Callback = callback,
                QuestionnaireAction = actions
            };

            //get the patient for the questionnaire. At present it will always be from depression
            ViewData["Patient"] = _studiesService.DepressionService.GetPatient(_studiesService.DepressionService.QuestionnairePatientIDGet(answerset.ParticipantID));
            return View("Complete", m);
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
