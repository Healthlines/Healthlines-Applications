using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace NHSD.ElephantParade.Web.Areas.Advisor.Models
{
    public class BloodPressureReadingsWrapper
    {
        public BloodPressureReadingsWrapper()
        {            
            BPStartDateToSendToGP = DateTime.Today.Date.AddMonths(-1);
            BPEndDateToSendToGP = DateTime.Today.Date;
        }

        public IList<Domain.Models.BloodPressureReadingViewModel> BloodPressureReadings { get; set; }

        //public string StudyId { get; set; }
        //public string PatientId { get; set; }

        public string AverageSystolicBPReading;
        public string AverageDiastolicBPReading;
        public string AverageBPReadingFrequencyText;
        public int ValidReadingsCount;

        //public string EmailContent { get; set; }

        [Display(Name = "Start Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime BPStartDateToSendToGP { get; set; }

        [Display(Name = "End Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime BPEndDateToSendToGP { get; set; }
    }
}
