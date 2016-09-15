// -----------------------------------------------------------------------
// <copyright file="InvalidAnswerException.cs" company="NHS Direct">
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
    /// Represents errors that occur when an invalid answer is provided
    /// </summary>
    public class AnswerException : Exception 
    {
        public AnswerException(string message) : base(message) 
        {

        }
    }
    /// <summary>
    /// Represents errors that occur when an answers are provided from different pages
    /// </summary>
    public class AnswerExceptionNotInQuestionSetPage : AnswerException
    {        
        public AnswerExceptionNotInQuestionSetPage(string message):base(message)
        {
            
        }
    }

    public class AnswerExceptionMissingAnswer : AnswerException
    {
        public AnswerExceptionMissingAnswer(string message)
            : base(message)
        {

        }
    }
}
