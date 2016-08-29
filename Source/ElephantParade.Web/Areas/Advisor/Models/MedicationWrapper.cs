using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NHSD.ElephantParade.Web.Areas.Advisor.Models
{
    public class MedicationWrapper
    {
        public MedicationWrapper()
        {
            PatientMedicalConditions = new Domain.Models.PatientMedicalConditions();
            //NewMedicalConditionItem = new NHSD.ElephantParade.Domain.Models.MedicalConditionItem();
        }
        public string StudyID { get { return PatientMedicalConditions.StudyID; } set { PatientMedicalConditions.StudyID = value; } }
        public string PatientId { get { return PatientMedicalConditions.PatientId; } set { PatientMedicalConditions.PatientId = value; } }

        public Domain.Models.PatientMedicalConditions PatientMedicalConditions { get; set; }
        public Domain.Models.MedicalConditionItem NewMedicalConditionItem { get; set; }
    }
}