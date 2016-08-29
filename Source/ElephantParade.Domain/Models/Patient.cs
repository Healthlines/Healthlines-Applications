using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using NHSD.ElephantParade.Domain.Properties;


namespace NHSD.ElephantParade.Domain.Models
{
    
    public class Patient
    {

        /// <summary>
        /// Gets or sets the identifier for the user.
        /// </summary>
        [ScaffoldColumn(true)]
        public string PatientId { get; set; }

        /// <summary>
        /// Gets or sets the user's display name.
        /// </summary>
        [StringLength(200, ErrorMessageResourceName = "PatientDisplayNameStringLengthValidationError", ErrorMessageResourceType = typeof(Resources))]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "PatientDisplayNameRequired", ErrorMessageResourceType = typeof(Resources))]
        [Display(Name = "PatientDisplayNameLabelText", ResourceType = typeof(Resources))]
        public string DisplayName 
        {
            get
            {
                if (_displayName == null )
                    _displayName =  string.Format("{0} {1} {2}", this.Title, this.Forename, this.Surname).Trim();
                return _displayName;
            }
            set { _displayName = value; } 
        }
        private string _displayName = null;

        [StringLength(10, ErrorMessageResourceName = "PatientTitleStringLengthValidationError", ErrorMessageResourceType = typeof(Resources))]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "PatientTitleRequired", ErrorMessageResourceType = typeof(Resources))]
        [Display(Name = "PatientTitleLabelText", ResourceType = typeof(Resources))]
        public string Title { get; set; }

        [StringLength(50, ErrorMessageResourceName = "PatientForenameStringLengthValidationError", ErrorMessageResourceType = typeof(Resources))]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "PatientForenameRequired", ErrorMessageResourceType = typeof(Resources))]
        [Display(Name = "PatientForenameLabelText", ResourceType = typeof(Resources))]
        public string Forename {get;set;}

        [StringLength(50, ErrorMessageResourceName = "PatientSurnameStringLengthValidationError", ErrorMessageResourceType = typeof(Resources))]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "PatientSurnameRequired", ErrorMessageResourceType = typeof(Resources))]
        [Display(Name = "PatientSurnameLabelText", ResourceType = typeof(Resources))]
        public string Surname { get; set; }

        //ToDo:Attributes
        [RegularExpression(@"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$", ErrorMessageResourceName = "EmailAddressInvalid", ErrorMessageResourceType = typeof(Resources))]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "EmailAddressRequired", ErrorMessageResourceType = typeof(Resources))]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "PatientEmailLabelText", ResourceType = typeof(Resources))]
        public string Email { get; set; }

        //ToDo:Attributes        
        [Display(Name = "PatientDOBLabelText",ResourceType =typeof(Resources))]
        [DataType(DataType.Date) ]
        public DateTime? DOB { get; set; }

        //ToDo:Attributes
        [Display(Name = "PatientGenderLabelText", ResourceType = typeof(Resources))]
        public String Gender { get; set; }

        [Display(Name = "PatientEthnicityLabelText", ResourceType = typeof(Resources))]
        public String Ethnicity { get; set; }

        //ToDo:Attributes
        [Display(Name = "PatientTelephoneLabelText",ResourceType =typeof(Resources))]
        [DataType(DataType.PhoneNumber)]
        public string TelephoneNumber { get; set; }

        //ToDo:Attributes
        [Display(Name = "PatientTelephoneNumberMobileLabelText", ResourceType = typeof(Resources))]
        [DataType(DataType.PhoneNumber)]
        public string TelephoneNumberMobile { get; set; }

        //ToDo:Attributes
        [Display(Name = "PatientTelephoneNumberOtherLabelText", ResourceType = typeof(Resources))]
        [DataType(DataType.PhoneNumber)]
        public string TelephoneNumberOther { get; set; }

        //ToDo:Attributes
        [Display(Name = "PatientRegisteredDateLabelText", ResourceType = typeof(Resources))]
        [DataType(DataType.Date)]
        public DateTime RegisteredDate { get; set; }

        //public Address Address { get; set; }
        [Display(Name = "PatientAddressLabelText", ResourceType = typeof(Resources))]
        public Address Address { get; set; }
        

    }
}