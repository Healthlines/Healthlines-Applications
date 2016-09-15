// -----------------------------------------------------------------------
// <copyright file="AnswerConvertor.cs" company="NHS Direct">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Questionnaires.Core.Services.Covertors
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class AnswerConvertor
    {

        

        internal static Models.AnswerSetAnswer Convert(List<BusinessObjects.Answer> list)
        {
            if (list == null && list.Count>0)
                return null;
                      
            if (list.GroupBy(a => a.QuestionID).Count() > 1)
                throw new InvalidOperationException("Cannot convert answer list to AnswerSetAnswer. non-distinct questionID");
            

            BusinessObjects.Answer answer = list[0];

            Models.AnswerSetAnswer answerSetAnswer = new Models.AnswerSetAnswer();
            
            answerSetAnswer.AnswerSetID = answer.AnswerSetID;
            answerSetAnswer.QuestionID = answer.QuestionID;
            answerSetAnswer.QuestionLabel = answer.Question.Label;
            answerSetAnswer.QuestionText = answer.Question.QuestionText;
            answerSetAnswer.QuestionExplainationText = answer.Question.Explanation;
            answerSetAnswer.Date = answer.Date;
            answerSetAnswer.OperatorID = answer.OperatorID;
            answerSetAnswer.Page = answer.Page;
            answerSetAnswer.Order = answer.Order;
            answerSetAnswer.Values = list.Select(a => new Models.AnswerSetAnswer.AnswerValue()
            {
                Type = (Models.AnswerOption.OptionType)Enum.Parse(typeof(Models.AnswerOption.OptionType), a.Type),
                Value = a.Value  
            }).ToArray();
            
            return answerSetAnswer;
            throw new NotImplementedException();
        }
    }
}
