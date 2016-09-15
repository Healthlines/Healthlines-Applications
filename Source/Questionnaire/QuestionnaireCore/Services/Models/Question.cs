using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Questionnaires.Core.Services.Models
{
    /// <summary>
    /// Represents a question that can be used in more than one questionnaire
    /// </summary>
    public class Question
    {
        private ICollection<AnswerOption> _answerOptions;

        #region Properties

        public virtual int QuestionID
        {
            get;
            set;
        }

        public virtual string Explanation
        {
            get;
            set;
        }

        public virtual string QuestionText
        {
            get;
            set;
        }

        public virtual string QuestionGroup
        {
            get;
            set;
        }

        public virtual bool Required
        {
            get;
            set;
        }

        public virtual string Label
        { 
            get; 
            set; 
        }

        public virtual ICollection<AnswerOption> AnswerOptions
        {
            get
            {
                if (_answerOptions == null)
                    _answerOptions = new List<AnswerOption>();
                return _answerOptions;
            }
            set
            {                
                _answerOptions = value;
            }
        }
        
        #endregion
    }
}
