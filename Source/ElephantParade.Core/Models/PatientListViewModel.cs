using System.Collections.Generic;

namespace NHSD.ElephantParade.Core.Models
{
    public class PatientListViewModel
    {
        public List<NHSD.ElephantParade.Domain.Models.StudyPatient> PatientList { get; set; }
        public int TotalItemCount { get; set; }
        public int ThisPageItemCount { get; set; }
        public int TotalPageCount { get; set; }
    }
}

