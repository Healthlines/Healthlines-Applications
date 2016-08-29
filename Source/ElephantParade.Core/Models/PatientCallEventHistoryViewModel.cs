using System;
using System.ComponentModel.DataAnnotations;

namespace NHSD.ElephantParade.Core.Models
{
    public class CallEventViewModel
    {        
        [Required(AllowEmptyStrings = false, ErrorMessage = "Patient ID Required")]
        [Display(Name = "Patient ID")]
        public virtual string PatientID { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Study ID Required")]
        [Display(Name = "Study ID")]
        public virtual string StudyID { get; set; }

        [Display(Name = "Date")]
        public virtual DateTime Date { get; set; }

        [Display(Name = "Text")]
        public virtual string Text { get; set; }

        [Display(Name = "Call Handler")]
        public virtual string UserID { get; set; }

        [Display(Name = "Type")]
        public virtual string EventCode { get;  set; }
    }
}
