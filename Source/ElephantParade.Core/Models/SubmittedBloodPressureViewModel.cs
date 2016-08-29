using NHSD.ElephantParade.Domain.Models;

namespace NHSD.ElephantParade.Core.Models
{
    public class SubmittedBloodPressureViewModel
    {
        //public string Warning { get; set;}
        public BloodPressureTargetViewModel Targets { get; set; }
        public BloodPressureReadingViewModel Readings { get; set; }
        public string DateOfNextReading { get; set; }
    }
}