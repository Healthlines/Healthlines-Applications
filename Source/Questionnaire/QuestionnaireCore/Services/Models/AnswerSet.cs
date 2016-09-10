// -----------------------------------------------------------------------
// <copyright file="AnswerSet.cs" company="NHS Direct">
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
    /// Represents a collection of answers to a Question Set
    /// </summary>
    public class AnswerSet
    {
        public enum State
	    {
            Open,
            Completed,
	        Closed 
	    }

        #region Fields

        private int _answerSetID;
        private Nullable<int> _currentQuestionID;

        #endregion

        #region Properties

        public virtual int AnswerSetID
        {
            get { return _answerSetID; }
            set {_answerSetID = value;}
        }

        public virtual string QuestionnaireTitle
        { get; set; }

        public virtual int QuestionnaireID
        {
            get;
            set;
        }
                
        public virtual Nullable<int> CurrentQuestionID
        {
            get { return _currentQuestionID; }
            set { _currentQuestionID = value; }
        }
        
        public virtual string OperatorID
        {
            get;
            set;
        }

        public virtual string ParticipantID
        {
            get;
            set;
        }

        internal virtual bool Completed
        {
            get;
            set;
        }
        public virtual System.DateTime StartDate
        {
            get;
            set;
        }

        public virtual Nullable<System.DateTime> EndDate
        {
            get;
            set;
        }
        public virtual State Status
        {
            get
            {
                if (!Completed && EndDate != null)
                    return State.Closed;
                return Completed ? State.Completed : State.Open; 
            }            
        }
        
        #endregion
    }
}
