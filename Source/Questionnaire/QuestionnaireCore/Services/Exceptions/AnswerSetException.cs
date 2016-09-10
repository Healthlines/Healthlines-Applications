// -----------------------------------------------------------------------
// <copyright file="QuestionnaireException.cs" company="NHS Direct">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Questionnaires.Core.Services.Exceptions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class AnswerSetException : Exception 
    {
        public int AnswerSetID { get; private set; }
        public AnswerSetException(string message,int answerSetID,Exception innerException)
            : base(message, innerException) 
        {
            this.AnswerSetID = answerSetID;
        }

        public AnswerSetException(string message, int answerSetID)
            : this(message,answerSetID,null)
        {
            
        }
    }
}
