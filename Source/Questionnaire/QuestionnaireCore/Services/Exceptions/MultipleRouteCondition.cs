// -----------------------------------------------------------------------
// <copyright file="MultipleRouteCondition.cs" company="NHS Direct">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Questionnaires.Core.Services.Exceptions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Questionnaires.Core.Services.Models;

    /// <summary>
    /// Represents an error that occurs when a page of answers results in more than one possible question route
    /// </summary>
    public class MultipleRouteCondition : ConfigurationException 
    {
        public int QuestionSetID {get; private set;}

        public MultipleRouteCondition(string message)
            : base(message)
        {

        }

        public MultipleRouteCondition(string message, Exception innerException, int questionSetID)
            : base(message, innerException)
        {
            QuestionSetID = questionSetID;
        }

    }
}
