using System.Collections.Generic;
using System.Web.Mvc;
using NHSD.ElephantParade.Core.Interfaces;
using NHSD.ElephantParade.Core.Models;

namespace NHSD.ElephantParade.Web.Areas.Advisor.Controllers
{
    public class SearchController : Controller
    {
        //
        // GET: /Advisor/Search/
        private IStudiesService _studiesService;
        private ICallbackService _callbackService;

        public SearchController(IStudiesService studiesService, ICallbackService callbackService)
        {
            _studiesService = studiesService;
            _callbackService = callbackService;
        }

        [Authorize(Roles = "Administrator,Advisor")]
        [HttpGet]
        public ActionResult PatientSearch()
        {
            return View();
        }

        [Authorize(Roles = "Administrator,Advisor")]
        public ActionResult SubmitSearch(PatientSearchViewModel patientSearchViewModel)
        {
            IList<Domain.Models.StudyPatient> patientList = _studiesService.PatientSearch(patientSearchViewModel);
            return PartialView("_PatientSearchResults", patientList);
        }
        [Authorize(Roles = "Administrator,Advisor")]
        public ActionResult ViewPatientDetails(string patientId, string studyId)
        {
            if (string.IsNullOrEmpty(patientId) || string.IsNullOrEmpty(studyId))
                return View("PatientSearch");

            Domain.Models.StudyPatient patient = new Domain.Models.StudyPatient
                {
                    PatientId = patientId,
                    StudyID = studyId
                };

            var singlePatientScheduledCallbacks = _callbackService.Callbacks_List(patientId, studyId, 1);

            ViewBag.ScheduledCallbacksForSinglePatient = singlePatientScheduledCallbacks.TotalRecordsCount;

            return View("PatientDetailsAndCallbacks", patient);
        }
    }
}
