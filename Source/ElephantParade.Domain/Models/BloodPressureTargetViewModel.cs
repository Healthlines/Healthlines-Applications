using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using NHSD.ElephantParade.Domain.Models;
using System.Collections.Generic;

namespace NHSD.ElephantParade.Domain.Models
{
    public class BloodPressureTargetViewModel : IValidatableObject
    {
        public StudyPatient Patient;

        [Required(ErrorMessage = "Please enter the Systolic Target")]
        [DataType(DataType.Text)]
        [DisplayName("Systolic Target (High reading): ")]
        public string SystolicTarget
        { get; set; }

        [Required(ErrorMessage = "Please enter the Diastolic Target")]
        [DataType(DataType.Text)]
        [DisplayName("Diastolic Target (Low reading): ")]
        public string DiastolicTarget
        { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            List<ValidationResult> result = new List<ValidationResult>();

            if (string.IsNullOrEmpty(SystolicTarget) || string.IsNullOrEmpty(DiastolicTarget))
                result.Add(new ValidationResult("Please enter both systolic target and diastolic target value."));
            else
            {
                int diastolic = 0;
                int systolic = 0;

                try
                {
                    diastolic = int.Parse(DiastolicTarget);
                    systolic = int.Parse(SystolicTarget);
                }
                catch (FormatException)
                {
                    result.Add(new ValidationResult("The diastolic and systolic targets must be set as numbers.", new List<string> {"Diastolic target", "Systolic target"}));
                    return result;
                }

                if (diastolic < 1)
                    result.Add(new ValidationResult("The diastolic target must be set.", new List<string> { "Diastolic target" }));

                if (systolic < 1)
                    result.Add(new ValidationResult("The systolic target must be set.", new List<string> { "Systolic target" }));

                if (diastolic >= systolic)
                {
                    result.Clear();
                    result.Add(new ValidationResult("The systolic target must be greater than the diastolic target.", new List<string> { "Systolic target" }));
                }

                Validator.TryValidateProperty(this.DiastolicTarget,
                    new ValidationContext(this, null, null)
                    {
                        MemberName = "DiastolicTarget"
                    },
                    result);

                Validator.TryValidateProperty(this.SystolicTarget,
                    new ValidationContext(this, null, null)
                    {
                        MemberName = "SystolicTarget"
                    },
                    result);
            }

            return result;
        }
    }
}
