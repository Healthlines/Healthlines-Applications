using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace NHSD.ElephantParade.Domain.Models
{
    public abstract class Reading
    { 
        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        [DisplayName("Date reading taken")]
        public DateTime? DateOfReading
        { get; set; }
 
        [DataType(DataType.Time)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:hh:mm}")]
        [DisplayName("Time reading taken")]
        public DateTime? TimeOfReading
        { get; set; }

        public string PatientId
        { get; set; }

        public string SubmittedBy 
        { get; set; }

        public string StudyId
        { get; set; }

        public bool Valid
        { get; set; }
    }
}