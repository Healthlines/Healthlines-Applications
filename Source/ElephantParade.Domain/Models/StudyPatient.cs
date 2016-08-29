// -----------------------------------------------------------------------
// <copyright file="StudyPatient.cs" company="NHS Direct">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace NHSD.ElephantParade.Domain.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.ComponentModel.DataAnnotations;
    using NHSD.ElephantParade.Domain.Properties;
    using System.ComponentModel;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class StudyPatient
        :Patient
    {

        #region enums
        public enum PreferredContactNumberType
	    {
	        Home,
            Other,
            Mobile
	    }
       public enum PatientStudyStatusType
       {           
           Active = 1,
           Suspended,
           Withdrawn,
           Refused,
           Complete
       }
        #endregion

        public StudyPatient()
        {
            GPPractice = new GeneralPractitioner();
        }

       #region Study Information

       [ScaffoldColumn(true)]
       [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "StudyIdRequired", ErrorMessageResourceType = typeof(Resources))]
        public string StudyID
        { get; set; }

        [Display(Name = "StudyTrialNumberLabelText", ResourceType = typeof(Resources))]
        public string StudyTrialNumber
        { get; set; }

        [Display(Name = "StudySiteLabelText", ResourceType = typeof(Resources))]
        public String StudySite { get; set; }

        [Display(Name = "PatientStatusLabelText", ResourceType = typeof(Resources))]
        public PatientStudyStatusType Status
        {
            get;
            set;
        }

        [Display(Name = "StudyConsentedDateLabelText", ResourceType = typeof(Resources))]       
        public DateTime? StudyConsentedDate
        { get; set; }

        [Display(Name = "StudyReferralDateLabelText", ResourceType = typeof(Resources))]
        public DateTime? StudyReferralDate
        { get; set; }

        [Display(Name = "EducationLabelText", ResourceType = typeof(Resources))]
        public string Education
        { get; set; }

       #endregion

        [StringLength(15, ErrorMessageResourceName = "PatientNhsNumberStringLengthValidationError", ErrorMessageResourceType = typeof(Resources))]
       //[Required(AllowEmptyStrings = false, ErrorMessageResourceName = "PatientNhsNumberRequired", ErrorMessageResourceType = typeof(Resources))]
       [Display(Name = "PatientNhsNumberLabelText", ResourceType = typeof(Resources))]
       public string NhsNumber { get; set; }

        [Display(Name = "PatientPreferredContactNumberTypeLabelText", ResourceType = typeof(Resources))]
        public PreferredContactNumberType PreferredContactNumber { get; set; }

        [Display(Name = "PatientPrederredContactTimeLabelText", ResourceType = typeof(Resources))]
        public string PrederredContactTime { get; set; }

        public GeneralPractitioner GPPractice { get; set; }
        
        [Display(Name = "PatientOnAntidepressantsTypeLabelText", ResourceType = typeof(Resources))]
        public Boolean OnAntidepressants
        { get; set; }
        [Display(Name = "PatientBaselinePHQ9LabelText", ResourceType = typeof(Resources))]
        public decimal BaselinePHQ9
        { get; set; }
        [Display(Name = "PatientBaselineGAD7TypeLabelText", ResourceType = typeof(Resources))]
        public decimal BaselineGAD7
        { get; set; }

        public string getPreferedContactNumber()
        {
            switch (this.PreferredContactNumber )
            {
                case PreferredContactNumberType.Home:
                    return this.TelephoneNumber;
                case PreferredContactNumberType.Other:
                    return this.TelephoneNumberOther ;
                case PreferredContactNumberType.Mobile:
                    return this.TelephoneNumberMobile;
                default:
                    return this.TelephoneNumber;
            }
        }

        
    }
}
