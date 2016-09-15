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
    /// Representes and an answer within an answer set
    /// </summary>
    public class AnswerSetAnswer
    {
        public struct AnswerValue
        {
		    public AnswerOption.OptionType Type;
            public string Value;
        }

        #region Fields

        private int _answerSetID;
        private int _questionID;

        #endregion

        #region Properties
    
        public virtual int AnswerSetID
        {
            get { return _answerSetID; }
            set { _answerSetID = value; }
        }        
    
        public virtual int QuestionID
        {
            get { return _questionID; }
            set{_questionID = value;}            
        }

        public virtual string QuestionExplainationText
        {
            get;
            set;
        }
        public virtual string QuestionLabel
        {
            get;
            set;
        }
        public virtual string QuestionText
        {
            get;
            set;
        }

        public virtual AnswerValue[] Values
        {
            get;
            set;
        }
    
        public virtual System.DateTime Date
        {
            get;
            set;
        }
    
        public virtual string OperatorID
        {
            get;
            set;
        }

        public virtual int Page
        { 
            get; 
            set; 
        }

        public virtual int Order
        {
            get;
            set;
        }
        #endregion
    }
}
