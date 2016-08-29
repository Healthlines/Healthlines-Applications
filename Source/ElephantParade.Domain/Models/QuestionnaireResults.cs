// -----------------------------------------------------------------------
// <copyright file="AnswerSet.cs" company="NHS Direct">
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
    public class QuestionnaireResults
        :QuestionnaireSession 
    {
              

        public IEnumerable<Question> Questions
        { get; set; }


        public class Question
        {
            public string QuestionTitle
            { get; set; }
            public string QuestionText
            { get; set; }
            public virtual String[] Answers
            { get; set; }
            public virtual System.DateTime Date
            { get; set; }
            public virtual string OperatorID
            { get; set; }
            public virtual string QuestionExplaination 
            { get; set; }
        }
    }
}
