// -----------------------------------------------------------------------
// <copyright file="StudyPatient.cs" company="NHS Direct">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace NHSD.ElephantParade.Web.Areas.Administration.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// Represents a Patient to import.
    /// Includes supporting data required for an import
    /// </summary>
    public class PatientImportData
        :IValidatableObject 
    {
        public Domain.Models.StudyPatient Patient { get; set; }

        // Notes is used both as patient comments in the CAD system as well as in DUKE if it is a CVD patient
        public string Notes
        { get; set; }

        // Additional fields that differ from the two types of patients
        // The following are CVD Data
        public decimal TargetDiastolic
        { get; set; }

        public decimal TargetSystolic
        { get; set; }   

        public string BaselineQriskScore
        { get; set; }

        public string BaselineHeight
        { get; set; }

        public string BaselineWeight
        { get; set; }

        public string BaselineBMI
        { get; set; }

        public string OnBPMeds
        { get; set; }

        public decimal BaselineSystolicBP
        { get; set; }

        public decimal BaselineDiastolicBP
        { get; set; }

        public string BaselineSmokingStatus
        { get; set; }

        public decimal TotalCholesterolRatio
        { get; set; }

        public string Diabetes
        { get; set; }

        public string ChronicKidneyDisease
        { get; set; }

        public string AtrialFibrillation
        { get; set; }

        public string DepressionStudyId
        { get; set; }

        public string CvdStudyId
        { get; set; }
        

        public PatientImportData(string depressionStudyId, string cvdStudyId)
        {
            DepressionStudyId = depressionStudyId;
            CvdStudyId = cvdStudyId;
        }

        public IEnumerable<ValidationResult>  Validate(ValidationContext validationContext)
        {
            var results = new List<ValidationResult>();
            var context = new ValidationContext(Patient, serviceProvider: null, items: null);
            
            Validator.TryValidateObject(Patient, context, results,true);
            
            if (String.IsNullOrWhiteSpace(Patient.StudySite))
                results.Add(new ValidationResult( "Invalid Study Site"));

            if (Patient.DOB == null)
                results.Add(new ValidationResult("Patient DOB Required"));

            if (String.IsNullOrWhiteSpace(Patient.Address.Line1))
                results.Add(new ValidationResult("Patient AddressLine1 Required"));

            if (String.IsNullOrWhiteSpace(Patient.Address.PostCode))
                results.Add(new ValidationResult("Patient Postcode Required"));

            if (String.IsNullOrWhiteSpace(Patient.TelephoneNumber) &&
                             String.IsNullOrWhiteSpace(Patient.TelephoneNumberMobile) &&
                             String.IsNullOrWhiteSpace(Patient.TelephoneNumberOther))
                results.Add(new ValidationResult("One Patient Contact Number Required"));

            if (String.IsNullOrWhiteSpace(Patient.GPPractice.PrimaryCareTrust))
                results.Add(new ValidationResult("GPPractice.PrimaryCareTrust Required"));

            if (String.IsNullOrWhiteSpace(Patient.GPPractice.Practice))
                results.Add(new ValidationResult("GPPractice.Practice Required"));

            if (String.IsNullOrWhiteSpace(Patient.GPPractice.EmailAddress))
                 results.Add(new ValidationResult("GPPractice.EmailAddress Required"));

            if (String.IsNullOrWhiteSpace(Patient.GPPractice.Name))
                results.Add(new ValidationResult("GPPractice.Name Required"));

            if (String.IsNullOrWhiteSpace(Patient.GPPractice.Address.Line1))
                results.Add(new ValidationResult("GPPractice.Address.Line1 Required"));

            if (String.IsNullOrWhiteSpace(Patient.GPPractice.Address.PostCode))
                results.Add(new ValidationResult("GPPractice.Address.PostCode Required"));

            if (String.IsNullOrWhiteSpace(Patient.Gender))
                results.Add(new ValidationResult("Patient Gender Required"));

            if (String.IsNullOrWhiteSpace(Patient.Ethnicity))
                results.Add(new ValidationResult("Patient Ethnicity Required"));

            if (!String.IsNullOrWhiteSpace(Patient.GPPractice.EmailAddress) && !Patient.GPPractice.EmailAddress.Trim().ToLower().EndsWith("@nhs.net"))
                results.Add(new ValidationResult("Invalid GP Practice Email Address"));

            if(Patient.StudyReferralDate == null || Patient.StudyReferralDate == DateTime.MinValue)
                results.Add(new ValidationResult("Study Referral Date Required"));
                
            if (Patient.StudyID == DepressionStudyId)
            {
                //if (Patient.BaselinePHQ9<=0)
                //    results.Add(new ValidationResult("BaselinePHQ9 Required"));
                //if (Patient.BaselineGAD7 <= 0)
                //    results.Add(new ValidationResult("BaselineGAD7 Required"));
            }
            else if (Patient.StudyID == CvdStudyId)
            {
                if(String.IsNullOrWhiteSpace(this.BaselineHeight))
                    results.Add(new ValidationResult("Baseline Height Required"));
                if (String.IsNullOrWhiteSpace(this.AtrialFibrillation))
                    results.Add(new ValidationResult("Atrial Fibrillation Required"));
                if(this.BaselineDiastolicBP <=0)
                    results.Add(new ValidationResult("Baseline DiastolicBP Required"));
                if (String.IsNullOrWhiteSpace(this.BaselineQriskScore))
                    results.Add(new ValidationResult("Baseline QriskScore Required"));
                if (String.IsNullOrWhiteSpace(this.BaselineSmokingStatus))
                    results.Add(new ValidationResult("Baseline SmokingStatus Required"));
                if (this.BaselineSystolicBP <= 0)
                    results.Add(new ValidationResult("Baseline SystolicBP Required"));
                if (String.IsNullOrWhiteSpace(this.BaselineWeight))
                    results.Add(new ValidationResult("Baseline Weight Required"));
                if (String.IsNullOrWhiteSpace(this.ChronicKidneyDisease))
                    results.Add(new ValidationResult("Chronic Kidney Disease Required"));
                if (String.IsNullOrWhiteSpace(this.Diabetes))
                    results.Add(new ValidationResult("Diabetes Required"));
                if (String.IsNullOrWhiteSpace(this.OnBPMeds))
                    results.Add(new ValidationResult("On BP Meds Required"));
                if (this.TargetDiastolic <= 0)
                    results.Add(new ValidationResult("Target Diastolic Required"));
                if (this.TargetSystolic <= 0)
                    results.Add(new ValidationResult("Target Systolic Required"));
                if (this.TotalCholesterolRatio <= 0)
                    results.Add(new ValidationResult("Total Cholesterol Ratio Required"));
            }
            return results;
        }
    }
}

