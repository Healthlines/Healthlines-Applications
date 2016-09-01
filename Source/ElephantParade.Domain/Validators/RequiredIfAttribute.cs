using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace NHSD.ElephantParade.Domain.Validators
{
    public class RequiredIfAttribute : ConditionalAttributeBase, IClientValidatable
    {
        private RequiredAttribute _innerAttribute = new RequiredAttribute();

        public string DependentProperty { get; set; }
        public object TargetValue { get; set; }

        public RequiredIfAttribute(string dependentProperty, object targetValue)
            : this(dependentProperty, targetValue, null)
        {
        }

        public RequiredIfAttribute(string dependentProperty, object targetValue, string errorMessage)
            : base(errorMessage)
        {
            this.DependentProperty = dependentProperty;
            this.TargetValue = targetValue;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            // check if the current value matches the target value
            if (ShouldRunValidation(value, this.DependentProperty, this.TargetValue, validationContext))
            {
                // match => means we should try validating this field
                if (!_innerAttribute.IsValid(value))
                    // validation failed - return an error
                    return new ValidationResult(FormatErrorMessage(validationContext.DisplayName), new[] { validationContext.MemberName });
            }

            return ValidationResult.Success;
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            var rule = new ModelClientValidationRule()
            {
                ErrorMessage = FormatErrorMessage(metadata.GetDisplayName()),
                ValidationType = "mvcvtkrequiredif",
            };

            string depProp = BuildDependentPropertyId(metadata, context as ViewContext);

            // find the value on the control we depend on;
            // if it's a bool, format it javascript style 
            // (the default is True or False!)
            string targetValue = (this.TargetValue ?? "").ToString();
            if (this.TargetValue != null && this.TargetValue.GetType() == typeof(bool))
                targetValue = targetValue.ToLower();

            rule.ValidationParameters.Add("dependentproperty", depProp);
            rule.ValidationParameters.Add("targetvalue", targetValue);

            yield return rule;
        }

        private string BuildDependentPropertyId(ModelMetadata metadata, ViewContext viewContext)
        {
            return QualifyFieldId(metadata, this.DependentProperty, viewContext);
        }

        public override string FormatErrorMessage(string name)
        {
            if (!String.IsNullOrEmpty(this.ErrorMessageString))
                _innerAttribute.ErrorMessage = this.ErrorMessageString;
            return _innerAttribute.FormatErrorMessage(name);
        }
    }
}
