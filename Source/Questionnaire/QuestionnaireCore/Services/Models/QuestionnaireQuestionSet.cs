// -----------------------------------------------------------------------
// <copyright file="QuestionnaireQuestionSet.cs" company="NHS Direct">
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
    /// TODO: Update summary.
    /// </summary>
    public class QuestionnaireQuestionSet
    {
        #region Primitive Properties

        public virtual int QuestionnaireQuestionSetsID
        {
            get;
            set;
        }

        public virtual int Order
        {
            get;
            set;
        }

        public virtual int QuestionnaireID
        {
            get;
            set;
        }


        public virtual int QuestionSetID
        {
            get;
            set;
        }


        public string QuestionSetName
        { get; set; }


        #endregion
    }
}
