// -----------------------------------------------------------------------
// <copyright file="QuestionSetPageItem.cs" company="NHS Direct">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Questionnaires.Core.Services.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// Represents a QuestionSetQuestion including a hydrated question object
    /// </summary>
    public class QuestionSetPageItem : QuestionSetQuestion 
    {
        public Question Question
        {
            get;
            set;
        }
    }
}
