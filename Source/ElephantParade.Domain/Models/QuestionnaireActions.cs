// -----------------------------------------------------------------------
// <copyright file="QuestionnaireAction.cs" company="NHS Direct">
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
    /// Lists of actions available 
    /// </summary>
    public class QuestionnaireActions
        :QuestionnaireSession 
    {

        public virtual IEnumerable<ModuleLetter> LetterActionData
        { get; set; }
                
    }
}
