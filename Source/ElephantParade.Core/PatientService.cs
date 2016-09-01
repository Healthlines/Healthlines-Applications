using System;
using System.Collections.Generic;
using System.Linq;
using NHSD.ElephantParade.Core.Interfaces;
using NHSD.ElephantParade.Core.Models;
using NHSD.ElephantParade.Core.Mapping;
using NHSD.ElephantParade.DAL.Interfaces;
using NHSD.ElephantParade.DAL.EntityModels;
using NHSD.ElephantParade.DAL.Repositories;

namespace NHSD.ElephantParade.Core
{
    public class PatientService         
        :IPatientService
    {
        //internal const string STUDY_ID = "Depression";
        private string _studyID = "";
        //Member variables
        private IPatientRepository _patientDepressionRepository;
        private IPatientMedicationRepository _patientMedicationRepository;
        private IGpPracticeAddressRepository _gpPracticeAddressRepository;

        public event PatientUpdatedEventHandler PatientDetailsChanged;

        //Contsructors
        public PatientService()
            : this(new PatientRepository()
            ,  new PatientMedicationRepository()
            ,  new GpPracticeAddressRepository()
            )
        {

        }

        public PatientService(IPatientRepository patientRepository,
            IPatientMedicationRepository patientMedicationRepository,
            IGpPracticeAddressRepository gpPracticeAddressRepository
            )
        {
            _patientDepressionRepository = patientRepository ?? new PatientRepository();
            _patientMedicationRepository = patientMedicationRepository;
            _gpPracticeAddressRepository = gpPracticeAddressRepository;
        }

        #region IPatientService

        public virtual string StudyID
        {
            get 
            {
                if(_studyID == "" )
                    throw new ArgumentNullException("StudyID has not been set for NHSD.ElephantParade.Core.PatientService");
                return _studyID; }
            set
            {
                _studyID = value;
            }
        }

        
        /// <summary>
        /// Add a new patient
        /// </summary>
        /// <param name="patient"></param>
        /// <param name="userId"></param>
        public ElephantParade.Domain.Models.StudyPatient AddPatient(Domain.Models.StudyPatient patient, string userId)
        {
            ConvertViewModelObjects cvmo = new ConvertViewModelObjects();
            GpPracticeAddress gpAddress = new GpPracticeAddress();
            
            Patient p = cvmo.ConvertFromPatientViewModel(patient,ref gpAddress);            
            _patientDepressionRepository.Add(p);
            _patientDepressionRepository.SaveChanges();

            gpAddress.PatientID = Convert.ToString(p.PatientId);
            _gpPracticeAddressRepository.Add(gpAddress);
            _gpPracticeAddressRepository.SaveChanges();

            var patientIdString = Convert.ToString(p.PatientId);
            var gpPracticeAddress = _gpPracticeAddressRepository.GetSingle(pa => pa.PatientID == patientIdString && pa.StudyID==this.StudyID);

            return cvmo.ConvertFromPatient(p, this.StudyID, gpPracticeAddress);
        }
        /// <summary>
        /// Delete a patient
        /// Relies on DB to delete dependencies
        /// </summary>
        /// <param name="patientId"></param>
        /// <param name="userId"></param>
        public void DeletePatient(string patientId, string userId)
        {
            Guid depressionPatientId;
            if (Guid.TryParse(patientId, out depressionPatientId))
            {

                var patient = _patientDepressionRepository.GetSingle(p => p.PatientId == depressionPatientId);
                if (patient != null)
                {
                    ConvertViewModelObjects cvmo = new ConvertViewModelObjects();

                    var patentIdString = Convert.ToString(patient.PatientId);
                    var gpPracticeAddress = _gpPracticeAddressRepository.GetSingle(p => p.PatientID == patentIdString && p.StudyID==this.StudyID);

                    var sp = cvmo.ConvertFromPatient(patient, this.StudyID, gpPracticeAddress);
                    
                    if (PatientDetailsChanged != null)
                        PatientDetailsChanged(this, sp, null);

                    _patientDepressionRepository.Delete(new Patient() {PatientId = depressionPatientId});
                    _gpPracticeAddressRepository.SaveChanges();
                    if (gpPracticeAddress != null)
                    {
                        _gpPracticeAddressRepository.Delete(gpPracticeAddress);
                        _patientDepressionRepository.SaveChanges();
                    }
                    
                }
            }
        }
        /// <summary>
        /// Getting Individual Patient Details
        /// </summary>
        public ElephantParade.Domain.Models.StudyPatient GetPatient(string patientId)
        {           
            Guid depressionPatientID = Guid.Empty;
            Guid.TryParse(patientId, out depressionPatientID);

            var patient = _patientDepressionRepository.GetSingle(p => p.PatientId == depressionPatientID);

            if (patient == null)
                return null;

            ConvertViewModelObjects cvmo = new ConvertViewModelObjects();

            var patentIdString = Convert.ToString(patient.PatientId);
            var gpPracticeAddress = _gpPracticeAddressRepository.GetSingle(p => p.PatientID == patentIdString && p.StudyID==this.StudyID);

            return cvmo.ConvertFromPatient(patient, this.StudyID, gpPracticeAddress);
        }

        public Domain.Models.StudyPatient GetPatientByEmail(string email)
        {
            var patient = _patientDepressionRepository.GetQueryable().Where(p => p.EmailAddress == email).FirstOrDefault();
            if (patient == null)
                return null;
            ConvertViewModelObjects cvmo = new ConvertViewModelObjects();

            var patentIdString = Convert.ToString(patient.PatientId);
            var gpPracticeAddress = _gpPracticeAddressRepository.GetSingle(p => p.PatientID == patentIdString && p.StudyID==this.StudyID);
            
            return cvmo.ConvertFromPatient(patient, this.StudyID, gpPracticeAddress);
        }

        public bool DoesPatientExistInTheDatabase(Domain.Models.StudyPatient spVM)
        {
            if (_patientDepressionRepository.GetQueryable().Any(p => p.EmailAddress == spVM.Email))
                return true;
            else
                return false;
        }

        public IList<string> GetPatientsInactive()
        {
            return _patientDepressionRepository.GetAll
                (p => p.Status != (int)Domain.Models.StudyPatient.PatientStudyStatusType.Active).Select(p => p.PatientId.ToString()).ToList();
        }

        public IList<Domain.Models.StudyPatient> PatientSearch(PatientSearchViewModel patientSearchViewModel)
        {
            if (patientSearchViewModel == null)
                return new List<Domain.Models.StudyPatient>();

            IList<Patient> patientsList = new List<Patient>();

            if (!string.IsNullOrEmpty(patientSearchViewModel.PatientName))
            {
                patientsList = _patientDepressionRepository.GetAll(p => (p.Firstname + " " + p.Lastname).Contains(patientSearchViewModel.PatientName));
            }
            else if (!string.IsNullOrEmpty(patientSearchViewModel.PatientPostCode))
            {
                patientsList = _patientDepressionRepository.GetAll(p => p.PostCode.Contains(patientSearchViewModel.PatientPostCode));
            }
            else if (!string.IsNullOrEmpty(patientSearchViewModel.PatientGP))
            {
                patientsList = _patientDepressionRepository.GetAll(p => p.GpPracticeName.Contains(patientSearchViewModel.PatientGP));
            }

            ConvertViewModelObjects cvmo = new ConvertViewModelObjects();

            return patientsList.Select(patient => cvmo.ConvertFromPatient(patient, this.StudyID, _gpPracticeAddressRepository)).ToList();
        }


        /// <summary>
        /// ToDo: support studyid
        /// </summary>
        /// <param name="patientViewModel"></param>
        /// <param name="userId"></param>
        public void UpdatePatient(ElephantParade.Domain.Models.StudyPatient patientViewModel, string userId)
        {
            //if(patientViewModel.StudyID=="")
            
            Guid depressionPatientId = Guid.Parse(patientViewModel.PatientId);
            
            ConvertViewModelObjects cvmo = new ConvertViewModelObjects();

            var patient = _patientDepressionRepository.GetSingle(p => p.PatientId == depressionPatientId);

            var depressionPatientIdString = Convert.ToString(depressionPatientId);
            var gpPracticeAddress = _gpPracticeAddressRepository.GetSingle(p => p.PatientID == depressionPatientIdString && p.StudyID==this.StudyID);
            if (gpPracticeAddress == null)
            {
                gpPracticeAddress = new GpPracticeAddress();
                gpPracticeAddress.PatientID = Convert.ToString(patient.PatientId);
                gpPracticeAddress.StudyID = this.StudyID;
                _gpPracticeAddressRepository.Add(gpPracticeAddress);                
            }

            ElephantParade.Domain.Models.StudyPatient original;
            original = cvmo.ConvertFromPatient(patient, this.StudyID, gpPracticeAddress);

            patient = cvmo.ConvertFromPatientViewModel(patientViewModel, patient, ref gpPracticeAddress);           

            _patientDepressionRepository.SaveChanges();
            _gpPracticeAddressRepository.SaveChanges();


            // Add Patient History
            if (this.PatientDetailsChanged != null)
            {
                this.PatientDetailsChanged(this, original, patientViewModel);
            }
        }

        #endregion
        
    
        public Domain.Models.PatientMedicalConditions MedicalConditions_List(string patientID)
        {

            ConvertViewModelObjects cvmo = new ConvertViewModelObjects();
            var meds = _patientMedicationRepository.GetQueryable().Where(m => m.PatientID == patientID && m.StudyID == this.StudyID).ToList();

            Domain.Models.PatientMedicalConditions pmc = new Domain.Models.PatientMedicalConditions();
            pmc.PatientId = patientID;
            pmc.StudyID = this.StudyID;
            pmc.MedicationsConditions = meds.Select(m => cvmo.ConvertFromPatientMedication(m)).ToList();

            return pmc;
        }

        public void MedicalConditions_Save(string patientID, Domain.Models.MedicalConditionItem item)
        {
            var p = this.GetPatient(patientID);
            if (p == null)
                throw new ArgumentException(string.Format("cannot find patient {0}", patientID));

            PatientMedication mci;
            if(item.ItemID != null)
            {
                mci = _patientMedicationRepository.GetQueryable().Where(m=>m.ItemID == item.ItemID.Value).FirstOrDefault();
                if(mci == null)
                    throw new ArgumentException(string.Format("cannot find MedicalConditionItem {0}", item.ItemID.Value));
                if(mci.StudyID != this.StudyID || mci.PatientID != patientID )
                    throw new ArgumentException(string.Format("cannot update MedicalConditionItem {0}. Study or patientid missmatch", item.ItemID.Value));
            }
            else
            {
                mci = new PatientMedication();
                 _patientMedicationRepository.Add(mci);
            }

            ConvertViewModelObjects cvmo = new ConvertViewModelObjects();
            cvmo.ConvertFromPatientMedicationVM(item,mci);
            mci.PatientID = patientID;
            mci.StudyID = this.StudyID;
            var i = _patientMedicationRepository.SaveChanges();
            if (i == 0)
                throw new Exception("Save failed for MedicalConditions item.");
        }


        public void MedicalConditions_Delete(int itemID)
        {
            var e = _patientMedicationRepository.GetSingle(m=>m.ItemID == itemID);
            _patientMedicationRepository.Delete(e);
            _patientMedicationRepository.SaveChanges();
        }

        #region IPatientService Members

        #endregion


        
    }
}
