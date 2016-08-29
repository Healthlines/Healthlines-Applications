// -----------------------------------------------------------------------
// <copyright file="LetterDetails.cs" company="NHS Direct">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace NHSD.ElephantParade.DocumentGenerator.Letters
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
using NHSD.ElephantParade.Domain.Models;

    /// <summary>
    /// describes a letter and its requirements
    /// </summary>
    public class LetterDetails
    {
        // a list of fields needed to generate the letter
        public IDictionary<string, LetterUserContent> UserFields { get; set; }
        //the display name of the letter
        public string LetterName { get; set; }
        //the intended recipent of the letter
        public LetterTarget LetterTarget { get; set; }
        //the Template ID of the letter
        public string LetterTemplate { get; set; }   
    
        public  string Description { get; set; }
    }
}
