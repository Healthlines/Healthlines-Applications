// -----------------------------------------------------------------------
// <copyright file="PatientMedicalConditions.cs" company="NHS Direct">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace NHSD.ElephantParade.Domain.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class PatientMedicalConditions
    {
        public string StudyID   { get; set; }
        public string PatientId { get; set; }
        
        public IList<MedicalConditionItem> MedicationsConditions { get; set; }
    }
}
