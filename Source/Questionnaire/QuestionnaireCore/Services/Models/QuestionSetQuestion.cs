using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Questionnaires.Core.Services.Models
{
    /// <summary>
    /// Represents a question that is assigned to a question set
    /// </summary>
    public class QuestionSetQuestion
    {
        #region Fields

        private int _questionID;
        private int _questionSetID;

        #endregion

        #region Properties

        public virtual int? QuestionSetQuestionID
        {
            get;
            set;
        }

        public virtual int QuestionSetID
        {
            get { return _questionSetID; }
            set {_questionSetID = value;}            
        }
        

        public virtual int QuestionID
        {
            get { return _questionID; }
            set {_questionID = value;}            
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
