using System;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace NHSD.ElephantParade.Domain.Validators
{
    public sealed class CompareBloodPressureAttribute : ValidationAttribute
    {
        private const string _errorMessage = "The first blood pressure number should be higher than the second.";
        public string HighReadingProperty { get; set; }
        public override bool IsValid(object value)
        {
            // Get Value from the HighReading property
            string highReadingString = HttpContext.Current.Request[HighReadingProperty];
            if (highReadingString == string.Empty)
            { return false; }

            int lowReading;
            int highReading;

            try
            {
                if (value != null)
                    lowReading = (int)value;
                else
                    lowReading = 0;

                if (!String.IsNullOrEmpty(highReadingString))
                    highReading = int.Parse(highReadingString);
                else
                    highReading = 0;
            }
            catch (FormatException)
            {
                return false;
            }
            catch (OverflowException)
            {
                return false;
            }

            // low reading must be less than high reading.
            return lowReading < highReading;
        }

        /// <summary>
        /// Return custom error message.
        /// </summary>
        public override string FormatErrorMessage(string name)
        {
            return _errorMessage;
        }
    }
}
