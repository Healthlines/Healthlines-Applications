using System;
using System.ComponentModel.DataAnnotations;

namespace NHSD.ElephantParade.Domain.Models
{
    public class ReadingTarget
    {
        [DataType(DataType.DateTime)]
        public DateTime DateSet
        { get; set; }

        [Required]
        [DataType(DataType.Text)]
        public string Target
        { get; set; }

        public ReadingTypes ReadingType
        { get; set; }

        public string PatientId
        { get; set; }

        public string StudyId
        { get; set; }

        public string SubmittedBy
        { get; set; }

        public bool Valid
        { get; set; }
    }
}
