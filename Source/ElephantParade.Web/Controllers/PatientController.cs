using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Web.Profile;
using NHSD.ElephantParade.Core.Interfaces;
using NHSD.ElephantParade.Core;
using NHSD.ElephantParade.Core.Models;
using NHSD.ElephantParade.Web.Helpers;
using NHSD.ElephantParade.Web.Authentication;
using NHSD.ElephantParade.Domain.Models;


namespace NHSD.ElephantParade.Web.Controllers
{
    public class PatientController : BaseController
    {
        private IStudiesService _studiesService;
        private IFormsAuthentication _formsAuthentication;

        public PatientController(IStudiesService studiesService,IFormsAuthentication formsAuthentication)       
        {
            _studiesService = studiesService;
            _formsAuthentication = formsAuthentication;
        }

        [Authorize(Roles = "CVD,Depression,Advisor")]
        public ActionResult Index()
        {
            var HlIdentity = User.Identity as NHSD.ElephantParade.Web.Authentication.HealthLinesParticipantIdentity;
            string studyID = "",patienID= "" ;           

            if(User.IsInRole("Advisor"))
            {
                studyID = RouteData.Values["StudyID"].ToString();
                patienID=RouteData.Values["PatientID"].ToString();
            }
            else if (HlIdentity.PatientId != null && HlIdentity.StudyID != null)
            {
                studyID = HlIdentity.StudyID;
                patienID=HlIdentity.PatientId;                
            }
            StudyPatient pvm = _studiesService.GetPatient(patienID, studyID);
            
            return View(pvm);

            //MembershipUser user =  Membership.GetUser();           
            //ViewBag.Message = "Welcome " + ((HealthLinesParticipantIdentity)User.Identity).DisplayName;  
            //return View();
        }

        

        public ActionResult WelcomePack()
        {
            return View();
        }

        [Authorize(Roles = "CVD,Depression")]
        public ActionResult Details()
        {
            var HlIdentity = User.Identity as NHSD.ElephantParade.Web.Authentication.HealthLinesParticipantIdentity;

            if (HlIdentity.PatientId != null && HlIdentity.StudyID != null)
            {
                var pvm = _studiesService.GetPatient(HlIdentity.PatientId, HlIdentity.StudyID);
                return View(pvm);
            }
            else
            {
                TempData["Message"] = "patientId and studyId is null";
                return RedirectToAction("Error", "Error");
            }
        }

        // Updates the patient details but only those 
        [Authorize(Roles = "CVD,Depression,Advisor")]
        [HttpPost]
        public ActionResult UpdatePatientDetails(ElephantParade.Domain.Models.StudyPatient pVM)
        {
            if (ViewData.ModelState.IsValid)
            {
                //Get the PatientObject
                StudyPatient pObj = _studiesService.GetPatient(pVM.PatientId, pVM.StudyID); 

                //check authorised to update the specified patient details
                if (User.Identity is HealthLinesParticipantIdentity && ((HealthLinesParticipantIdentity)User.Identity).PatientId != null )
                {
                    if (((HealthLinesParticipantIdentity)User.Identity).PatientId != pObj.PatientId || ((HealthLinesParticipantIdentity)User.Identity).StudyID != pObj.StudyID)                    
                        throw new UnauthorizedAccessException("Not authorised to update this patients details.");                    
                }
                 
                //update only the values we want 
                //*******************************
                //Get the Values from the form 
                pObj.Title = pVM.Title;
                pObj.Forename = pVM.Forename;
                pObj.Surname = pVM.Surname;
                //pObj.Ethnicity = pVM.Ethnicity; //patient not allowed to update this field
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
                //pObj.Email = pVM.Email;//patient not currently allowed to update this field

                //Update to DB
                _studiesService.UpdatePatient(pObj, LoginHelper.GetLoggedInUser());

                // Get the updated data and populate it in panel
                pObj = _studiesService.GetPatient(pVM.PatientId, pVM.StudyID);

                //update the IIdenty if the updated details are for the current user
                if ( User.Identity is HealthLinesParticipantIdentity)
                {
                    var ident = User.Identity as HealthLinesParticipantIdentity;
                    if(ident.PatientId!=null && ident.PatientId == pObj.PatientId)
                    {                        
                        //apply the ticket                                             
                        _formsAuthentication.SetAuthCookie(this.HttpContext, UserAuthenticationTicketBuilder.CreateAuthenticationTicket(User.Identity.Name, pObj,false));
                    }
                }
                
                ViewBag.SubmitMessage = "Details updated.";
                return PartialView("_Demographics", pObj);
            }
            else
            {
                //return new HttpStatusCodeResult(500, errorMessage);
                ViewBag.SubmitMessage = "Update Failed. Changes to details were not saved.";
                return PartialView("_Demographics", pVM);
            }
        }

        [Authorize(Roles = "CVD,Depression, Advisor")]
        public ActionResult Reports()
        {
            if (User.IsInRole("Advisor"))
            {
                string studyId = RouteData.Values["studyid"].ToString();
                string patientId = RouteData.Values["patientID"].ToString();
                var files = _studiesService.FileList(studyId, patientId);
                return View(files);
            }
            else
            {               
                HealthLinesParticipantIdentity identity = User.Identity as HealthLinesParticipantIdentity;
                var files = _studiesService.FileList(identity.StudyID, identity.PatientId);
                return View(files);
            }                    
        }

        [Authorize(Roles = "CVD,Depression,Advisor")]
        public ActionResult Download(int fileID)
        {
            IList<PatientFile> files =null;
            if(!User.IsInRole("Advisor"))
            {
                //get the file ensuring the file belongs to the user
                //todo: support advisor view
                HealthLinesParticipantIdentity identity = User.Identity as HealthLinesParticipantIdentity;
                files = _studiesService.FileList(identity.StudyID, identity.PatientId);
            }
            else
                files = _studiesService.FileList(RouteData.Values["studyId"].ToString(), RouteData.Values["patientId"].ToString());
            if ((from f in files where f.FileID == fileID select f).Count() == 1)
            {
                var file = _studiesService.FileGetData(fileID);
                return File(file.Data, "application/octet-stream", file.Filename);
            }
            return RedirectToAction("Reports");
        }

        [Authorize(Roles = "CVD,Depression,Advisor")]
        public ActionResult DownloadWelcomePack(string fileName)
        {
            return File(fileName, "application/octet-stream", "HealthlinesWelcomePack.pdf");
        }
        
    }
}
