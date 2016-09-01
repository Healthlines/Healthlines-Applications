using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using NHSD.ElephantParade.Domain.Models;
using System.Collections.Generic;
using NHSD.ElephantParade.Domain.Properties;

namespace NHSD.ElephantParade.Domain.Models
{
    public class PatientEmailAddressViewModel
    {
        [RegularExpression(@"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$", ErrorMessageResourceName = "ExistingEmailAddressInvalid", ErrorMessageResourceType = typeof(Resources))]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "ExistingEmailAddressRequired", ErrorMessageResourceType = typeof(Resources))]
        [DataType(DataType.EmailAddress)]
        [DisplayName("Enter existing email address")]
        public string ExistingEmailAddress { get; set; }

        [RegularExpression(@"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$", ErrorMessageResourceName = "NewEmailAddressInvalid", ErrorMessageResourceType = typeof(Resources))]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "NewEmailAddressRequired", ErrorMessageResourceType = typeof(Resources))]
        [DataType(DataType.EmailAddress)]
        [DisplayName("Enter new email address")]
        public string NewEmailAddress { get; set; }

        public string ReturnURL { get; set; }
    }
}
