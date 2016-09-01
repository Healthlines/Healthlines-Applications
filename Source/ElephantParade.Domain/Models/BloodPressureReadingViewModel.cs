using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using NHSD.ElephantParade.Domain.Validators;

namespace NHSD.ElephantParade.Domain.Models
{
    public class BloodPressureReadingViewModel: Reading
    {
        // BP high reading
        [Required]
        [DataType(DataType.Text)]
        public int Systolic
        { get; set; }

        // BP low reading
        [Required]
        [DataType(DataType.Text)]
        [CompareBloodPressure(HighReadingProperty = "Systolic")]
        public int Diastolic
        { get; set; }

        // BP high and low reading - i.e. it is the systolic reading / diastolic reading (e.g. 100/75)
        public string BPHighAndLowReading
        { get; set; }

        // BP status - the status where the blood pressure has overdue a reading or the BP has exceeded threshold
        public BPStatus BPStatus
        { get; set; }

        //Target Values for this reading
        public BloodPressureTargetViewModel Target 
        { get; set; }

    }
}