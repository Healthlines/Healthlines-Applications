// -----------------------------------------------------------------------
// <copyright file="QuestionnaireLetterAction.cs" company="NHS Direct">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace NHSD.ElephantParade.Domain.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// Represents a required or generated letter for a patient from a questionnarire
    /// </summary>
    public class QuestionnaireLetterAction
    {
        public QuestionnaireLetterAction(){}

        [ScaffoldColumn(true)]
        public int ID { get; set; }

        [Required]
        public String StudyID { get; set; }

        [Required]
        public string PatientID { get; set; }

        [Required]
        public string ResultSetID { get; set; }

        [Required]
        public LetterType LetterTemplate { get; set; }

        string _letterTitle = null;
        public string LetterTitle {
            get
            {
                if (_letterTitle == null)
                {
                    _letterTitle = "";
                }
                return _letterTitle;
            }
            set
            {
                _letterTitle = value;
            }
        }

        public LetterTarget LetterTarget { get; set; }

        public DateTime? ProcessedDate { get; set; }

        public String ProcessedBy { get; set; }

        public int? FileID { get; set; }

        
    }
}
