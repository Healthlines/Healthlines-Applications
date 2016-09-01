using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NHSD.ElephantParade.Core.Interfaces;
using NHSD.ElephantParade.DocumentGenerator.Interfaces;
using System.IO;
using System.Web.Security;
using NHSD.ElephantParade.DocumentGenerator.Letters;
using NHSD.ElephantParade.Domain.Models;
using NHSD.ElephantParade.Web.Attributes;
using NHSD.ElephantParade.Web.Areas.Advisor.Models;
using NHSD.ElephantParade.Web.Areas.Advisor.Helpers;

namespace NHSD.ElephantParade.Web.Areas.Advisor.Controllers
{
    public class CommunicationsController : Controller
    {
        #region Fields
        IDocumentService _documentService;
        IStudiesService _studyService;
        IEmailService _emailService;
        IReadingService _readingService;
        #endregion

        #region Ctor
        public CommunicationsController(IDocumentService docGenerator, IStudiesService studyService, 
            IEmailService emailService, IReadingService readingService)
        {
            _documentService = docGenerator;
            _studyService = studyService;
            _emailService = emailService;
            _readingService = readingService;
        }
        #endregion

        #region Letter Actions
        [Authorize(Roles = "Administrator,Advisor")]
        [HttpGet]
        public ActionResult Letters(string studyID, string patientID, string resultSetID,string returnUrl)
        {
            IQuestionnaireResults service;
            if (studyID == _studyService.DepressionStudyID)
            {
                service = _studyService.DepressionService;
            }
            else if (studyID == _studyService.CVDStudyID)
            {
                service = _studyService.CvdService;
            }
            else
                throw new Exception("Unknown Study");

            var actions = service.QuestionnaireActionsGet(resultSetID);
            ViewData["patientID"] = patientID;     
            ViewData["returnUrl"] = GetReturnURL(returnUrl);

            return View("Letters", actions);
        }

        [Authorize(Roles = "Administrator,Advisor")]
        [HttpGet]
        public ActionResult PatientLetters(string studyID, string patientID, string returnUrl)
        {
            IQuestionnaireResults service;
            if (studyID == _studyService.DepressionStudyID)
            {
                service = _studyService.DepressionService;                
            }
            else if (studyID == _studyService.CVDStudyID)
            {
                service = _studyService.CvdService;
            }
            else
                throw new Exception("Unknown Study");

            var actions = service.QuestionnaireActionsGetByPatient(patientID);
            ViewData["patientID"] = patientID;
            ViewData["Patient"] = _studyService.GetPatient(patientID, studyID);
            ViewData["returnUrl"] = GetReturnURL(returnUrl);

            return View("PatientLetters", actions);
        }

        [Authorize(Roles = "Administrator,Advisor")]
        [HttpGet]
        public ActionResult ReviewQuestionnaireLetter(string letterID, string studyID, string patientID, string resultSetID, string returnUrl)
        {            
            //set the return url
            ViewData["returnUrl"] = GetReturnURL(returnUrl);

            LetterType lType;
            if (Enum.TryParse(letterID, out lType))
            {
                //Get the letter details from the Document generator. If null then no pdf can be generated for the lettertemplate name
                LetterDetails letterDetails = _documentService.GetLetterDetails(lType);
                CommunicationsLetterWrapper letter = new CommunicationsLetterWrapper();
                LetterTarget letterTarget = LetterTarget.Unknown;
                LetterActionData letterActionData = null;
                var patient = _studyService.GetPatient(patientID, studyID);


                // If it is the blood bressure readings that are to be sent then assign the LetterTarget to GP
                if (letterID == LetterType.CVDGpBloodPressureReadings.ToString())                
                    letterTarget = LetterTarget.GP;                    
                
                if (resultSetID != null)
                {                    
                    letterActionData = GetQuestionnaireLetterActionData(letterID, studyID, resultSetID);                    
                    letterTarget = letterActionData == null ? LetterTarget.Unknown : letterActionData.LetterTarget;
                }

                //If null then no pdf can be generated for lettertemplate so we will just send an email
                if (letterDetails != null)
                {
                    letter.fields = FilterFields(letterDetails.UserFields, patientID, studyID);
                    letter.LetterDiscription = letterDetails.Description;
                    letter.LetterName = letterDetails.LetterName;                    
                }
                else
                {                    
                    if (letterActionData != null)
                    {
                        letter.LetterDiscription = "<ul>" + String.Join("",letterActionData.LetterValues.Select(a => "<li>" + a.Key + ": " + a.Value + "</li>").ToArray()) + "</ul>"; 
                    }
                }
                letter.LetterID = letterID;
                letter.PatientId = patientID;
                letter.StudyID = studyID;
                letter.ResultSetID = resultSetID;
                
                switch (letterTarget)
                {                    
                    case LetterTarget.GP:
                        letter.EmailContent = EmailHelper.ParseText(Properties.Resources.GPEmailText, patient);
                        if (patient.GPPractice.EmailAddress == null || patient.GPPractice.EmailAddress.Trim() == "")
                            ModelState.AddModelError("Missing GP Email", "Missing GP Email address. Update patient detials");
                        letter.EmailTo = patient.GPPractice.EmailAddress;
                        break;
                    case LetterTarget.Patient:
                        if(letterDetails==null)
                            letter.EmailContent = EmailHelper.ParseText(Properties.Resources.PatientEmailTemplate, patient);
                        else
                            letter.EmailContent = EmailHelper.ParseText(Properties.Resources.PatientEmailText, patient);
                        if (patient.Email == null || patient.Email.Trim() == "")
                            ModelState.AddModelError("Missing GP Email", "Missing Patient Email address. Update patient detials");
                        letter.EmailTo = patient.Email;
                        break;
                    default:
                        throw new Exception("letter target unknown");                        
                }

                // If the letter type is enum CVDGpBloodPressureReadings then return the view for processBPReading
                if (letterID == LetterType.CVDGpBloodPressureReadings.ToString())
                    return View("ProcessBPReadings", letter);
                else
                    return View("ProcessLetter", letter);
            }
            return View("ProcessLetter");
        }

        private IDictionary<string, LetterUserContent> FilterFields(IDictionary<string, LetterUserContent> iDictionary,string patientID,string studyID)
        {
            foreach (var item in iDictionary)
            {
                if (item.Key == "Target BP")
                {
                    var target1 = _readingService.GetPatientTarget(ReadingTypes.SystolicBP, patientID, studyID);
                    var target2 = _readingService.GetPatientTarget(ReadingTypes.DiastolicBP, patientID, studyID);
                    if (target1 != null && target2 != null)
                        item.Value.DefaultContent = target1.Target + "/" + target2.Target;
                }
                else if (item.Key == "Average BP")
                {

                    var readingExpectedFrequency = _readingService.GetReadingExpectedFrequency(patientID, studyID, ReadingTypes.BloodPressure);
                    var averageSystolicBpReading = _readingService.GetAverageReadingForTheLastSixDaysOrWeeks(readingExpectedFrequency, ReadingTypes.SystolicBP, patientID, studyID);
                    var averageDiastolicBpReading = _readingService.GetAverageReadingForTheLastSixDaysOrWeeks(readingExpectedFrequency, ReadingTypes.DiastolicBP, patientID, studyID);
                    if (averageSystolicBpReading != null && averageDiastolicBpReading != null)
                        item.Value.DefaultContent = averageSystolicBpReading + "/" + averageDiastolicBpReading;
                }
                else if (item.Key == "BP History")
                {
                    var bpReadingsForPatient = _readingService.GetBPReadingsForPatient(patientID, studyID);
                    int recordCount = bpReadingsForPatient.Count ;
                    int lastRecordPos = recordCount -1;
                    if (recordCount > 0)
                    {
                        for (int i = 0; ((i < 12) && i < recordCount); i++)
                        {
                            if (bpReadingsForPatient[lastRecordPos].BPStatus.ToString() ==
                                BPStatus.AboveTarget.ToString())
                            {
                                item.Value.DefaultContent += bpReadingsForPatient[lastRecordPos].DateOfReading + " - "
                                                             + bpReadingsForPatient[lastRecordPos].BPHighAndLowReading +
                                                             " - " + "Above target \n";
                            }
                            else if (bpReadingsForPatient[lastRecordPos].BPStatus.ToString() ==
                                     BPStatus.WithinTarget.ToString())
                            {
                                item.Value.DefaultContent += bpReadingsForPatient[lastRecordPos].DateOfReading +
                                                             " - "
                                                             +
                                                             bpReadingsForPatient[lastRecordPos].BPHighAndLowReading +
                                                             " - " + "Within target \n";
                            }
                            else if (bpReadingsForPatient[lastRecordPos].BPStatus.ToString() ==
                                     BPStatus.BelowCriticalLimit.ToString())
                            {
                                item.Value.DefaultContent += bpReadingsForPatient[lastRecordPos].DateOfReading +
                                                             " - "
                                                             +
                                                             bpReadingsForPatient[lastRecordPos].
                                                                 BPHighAndLowReading + " - " +
                                                             "Below critical limit \n";
                            }
                            else if (bpReadingsForPatient[lastRecordPos].BPStatus.ToString() ==
                                     BPStatus.AboveCriticalLimit.ToString())
                            {
                                item.Value.DefaultContent +=
                                    bpReadingsForPatient[lastRecordPos].DateOfReading + " - "
                                    + bpReadingsForPatient[lastRecordPos].BPHighAndLowReading + " - " +
                                    "Above critical limit \n";
                            }
                            lastRecordPos--;
                        }
                    }
                }
            }
            return iDictionary;
        }

        [Authorize(Roles = "Administrator,Advisor")]
        [HttpParamAction]
        [HttpPost]
        public ActionResult ProcessLetter(CommunicationsLetterWrapper letter,string returnUrl)            
        {            
            //set the return url
            ViewData["returnUrl"] = GetReturnURL(returnUrl);
            MemoryStream document = new MemoryStream();
            try
            {
                //todo: ensure this is not a repost and the letter has not been sent
                
                var patient = _studyService.GetPatient(letter.PatientId, letter.StudyID);
                if (patient == null)
                    throw new Exception(string.Format("No patient found study {0} patientID {1}", letter.PatientId ?? "", letter.StudyID ?? ""));                

                LetterTarget LetterTarget;
                
                // If it is the blood bressure readings that are to be sent then assign the LetterTarget to GP
                if (letter.LetterID == LetterType.CVDGpBloodPressureReadings.ToString())
                {
                    LetterTarget = LetterTarget.GP;
                }
                else
                {
                    var actionDetails = GetQuestionnaireLetterActionData(letter.LetterID, letter.StudyID, letter.ResultSetID);
                    LetterTarget = actionDetails != null ? actionDetails.LetterTarget : LetterTarget.Unknown;
                }
                LetterDetails letterDetails = null;
                LetterType lType;
                //get the document details
                if (Enum.TryParse(letter.LetterID, out lType))
                    letterDetails = _documentService.GetLetterDetails(lType);

                //If letterDetails null then no pdf can be generated for lettertemplate so we will just send an email
                PatientFileData datafile = null;
                if (letterDetails != null || letter.LetterID == LetterType.CVDGpBloodPressureReadings.ToString())
                {
                    LetterTarget = CreateLetter(letter, document);

                    //prepare to save letter
                    datafile = new PatientFileData()
                    {
                        Data = document.ToArray(),
                        Date = DateTime.Now,
                        Extension = ".pdf",
                        Filename = GetFilename(letter.LetterID),
                        Length = document.Length,
                        PatientID = patient.PatientId,
                        StudyID = patient.StudyID
                    };
                }
                string email = null;
                System.Net.Mail.Attachment[] attachmnets =null;
                
                switch (LetterTarget)
	            {
		            case LetterTarget.Unknown:
                        throw new Exception("Unknown Letter Target");                     
                    case LetterTarget.GP:
                        email = patient.GPPractice.EmailAddress;                        
                        break;
                    case LetterTarget.Patient:
                        email = patient.Email;              
                        break;
                    default:
                        break;
	            }
                if (email == null || email.Trim() == "")
                {
                    ModelState.AddModelError("Missing GP Email", "Missing GP Email address. Update patient detials");
                    return View("ProcessLetter", letter);
                }

                //create attachments
                if (datafile!=null)                
                    attachmnets = new System.Net.Mail.Attachment[] { new System.Net.Mail.Attachment(document, GetFilename(letter.LetterID)) };
                                
                    _emailService.SendEmail(new string[] { email },
                                        "Heathlines Patient",
                                    letter.EmailContent,
                                        true,                                        
                                    attachmnets,
                                    true  
                                    );
  
                //Get the correct service 
                IQuestionnaireResults service = null;
                if (letter.StudyID == _studyService.DepressionStudyID)                
                    service  = _studyService.DepressionService;
                else if (letter.StudyID == _studyService.CVDStudyID)                
                    service = _studyService.CvdService;

                // if this is a set of blood pressure readings then just return success window
                if (letter.LetterID == LetterType.CVDGpBloodPressureReadings.ToString())
                {
                    //save a copy of the letter to the patients files
                    _studyService.FileSave(datafile);

                    return View("Successful", letter);
                }
                else if (datafile != null && service != null)
                {
                    //save a copy of the letter to the patients files
                    PatientFile file = _studyService.FileSave(datafile);

                    //mark the letter as sent for the encounter?
                    service.QuestionnaireActionLetterProcessed(letter.StudyID,letter.ResultSetID,letter.LetterID,"GP",User.Identity.Name,file.FileID);                
                    _studyService.PatientEventAdd(new PatientEvent()
                    {
                        PatientId = letter.PatientId,
                        StudyID = letter.StudyID,
                        EventCode = PatientEvent.EventTypes.GPEmailed,
                        User = User.Identity.Name,
                        Value1 = letter.EmailContent,
                        Value2 = letter.LetterID,
                        Details = ""
                    });
                }
                else
                {
                    _studyService.PatientEventAdd(new PatientEvent()
                    {
                        PatientId = letter.PatientId,
                        StudyID = letter.StudyID,
                        EventCode = PatientEvent.EventTypes.PatientEmailed,
                        User = User.Identity.Name,
                        Value1 = letter.EmailContent,
                        Value2 = "",
                        Details = ""
                    });
                    //mark the letter as sent for the encounter
                    service.QuestionnaireActionLetterProcessed(letter.StudyID, letter.ResultSetID, letter.LetterID, "GP", User.Identity.Name, null);
                }
                
                return View("Successful", letter);                
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Invalid Letter", ex.Message);                
            }            

            return View("ProcessLetter", letter);
        }
        [Authorize(Roles = "Administrator,Advisor")]
        [HttpParamAction]
        [HttpPost]
        public ActionResult ViewLetter(CommunicationsLetterWrapper letter)
        {
            //as letters a small in size we can use a memnory stream, 
            //if big we should really use the response stream to be written to directly
            MemoryStream document = new MemoryStream();
            try 
	        {
                CreateLetter(letter, document);
                //reset the memory stream position
                document.Position = 0;
                if (document.Length > 0)
                    return Download(GetFilename(letter.LetterID), document);
	        }
	        catch (Exception ex)
	        {
		        ModelState.AddModelError("Invalid Letter", ex.Message);		       
	        }
            return View("ProcessLetter", letter);
        }

        [Authorize(Roles = "Administrator,Advisor")]
        public ActionResult DownloadFile(int fileID)
        {
            var filedata = _studyService.FileGetData(fileID);
            var cd = new System.Net.Mime.ContentDisposition
            {
                FileName = filedata.Filename,
                // always prompt the user for downloading, set to true if you want 
                // the browser to try to show the file inline
                Inline = false,
            };
            Response.AppendHeader("Content-Disposition", cd.ToString());
            return File(filedata.Data, "application/octet-stream");
        }

        #region Private Letter Methods

        private string GetReturnURL(string returnUrl)
        {
            string url;
            if (Url.IsLocalUrl(returnUrl) && returnUrl.Length > 1 && returnUrl.StartsWith("/")
                        && !returnUrl.StartsWith("//") && !returnUrl.StartsWith("/\\"))
            {
                url = returnUrl;
            }
            else
            {
                if (User.IsInRole("Advisor"))
                    url = Url.Action("Index", "Home", new { Area = "Advisor" });
                else if (User.IsInRole("CVD") || User.IsInRole("Depression"))
                    url = Url.Action("Index", "Patient");
                url = returnUrl;
            }
            return url;
        }

        private LetterTarget CreateLetter(CommunicationsLetterWrapper letter, Stream letterStream)
        {
            //get the lettertype
            LetterType lType;
            if (Enum.TryParse(letter.LetterID, out lType))
            {
                //get all the letter action values for results set
                QuestionnaireActions actions = null;
                LetterActionData letterAction = null;

                //if its not CVDGpBloodPressureReadings process the questionnaire action data and the user provided data
                if (letter.LetterID != LetterType.CVDGpBloodPressureReadings.ToString())
                {
                    //get any actions and questionnaire data needed to be performed
                    if (letter.StudyID == _studyService.DepressionStudyID)
                    {
                        actions = _studyService.DepressionService.QuestionnaireActionsGet(letter.ResultSetID);
                    }
                    else if (letter.StudyID == _studyService.CVDStudyID)
                    {
                        actions = _studyService.CvdService.QuestionnaireActionsGet(letter.ResultSetID);
                    }
                    var lname = (LetterType)Enum.Parse(typeof(LetterType), letter.LetterID);
                    //find the action detials for the letter we are processing
                    letterAction = (from la in actions.LetterActionData
                                    where la.LetterActionData.LetterTemplate == lname
                                        select la.LetterActionData).FirstOrDefault();

                    //prepare the letter values required to be filled by the user
                    letter.fields = FilterFields(_documentService.GetLetterDetails(lType).UserFields,letter.PatientId,letter.StudyID);

                    //cast values posted from form to the required types for the letter we are generating               
                    foreach (var item in letter.fields)
                        if (letter.values.ContainsKey(item.Key) && letter.values[item.Key].Trim().Length > 0)
                        {
                            if (item.Value.Type == typeof(string[]))
                            {
                                string[] values = letter.values[item.Key].Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
                                letterAction.LetterValues.Add(item.Key, values);
                            }
                            else
                                letterAction.LetterValues.Add(item.Key, Convert.ChangeType(letter.values[item.Key], item.Value.Type));
                        }
                        else //a requested user filed was not supplied
                        { 
                            //if the letter is asking for bp readings try and get them
                            if(item.Value.Type == typeof(IList<BloodPressureReadingViewModel>))
                            {
                                var readingsInDateRange = _readingService.GetBPReadingsForPatientWithinDateRangeGiven(
                                                letter.PatientId, letter.StudyID, DateTime.Now.Date.AddMonths(-6),
                                                DateTime.Now.Date);

                                letterAction.LetterValues.Add(item.Key, readingsInDateRange);
                            }
                            //else if (item.Key == "Target BP")
                            //{
                            //    var target1 = _readingService.GetPatientTarget(ReadingTypes.SystolicBP,letter.PatientId, letter.StudyID);
                            //    var target2 = _readingService.GetPatientTarget(ReadingTypes.DiastolicBP, letter.PatientId, letter.StudyID);
                            //    if(target1!=null && target2!=null)
                            //        letterAction.LetterValues.Add(item.Key, target1.Target + "/" + target2.Target);
                            //}
                        }
                }

                if (letter.LetterID != Domain.Models.LetterType.CVDGpBloodPressureReadings.ToString())
                {                    
                    //generate the letter
                    _documentService.GenerateLetter(letterAction, User.Identity.Name, null, _studyService.GetPatient(letter.PatientId, letter.StudyID), letterStream, false, Server.MapPath(Url.Content("~/content/themes/healthlines/images/letterlogo.png")));
                }
                else
                {
                    IList<BloodPressureReadingViewModel> readingsInDateRange = null;
                    if (Request.QueryString["BPStartDateToSendToGP"] != null && Request.QueryString["BPEndDateToSendToGP"] != null)
                    {
                        var tmpBpStartDateToSendToGp =
                             Server.UrlDecode(Request.QueryString["BPStartDateToSendToGP"].Substring(0, 10));
                        var tmpBpEndDateToSendToGp =
                             Server.UrlDecode(Request.QueryString["BPEndDateToSendToGP"].Substring(0, 10));

                        DateTime bpStartDateToSendToGp = DateTime.ParseExact(tmpBpStartDateToSendToGp, "dd/MM/yyyy", null);
                        DateTime bpEndDateToSendToGp = DateTime.ParseExact(tmpBpEndDateToSendToGp, "dd/MM/yyyy", null);
                    
                         readingsInDateRange = _readingService.GetBPReadingsForPatientWithinDateRangeGiven(
                                                    letter.PatientId, letter.StudyID, bpStartDateToSendToGp,
                                                    bpEndDateToSendToGp);
                    }
                    else
                    {
                         readingsInDateRange = _readingService.GetBPReadingsForPatientWithinDateRangeGiven(
                                                letter.PatientId, letter.StudyID, DateTime.Now.Date.AddMonths(-6),
                                                DateTime.Now.Date);
                    }
                    _documentService.GenerateBloodPressurePDF(_studyService.GetPatient(letter.PatientId, letter.StudyID),
                                                              readingsInDateRange,
                                                              User.Identity.Name,
                                                              null,
                                                              letterStream,
                                                              false,
                                                              Server.MapPath(Url.Content("~/content/themes/healthlines/images/letterlogo.png")));
                    return LetterTarget.GP;
                }

                return letterAction.LetterTarget;
            }
            else
                throw new Exception("Invalid Letter");
        }

        private LetterActionData GetQuestionnaireLetterActionData(string LetterID, string StudyID, string ResultSetID)
        {
            LetterType lType;
            if (Enum.TryParse(LetterID, out lType))
            {
                //get all the letter action values for results set
                QuestionnaireActions actions = null;
                if (StudyID == _studyService.DepressionStudyID)
                {
                    actions = _studyService.DepressionService.QuestionnaireActionsGet(ResultSetID);
                }
                else if (StudyID == _studyService.CVDStudyID)
                {
                    actions = _studyService.CvdService.QuestionnaireActionsGet(ResultSetID);
                }
                //find the action detials for the letter we are processing
                var lname = (LetterType)Enum.Parse(typeof(LetterType),LetterID);
                var letterAction = (from la in actions.LetterActionData
                                    where la.LetterActionData.LetterTemplate == lname
                                    select la.LetterActionData).FirstOrDefault();

                return letterAction;
            }
            return null;
        }

        /// <summary>
        /// converts a questionnaire results set into a pdf document and returns it for download
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="pdfdocument"></param>
        /// <returns></returns>
        private ActionResult Download(string filename,Stream pdfdocument)
        {            
            var cd = new System.Net.Mime.ContentDisposition
            {
                FileName = filename,
                // always prompt the user for downloading, set to true if you want 
                // the browser to try to show the file inline
                Inline = false,
            };
            Response.AppendHeader("Content-Disposition", cd.ToString());
          
            return File(pdfdocument, "application/octet-stream");
        }

        private string GetFilename(string letterName)
        {
            //create the filename
            string fileName = string.Format("{0}_{1}.pdf", letterName, DateTime.Now.ToShortDateString());
            string invalid = new string(Path.GetInvalidFileNameChars());
            foreach (char c in invalid)
                fileName = fileName.Replace(c.ToString(), "");
            return fileName;
        }

        #endregion (Private Letter Methods)

        #endregion

        #region BP Actions

        [Authorize(Roles = "Administrator,Advisor")]
        [HttpGet]
        public ActionResult ReviewBPReadings(string studyID, string patientID, string resultSetID)
        {
            var patient = _studyService.GetPatient(patientID, studyID);

            CommunicationsLetterWrapper letter = new CommunicationsLetterWrapper()
            {
                PatientId = patientID,
                StudyID = studyID
            };

            letter.EmailContent = EmailHelper.ParseText(Properties.Resources.GPEmailText, patient);
            if (patient.GPPractice.EmailAddress == null || patient.GPPractice.EmailAddress.Trim() == "")
                ModelState.AddModelError("Missing GP Email", "Missing GP Email address. Update patient detials");
            ViewBag.EmailAddress = patient.GPPractice.EmailAddress;

            return View("ProcessBPReadings");
        }


        /// <summary>
        /// Downloads the patient blood pressures
        /// </summary>
        /// <param name="id"></param>
        /// <param name="studyID"></param>
        /// <returns></returns>
        [Authorize(Roles = "Administrator,Advisor")]
        public ActionResult BP_Download(string id, string studyID)
        {
            var bpReadings = _readingService.GetBPReadingsForPatient(id, studyID);
            return BP_Download(bpReadings);
        }

        /// <summary>
        /// converts a questionnaire results set into a pdf document and returns it for download
        /// </summary>
        /// <param name="results"></param>
        /// <returns></returns>
        private ActionResult BP_Download(IList<BloodPressureReadingViewModel> bpReadings)
        {
            return View();
        }


        //private LetterDetails GetLetterDetails(string studyID, string patientID, LetterType lType)
        //{
        //    var letterDetails = _documentService.GetLetterDetails(lType);

        //    if ( letterDetails == null)
        //        letterDetails = new LetterDetails();


        //    var BPs = letterDetails.UserFields.Where(i => i.Value.Type == typeof(IList<NHSD.ElephantParade.Domain.Models.BloodPressureReadingViewModel>)).Select(i => i);

        //    foreach (var item in BPs)
        //    {
        //        //get the bp values
        //    }

        //    ////if the userfields are asking for BP readings the add them
        //    //if (letterDetails.UserFields.Contains(
        //    //        new KeyValuePair<string,LetterUserContent>("",new LetterUserContent()
        //    //            {
        //    //                Type = typeof(IList<NHSD.ElephantParade.Domain.Models.BloodPressureReadingViewModel>)
        //    //            }),new MyTypeComparer()
        //    //        )
        //    //    )
        //    //{
                
        //    //}

        //    return letterDetails;
        //}

        //private class MyTypeComparer
        //    :IEqualityComparer<KeyValuePair<string,LetterUserContent>>
        //{

        //    public bool Equals(KeyValuePair<string, LetterUserContent> x, KeyValuePair<string, LetterUserContent> y)
        //    {
        //        if (x.Value.Type == y.Value.Type)
        //            return true;
        //        else
        //            return false;
        //    }

        //    public int GetHashCode(KeyValuePair<string, LetterUserContent> obj)
        //    {
        //        return obj.ToString().ToLower().GetHashCode();
        //    }
        //}
        #endregion
    }
}
