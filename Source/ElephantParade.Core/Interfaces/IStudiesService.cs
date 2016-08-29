// -----------------------------------------------------------------------
// <copyright file="IStudiesService.cs" company="NHS Direct">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

using NHSD.ElephantParade.Core.Models;

namespace NHSD.ElephantParade.Core.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using NHSD.ElephantParade.Domain.Models;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public interface IStudiesService
    {
        String DepressionStudyID { get; }
        String CVDStudyID { get; }
        IDepressionService DepressionService { get; }
        ICvdService CvdService { get; }

        #region Patients
        StudyPatient GetPatient(string patientId, string studyID);
        StudyPatient AddPatient(StudyPatient patient, string userId);
        void DeletePatient(StudyPatient newPatient,string userId);
        Domain.Models.StudyPatient GetPatientByEmail(string email);
        void UpdatePatient(StudyPatient patientViewModel, string userid);
        IList<Domain.Models.StudyPatient> PatientSearch(PatientSearchViewModel patientSearchViewModel);
        bool DoesPatientExistInTheDatabase(StudyPatient studyPatient);
        IList<string> GetPatientsInactive();
        #endregion

        #region MedicalConditions
        Domain.Models.PatientMedicalConditions MedicalConditions_ListByPatient(String studyId, string patientId);
        void MedicalConditions_Save(string studyId, string patientId,Domain.Models.MedicalConditionItem item);
        void MedicalConditions_Delete(string studyId, int itemId);      
        #endregion

        #region Files

        PatientFile FileSave(PatientFileData file);
        PatientFile FileSave(PatientFile file);
        IList<PatientFile> FileList(string studyId, string patientId);
        PatientFileData FileGetData(int fileId); 

        #endregion

        #region PatientEvents

        void PatientEventAdd(PatientEvent newEvent);
        List<PatientEvent> PatientEventList(string studyId, string patientId);

        #endregion




        
    }
}
