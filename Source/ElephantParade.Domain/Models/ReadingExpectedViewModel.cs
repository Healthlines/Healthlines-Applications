using System;

namespace NHSD.ElephantParade.Domain.Models
{
    public class ReadingExpectedViewModel
    {
        public DateTime Date { get; set; }
        public string PatientId { get; set; }
        public string StudyId { get; set; }
        public bool ReadingGiven { get; set; }
    }
}
