// -----------------------------------------------------------------------
// <copyright file="Answer.cs" company="NHS Direct">
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
    /// Used to encapsulate answer information for a question
    /// </summary>
    public class Answer
    {
        public virtual int QuestionID
        { get; set; }        

        public virtual string OperatorID
        { get; set; }

        public virtual AnswerOption.OptionType Type
        { get; set; }

        public virtual string Value
        { get; set; }

        public virtual int AnswerOptionID
        { get; set; }
    }
}
