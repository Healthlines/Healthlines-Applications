// -----------------------------------------------------------------------
// <copyright file="QuestionnaireSession.cs" company="NHS Direct">
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
    public class QuestionnaireSession
    {
        public enum State
	    {
            Open,
            Completed,
	        Closed 
	    }

        public string StudyID
        {
            get;
            set;
        }

        public string ResultSetID
        {
            get;
            set;
        }

        public virtual string QuestionnaireID
        {
            get;
            set;
        }
        public virtual string QuestionnaireTitle
        {
            get;
            set;
        }

        public virtual string Operator
        {
            get;
            set;
        }

        public virtual string Participant
        {
            get;
            set;
        }

        public virtual System.DateTime StartDate
        {
            get;
            set;
        }

        public virtual bool Completed
        { 
            get; 
            set; 
        }

        public State Status
        {
            get
            {
                if (Completed)
                    return State.Completed;
                else if (this.EndDate == null)
                    return State.Open;
                else
                    return State.Closed;
            }
        }

        public virtual Nullable<System.DateTime> EndDate
        {
            get;
            set;
        }
        
        /// <summary>
        /// list of required and processed letters for this questionnaire session
        /// </summary>
        public virtual IList<QuestionnaireLetterAction> Letters
        { get; set; }
    }
}
