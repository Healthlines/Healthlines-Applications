// -----------------------------------------------------------------------
// <copyright file="QuestionSet.cs" company="NHS Direct">
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
    /// Represents a set of questions 
    /// </summary>
    public class QuestionSet
    {

        #region Properties
        public virtual int QuestionSetID
        {
            get;
            set;
        }

        public virtual string Name
        {
            get;
            set;
        }

        public virtual string CreatedBy
        {
            get;
            set;
        }

        public virtual System.DateTime CreatedDate
        {
            get;
            set;
        }

        public ICollection<QuestionSetQuestion> QuestionSetQuestions
        {
            get;
            set;
        }
        #endregion
    }
}
