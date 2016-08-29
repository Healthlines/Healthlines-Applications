// -----------------------------------------------------------------------
// <copyright file="ModuleLetter.cs" company="NHS Direct">
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
    public class ModuleLetter
    {
        
        public bool Required { get; set; }
        public NHSD.ElephantParade.Domain.Models.LetterActionData LetterActionData {get;set;}
    }
}
