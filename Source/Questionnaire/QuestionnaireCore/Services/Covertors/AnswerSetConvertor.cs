// -----------------------------------------------------------------------
// <copyright file="AnswerSetConvertor.cs" company="NHS Direct">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Questionnaires.Core.Services.Covertors
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Questionnaires.Core.DataAccess.Interfaces;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class AnswerSetConvertor 
    {
        internal static BusinessObjects.AnswerSet Convert(Models.AnswerSet answerSet)
        {
            if (answerSet == null)
                return null;
            BusinessObjects.AnswerSet result = new BusinessObjects.AnswerSet()
            {
                AnswerSetID = answerSet.AnswerSetID,
                ParticipantID = answerSet.ParticipantID,
                OperatorID = answerSet.OperatorID,
                QuestionnaireID = answerSet.QuestionnaireID,
                Completed = false,
                StartDate = answerSet.StartDate,
                EndDate = answerSet.EndDate                
            };
            return result;
        }

        internal static Models.AnswerSet Convert(BusinessObjects.AnswerSet answerSet)
        {
            return Convert(answerSet, answerSet.Questionnaire.Name);
        }

        internal static Models.AnswerSet Convert(BusinessObjects.AnswerSet answerSet,string _questionnaireTitle)
        {
            if (answerSet == null)
                return null;

            Models.AnswerSet result = GenericConvertor.Convert<Models.AnswerSet>(answerSet);
            //result.QuestionnaireTitle = answerSet.Questionnaire.Name;
            result.QuestionnaireTitle = _questionnaireTitle;
            return result;
        }
        
    }
}
