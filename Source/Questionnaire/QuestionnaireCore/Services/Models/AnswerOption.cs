// -----------------------------------------------------------------------
// <copyright file="AnswerOption.cs" company="NHS Direct">
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
    /// Represents an answer that can be selected for a question 
    /// </summary>
    public class AnswerOption
    {
        public enum OptionType
        {
            radio,
            check,
            checkunique,
            date,
            time,
            datetime,
            textarea,
            textline,
            integer,
            @decimal,
            label,
            weekspicker
        }

        #region Fields

        private int _questionID;
        private Condition _routeCondition;

        #endregion

        #region Properties

        public virtual int AnswerOptionID
        {
            get;
            set;
        }

        public virtual int QuestionID
        {
            get { return _questionID; }
            set {_questionID = value; }
        }
        

        public virtual string Label
        {
            get;
            set;
        }

        public virtual int Order
        {
            get;
            set;
        }

        public virtual OptionType Type
        {
            get;
            set;
        }

        public virtual string DefaultValue
        {
            get;
            set;
        }

        public virtual Nullable<int> RouteQuestionID
        {
            get;
            set;
        }

        public virtual Condition RouteCondition
        {
            get { return _routeCondition; }
            set { _routeCondition = value;}
        }        

        #endregion
    }
}
