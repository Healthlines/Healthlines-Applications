using System;
using System.Linq;
using NHSD.ElephantParade.Core.Models;
using System.Collections.Generic;
using NHSD.ElephantParade.Domain.Models;
namespace NHSD.ElephantParade.Core.Interfaces
{
    public delegate void PatientUpdatedEventHandler(object sender, StudyPatient original, StudyPatient updated);
  

    public interface IPatientService //: IService<PatientViewModel>
    {       
        event PatientUpdatedEventHandler PatientDetailsChanged;
        

        /// <summary>
        /// get a unique id for the patient collection
        /// </summary>
        string StudyID { get; set; }
        StudyPatient AddPatient(StudyPatient Patient, string userId);
        //StudyPatient AddPatient(Domain.Models.StudyPatient patientViewModel, Domain.Models.Address gpAddressViewModel, string userId);
        void DeletePatient(string patientId, string userId);
        StudyPatient GetPatient(string PatientId);
        bool DoesPatientExistInTheDatabase(Domain.Models.StudyPatient spVM);
        StudyPatient GetPatientByEmail(string email);
        IList<string> GetPatientsInactive();
        IList<Domain.Models.StudyPatient> PatientSearch(PatientSearchViewModel patientSearchViewModel);

//        void UpdatePatientEmailAddress(StudyPatient patientViewModel, string userId);
        void UpdatePatient(StudyPatient patientViewModel, string userId);

        Domain.Models.PatientMedicalConditions MedicalConditions_List(string patientID);

        void MedicalConditions_Save(string patientID, MedicalConditionItem item);

        void MedicalConditions_Delete(int itemID);

        
        
    }
}