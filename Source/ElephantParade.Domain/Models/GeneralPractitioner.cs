// -----------------------------------------------------------------------
// <copyright file="GeneralPractitioner.cs" company="NHS Direct">
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

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class GeneralPractitioner
    {
        //ToDo:Attributes
        [Display(Name = "GPNameLabelText", ResourceType = typeof(Resources))]
        public string Name { get; set; }

        [Display(Name = "GPPracticeLabelText", ResourceType = typeof(Resources))]
        public string Practice { get; set; }

        [Display(Name = "GPAddressLabelText", ResourceType = typeof(Resources))]
        public Address Address { get; set; }

        [RegularExpression(@"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$", ErrorMessageResourceName = "EmailAddressInvalid", ErrorMessageResourceType = typeof(Resources))]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "EmailAddressRequired", ErrorMessageResourceType = typeof(Resources))]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "GPEmailLabelText", ResourceType = typeof(Resources))]
        public string EmailAddress { get; set; }

        [Display(Name = "GPTelephoneLabelText", ResourceType = typeof(Resources))]
        public string TelephoneNumber { get; set; }

        [Display(Name = "GPPrimaryCareTrustLabelText", ResourceType = typeof(Resources))]
        public string PrimaryCareTrust { get; set; }
    }
}
