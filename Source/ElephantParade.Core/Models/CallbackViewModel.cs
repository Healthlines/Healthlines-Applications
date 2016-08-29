using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Collections.Generic;
using NHSD.ElephantParade.Core.Interfaces;
using NHSD.ElephantParade.Domain.Models;
using NHSD.ElephantParade.Domain.Properties;
using NHSD.ElephantParade.Domain.Validators;

namespace NHSD.ElephantParade.Core.Models
{
    [Description("Callback details.")]
    public class CallbackViewModel
        :IValidatableObject  //: ICallbackModel 
    {
        public enum CallbackType
        {
            Scheduled,            
            Patient,
            Recheduled
        }
        public Guid CallbackId { get; set; }

        [Required(ErrorMessage = "Please enter the Available From Time")]
        [Display(Name="Available from(hh:mm)")]
        public TimeSpan CallbackStartTime { get; set; }

        [Display(Name = "Available to (hh:mm)")]
        public TimeSpan? CallbackEndTime { get; set; }

        [Required(ErrorMessage = "Please enter the Scheduled Callback Date ")]
        [Display(Name="Date")]
        [DataType(DataType.Date)]
        public DateTime? CallbackDate { get; set; }

        [Display(Name="Callback ScheduledBy")]
        public string CallbackScheduledBy { get; set; }

        [Display(Name="PatientId")]
        public StudyPatient Patient { get; set; }

        public int? CallsCompleted { get; set; }
        public int? CallsFailed { get; set; }
       
        [Display(Name="Callback Type")]
        public CallbackType Type { get; set; }

        [Display(Name = "Completed")]
        public bool Completed { get; set; }

        [Display(Name = "Scheduled Date")]
        [DataType(System.ComponentModel.DataAnnotations.DataType.Date)]
        public DateTime CallbackScheduledDate { get ;set;}

        [Display(Name = "Date Locked")]
        [DataType(System.ComponentModel.DataAnnotations.DataType.DateTime)]
        public DateTime? LockedDate { get; set; }

        [Display(Name = "Locked To")]        
        public string LockedTo { get; set; }

        //[Required(ErrorMessage = "Please select the call outcome before rescheduling.")]
        [Display(Name = "Call outcome")]
        public string CallOutcome { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            List<ValidationResult> result = new List<ValidationResult>();
            if (CallbackDate < DateTime.Now.Date)
            {
                result.Add(new ValidationResult("The callback date must not be before today's date.", new List<string> { "Callback date" }));
            }

            Validator.TryValidateProperty(this.CallbackDate,
                new ValidationContext(this, null, null)
                {
                    MemberName = "CallbackDate"
                },
                result);

            return result;
        }
    }
}
