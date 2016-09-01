// -----------------------------------------------------------------------
// <copyright file="MedicalCondition.cs" company="NHS Direct">
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
    using NHSD.ElephantParade.Domain.Validators;
    using System.ComponentModel;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class MedicalConditionItem//:IValidatableObject

    {
        public enum ItemType
        {
            [DescriptionAttribute("Medicine")]
            Medication,
            Condition
        }
        public MedicalConditionItem()
        {
        }
        public MedicalConditionItem(int itemID)
        {
            ItemID = itemID;
        }

        public int? ItemID { get; set; }

        //[Required(AllowEmptyStrings = false, ErrorMessageResourceName = "MedicationTypeRequired", ErrorMessageResourceType = typeof(Resources))]
        [Display(Name = "MedicationTypeLabelText", ResourceType = typeof(Resources))]
        public ItemType Type { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "MedicationTypeRequiredText", ErrorMessageResourceType = typeof(Resources))]
        [Display(Name = "MedicationNameLabelText", ResourceType = typeof(Resources))]
        public string Name { get; set; }

        [RequiredIf("Type", ItemType.Medication, ErrorMessageResourceName = "MedicationDoseRequiredText", ErrorMessageResourceType = typeof(Resources))]
        [Display(Name = "MedicationDoseLabelText", ResourceType = typeof(Resources))]
        public string Dose { get; set; }

        [RequiredIf("Type", ItemType.Medication,ErrorMessageResourceName = "MedicationfrequencyRequiredText", ErrorMessageResourceType = typeof(Resources))]
        [Display(Name = "MedicationfrequencyLabelText", ResourceType = typeof(Resources))]
        public string Frequency { get; set; }

        //the below is not required due to the addition of the RequiredIf custom validator but can be interchanged if required
        /*
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            List<ValidationResult> result = new List<ValidationResult>();
            if (Type == ItemType.Mediaction && string.IsNullOrEmpty(Dose))
            {
                result.Add(new ValidationResult("Dose must be supplied for Mediaction type",new List<string>{"Dose"}));
               
            }
            if (Type == ItemType.Mediaction && string.IsNullOrEmpty(Frequency))
            {
                result.Add(new ValidationResult("Frequency must be supplied for Mediaction type.", new List<string> { "Frequency" }));

            }

            Validator.TryValidateProperty(this.Dose, 
                new ValidationContext(this, null, null) {
                    MemberName = "Dose" 
                }, 
                result);
            Validator.TryValidateProperty(this.Frequency,
                new ValidationContext(this, null, null)
                {
                    MemberName = "Dose"
                },
                result);                
            return result;
        }*/
    }
}
