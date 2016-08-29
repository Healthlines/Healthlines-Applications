// -----------------------------------------------------------------------
// <copyright file="StudyService.cs" company="NHS Direct">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

using System.ComponentModel.DataAnnotations;
using System.Globalization;
using NHSD.ElephantParade.Core.Models;

namespace NHSD.ElephantParade.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Interfaces;
    using Domain.Models;
    using DAL.Interfaces;
    using DAL.EntityModels;
    using DAL;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class StudyService : IStudiesService
    {
        #region fields
        List<IPatientService> _patientServices;
        IDepressionService _depressionService;
        ICvdService _cvdService;
        IFileRepository _patientFileRepository;
        IEventRepository _patientEventRepository;
        #endregion

        #region Ctor

        public StudyService(IDepressionService depressionService, ICvdService cvdService, IFileRepository patientFileRepository, IEventRepository patientEventRepository)
        {    
            _depressionService = depressionService;
            _cvdService = cvdService;
            if (_patientServices == null)
                _patientServices = new List<IPatientService> { _depressionService};
            else{
                _patientServices.Add(_depressionService);
           
            }
            _patientFileRepository = patientFileRepository??new FileRepository();
            _patientEventRepository = patientEventRepository ?? new EventRepository();

            //register event
            _patientServices.ForEach(s => s.PatientDetailsChanged += PatientEventHandler);
        }
        public StudyService(IEnumerable<IPatientService> patientServices, IDepressionService depressionPatients, ICvdService cvdPatients, IFileRepository patientFileRepository, IEventRepository patientEventRepository)
            : this(depressionPatients, cvdPatients, patientFileRepository, patientEventRepository)
        {
            var collection = patientServices as IPatientService[] ?? patientServices.ToArray();
            _patientServices.AddRange(collection);

            //register events
            foreach (var item in collection)
            {
                item.PatientDetailsChanged += PatientEventHandler;
            }
        }

        #endregion

        #region Properties
        public string DepressionStudyID
        {
            get { return _depressionService.StudyID; }
        }

        public string CVDStudyID
        {
            get { return "UNUSEDED_STUDY"; }
        }

        public IDepressionService DepressionService
        { get { return _depressionService; } }

        public ICvdService CvdService
        { get { return _cvdService; } }
        #endregion

        #region IStudiesService

        #region Patient

        public Domain.Models.StudyPatient GetPatient(string patientId, string studyID)
        {
            IPatientService service = GetPatientService(studyID);            
            return service.GetPatient(patientId);
        }

        public StudyPatient AddPatient(StudyPatient patient, string userId)
        {

            //validate the patient Object
            var context = new ValidationContext(patient, serviceProvider: null, items: null);
            var results = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(patient, context, results);
            if (!isValid)
            {
                StringBuilder errors = new StringBuilder();
                foreach (var validationResult in results)
                {
                    errors.AppendLine(validationResult.ErrorMessage);
                }
                throw new ArgumentException(errors.ToString());
            }
            IPatientService service = GetPatientService(patient.StudyID);
            var newPatient =  service.AddPatient(patient, userId);
            PatientEventAdd(new Domain.Models.PatientEvent
            {
                PatientId = newPatient.PatientId,
                StudyID = newPatient.StudyID,
                EventCode = Domain.Models.PatientEvent.EventTypes.PatientAdded,
                User = userId,
                Details = ""
            });
            return newPatient;
        }
        public void DeletePatient(StudyPatient patient,string userId)
        {
            IPatientService service = GetPatientService(patient.StudyID);

            service.DeletePatient(patient.PatientId, userId);
            
        }
        
        public StudyPatient GetPatientByEmail(string email)
        {
            StudyPatient patient =null;
            foreach (var item in _patientServices)
            {
                patient = item.GetPatientByEmail(email);
                if (patient != null)
                    break;
            }
            return patient;
        }
        
        public IList<string> GetPatientsInactive()
        {
            List<string> inactivePatients = new List<string>();
            foreach (IPatientService patientService in _patientServices)
            {
                inactivePatients.AddRange(patientService.GetPatientsInactive());
            }
            return inactivePatients;
        }

        public IList<StudyPatient> PatientSearch(PatientSearchViewModel patientSearchViewModel)
        {
            List<StudyPatient> studyPatients = new List<StudyPatient>();
            foreach (IPatientService patientService in _patientServices)
            {
                studyPatients.AddRange(patientService.PatientSearch(patientSearchViewModel));
            }
            return studyPatients;
        }

        public bool DoesPatientExistInTheDatabase(StudyPatient studyPatient)
        {
            IPatientService service = GetPatientService(studyPatient.StudyID);
            return service.DoesPatientExistInTheDatabase(studyPatient);
        }

        public void UpdatePatient(StudyPatient patientViewModel, string userid)
        {
            IPatientService service = GetPatientService(patientViewModel.StudyID);
            service.UpdatePatient(patientViewModel,userid);
        }
        #endregion

        #region Medical
        public PatientMedicalConditions MedicalConditions_ListByPatient(string studyId, string patientId)
        {
            IPatientService service = GetPatientService(studyId);
            return service.MedicalConditions_List(patientId);
        }

        public void MedicalConditions_Save(string studyId, string patientId, MedicalConditionItem item)
        {
            IPatientService service = GetPatientService(studyId);
            service.MedicalConditions_Save(patientId, item);

        }

        public void MedicalConditions_Delete(string studyId, int itemId)
        {
            IPatientService service = GetPatientService(studyId);
            service.MedicalConditions_Delete(itemId);            
        }
        #endregion

        #region Files

        public PatientFile FileSave(PatientFileData pfile)
        {
            var convertor = new Mapping.ConvertViewModelObjects();
            File fileData;
            if(pfile.FileID > 0){                
                fileData = _patientFileRepository.GetSingle(f=>f.FileID == pfile.FileID);
                if (fileData == null)
                    throw new ArgumentException(string.Format("File {0} not found", pfile.FileID));
            }
            else
            {
                fileData = new File();
                _patientFileRepository.Add(fileData);
            }

            convertor.ConvertFromFileData(pfile, ref fileData);            
            _patientFileRepository.SaveChanges();
            return convertor.ConvertFromFile(fileData,null);
        }

        public PatientFile FileSave(PatientFile pfile)
        {
            var convertor = new Mapping.ConvertViewModelObjects();
            File fileData;
            if (pfile.FileID > 0)
            {
                fileData = _patientFileRepository.GetSingle(f => f.FileID == pfile.FileID);
                if (fileData == null)
                    throw new ArgumentException(string.Format("File {0} not found", pfile.FileID));
            }
            else
            {
                fileData = new File();
                _patientFileRepository.Add(fileData);
            }
            convertor.ConvertFromFile(pfile, ref fileData);
            _patientFileRepository.SaveChanges();
            return pfile;
        }

        public IList<PatientFile> FileList(string studyId, string patientId)
        {
            var files = _patientFileRepository.ListByPatient(studyId, patientId);
            var convertor = new Mapping.ConvertViewModelObjects();
            return files.Select(f => convertor.ConvertFromFile(f, null)).OrderByDescending(f => f.Date).ToList();
        }

        public PatientFileData FileGetData(int fileId)
        {
            var convertor = new Mapping.ConvertViewModelObjects();
            var file = _patientFileRepository.GetSingle(f => f.FileID == fileId);

            return convertor.ConvertFromFileData(file);
        }

        #endregion


        #region Patient Events

        public void PatientEventAdd(Domain.Models.PatientEvent newEvent)
        {
            if (newEvent.EventID != 0)
                throw new ArgumentException("Invalid PatientEvent");

            if (string.IsNullOrWhiteSpace(newEvent.PatientId) || string.IsNullOrWhiteSpace(newEvent.StudyID))
                throw new ArgumentException("Invalid PatientEvent. Missing PatientId or StudyID");

            var convertor = new Mapping.ConvertViewModelObjects();

            DAL.EntityModels.PatientEvent pEvent = convertor.ConvertFromEvent(newEvent);
            pEvent.EventDate = DateTime.Now;

            if (string.IsNullOrWhiteSpace(pEvent.EventUser))            
                pEvent.EventUser = Context.UserPrincipal.Identity.Name;            
            
            _patientEventRepository.Add(pEvent);
            _patientEventRepository.SaveChanges();
        }

        public List<Domain.Models.PatientEvent> PatientEventList(string studyId, string patientId)
        {
            var events = _patientEventRepository.ListByPatient(studyId, patientId);
            var convertor = new Mapping.ConvertViewModelObjects();
            return events.Select(e => convertor.ConvertFromEvent(e)).ToList();          
        }

        #endregion

        #endregion

        #region private Methods
        /// <summary>
        /// gets a patient service based on id
        /// </summary>
        /// <param name="studyId"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">service not found</exception>
        private IPatientService GetPatientService(string studyId)
        {
            IPatientService service = _patientServices.FirstOrDefault(ps => ps.StudyID == studyId);
            if (service == null)
                throw new ArgumentException(String.Format("Unknown study {0}", studyId));
            return service;
        }

        /// <summary>
        /// Handeles Patient Updated event from the IPatient services
        /// saves any changes to the patient event log
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="orginal"></param>
        /// <param name="updated"></param>
        private void PatientEventHandler(object sender, StudyPatient orginal,StudyPatient updated)
        {
            Domain.Models.PatientEvent e = new Domain.Models.PatientEvent();
            e.PatientId = orginal.PatientId;
            e.StudyID = orginal.StudyID;
            e.Date = DateTime.Now;
            e.User = Context.UserPrincipal.Identity.Name;
            if (updated == null)
            {
                e.EventCode = Domain.Models.PatientEvent.EventTypes.PatientDeleted;
                updated = new StudyPatient{GPPractice = new GeneralPractitioner()};
            }
            else
                e.EventCode = Domain.Models.PatientEvent.EventTypes.PatientDetailsUpdated;
                
            if (orginal.StudyID != updated.StudyID)
                AddNewEvent(e, orginal.StudyID, updated.StudyID, "StudyID");
            if (orginal.StudyTrialNumber != updated.StudyTrialNumber)
                AddNewEvent(e, orginal.StudyTrialNumber, updated.StudyTrialNumber, "StudyTrialNumber");
            if (orginal.StudySite != updated.StudySite)
                AddNewEvent(e, orginal.StudySite, updated.StudySite, "StudySite");
            if (orginal.StudyConsentedDate != updated.StudyConsentedDate)
                AddNewEvent(e, orginal.StudyConsentedDate.ToString(), updated.StudyConsentedDate.ToString(),
                            "StudyConsentedDate");
            if (orginal.StudyReferralDate != updated.StudyReferralDate)
                AddNewEvent(e, orginal.StudyReferralDate.ToString(), updated.StudyReferralDate.ToString(),
                            "StudyReferralDate");
            if (orginal.Status != updated.Status)
                AddNewEvent(e, orginal.Status.ToString(), updated.Status.ToString(), "Status");
            if (orginal.Title != updated.Title)
                AddNewEvent(e, orginal.Title, updated.Title, "Title");
            if (orginal.Forename != updated.Forename)
                AddNewEvent(e, orginal.Forename, updated.Forename, "Forename");
            if (orginal.Surname != updated.Surname)
                AddNewEvent(e, orginal.Surname, updated.Surname, "Surname");
            if (orginal.TelephoneNumber != updated.TelephoneNumber)
                AddNewEvent(e, orginal.TelephoneNumber, updated.TelephoneNumber, "TelephoneNumber");
            if (orginal.TelephoneNumberMobile != updated.TelephoneNumberMobile)
                AddNewEvent(e, orginal.TelephoneNumberMobile, updated.TelephoneNumberMobile, "TelephoneNumberMobile");
            if (orginal.TelephoneNumberOther != updated.TelephoneNumberOther)
                AddNewEvent(e, orginal.TelephoneNumberOther, updated.TelephoneNumberOther, "TelephoneNumberOther");
            if (orginal.PreferredContactNumber != updated.PreferredContactNumber)
                AddNewEvent(e, orginal.PreferredContactNumber.ToString(), updated.PreferredContactNumber.ToString(),
                            "PreferredContactNumber");
            if (orginal.Email != updated.Email)
                AddNewEvent(e, orginal.Email, updated.Email, "Email");
            if (orginal.PrederredContactTime != updated.PrederredContactTime)
                AddNewEvent(e, orginal.PrederredContactTime, updated.PrederredContactTime, "PrederredContactTime");
            if (orginal.RegisteredDate != updated.RegisteredDate)
                AddNewEvent(e, orginal.RegisteredDate.ToString(CultureInfo.InvariantCulture), updated.RegisteredDate.ToString(CultureInfo.InvariantCulture),
                            "RegisteredDate");
            if (!orginal.Address.Equals(updated.Address))
                AddNewEvent(e, orginal.Address.ToString(), updated.Address==null?null:updated.Address.ToString(), "Address");
            if (orginal.DOB != updated.DOB)
                AddNewEvent(e, orginal.DOB.ToString(), updated.DOB.ToString(), "DOB");
            if (orginal.Gender != updated.Gender)
                AddNewEvent(e, orginal.Gender, updated.Gender, "Gender");
            if (orginal.Ethnicity != updated.Ethnicity)
                AddNewEvent(e, orginal.Ethnicity, updated.Ethnicity, "Ethnicity");
            if (orginal.Email != updated.Email)
                AddNewEvent(e, orginal.Email, updated.Email, "Email");
            if ((orginal.GPPractice.Address == null && updated.GPPractice.Address != null) ||
                (orginal.GPPractice.Address != null &&
                    !orginal.GPPractice.Address.Equals(updated.GPPractice.Address)))
                AddNewEvent(e, orginal.GPPractice.Address != null ? orginal.GPPractice.Address.ToString() : "",
                            updated.GPPractice.Address != null ? updated.GPPractice.Address.ToString() : "",
                            "Address");
            if (orginal.GPPractice.EmailAddress != updated.GPPractice.EmailAddress)
                AddNewEvent(e, orginal.GPPractice.EmailAddress, updated.GPPractice.EmailAddress, "EmailAddress");
            if (orginal.GPPractice.Name != updated.GPPractice.Name)
                AddNewEvent(e, orginal.GPPractice.Name, updated.GPPractice.Name, "Name");
            if (orginal.GPPractice.PrimaryCareTrust != updated.GPPractice.PrimaryCareTrust)
                AddNewEvent(e, orginal.GPPractice.PrimaryCareTrust, updated.GPPractice.PrimaryCareTrust,
                            "PrimaryCareTrust");
            if (orginal.GPPractice.TelephoneNumber != updated.GPPractice.TelephoneNumber)
                AddNewEvent(e, orginal.GPPractice.TelephoneNumber, updated.GPPractice.TelephoneNumber,
                            "TelephoneNumber");
            if (orginal.NhsNumber != updated.NhsNumber)
                AddNewEvent(e, orginal.NhsNumber, updated.NhsNumber, "NhsNumber");
            if (orginal.OnAntidepressants != updated.OnAntidepressants)
                AddNewEvent(e, orginal.OnAntidepressants.ToString(), updated.OnAntidepressants.ToString(),
                            "OnAntidepressants");
            if (orginal.BaselineGAD7 != updated.BaselineGAD7)
                AddNewEvent(e, orginal.BaselineGAD7.ToString(CultureInfo.InvariantCulture), updated.BaselineGAD7.ToString(CultureInfo.InvariantCulture), "BaselineGAD7");
            if (orginal.BaselinePHQ9 != updated.BaselinePHQ9)
                AddNewEvent(e, orginal.BaselinePHQ9.ToString(CultureInfo.InvariantCulture), updated.BaselinePHQ9.ToString(CultureInfo.InvariantCulture), "BaselinePHQ9");
            if (orginal.Education != updated.Education)
                AddNewEvent(e, orginal.Education, updated.Education, "Education");
            
        }

        private void AddNewEvent(Domain.Models.PatientEvent e, string origianlValue, string updatedValue,string propertyName)
        {
            e.EventID = 0;
            e.Value1 = propertyName;
            e.Value2 = updatedValue;
            e.Details = string.Format("{0} changed from '{1}' to '{2}'",propertyName,origianlValue,updatedValue);
            PatientEventAdd(e);  
        }
        
        #endregion        
    }
}
