// -----------------------------------------------------------------------
// <copyright file="QuestionnaireLetterActionData.cs" company="NHS Direct">
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
    /// letter details and required data for merging
    /// </summary>
    public class LetterActionData
    {
        //public string LetterTitle { get; set; }
        public LetterType LetterTemplate { get; set; }
        public LetterTarget LetterTarget { get; set; }
        public Dictionary<string, object> LetterValues { get; set; }
    }
    
}
