// -----------------------------------------------------------------------
// <copyright file="QuestionConvertor.cs" company="NHS Direct">
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
    public class QuestionConvertor :IConvertor<BusinessObjects.Question, Models.Question>
    {

        public Models.Question Convert(BusinessObjects.Question source)
        {
            Models.Question question = new Models.Question();
            this.Fill(source,  question);
            return question;
        }

        public BusinessObjects.Question Convert(Models.Question source)
        {
            BusinessObjects.Question question = new BusinessObjects.Question();
            this.Fill(source, question);
            return question;
        }

        public void Fill(Models.Question source,  BusinessObjects.Question target)
        {            
            target.QuestionID = source.QuestionID;
            target.QuestionGroup = source.QuestionGroup;
            target.Explanation = source.Explanation;
            target.QuestionText = source.QuestionText;
            target.Required = source.Required;
            target.Label = source.Label;

            //target.AnswerOptions.Clear();
            var items = target.AnswerOptions.ToArray();
            foreach (var item in items)
            {
                target.AnswerOptions.Remove(item);
            }

            foreach (var item in source.AnswerOptions)
            {
                //question.AnswerOptions.Add(GenericConvertor.Convert<Models.AnswerOption, BusinessObjects.AnswerOption>(item));
                BusinessObjects.AnswerOption ao = new BusinessObjects.AnswerOption()
                {
                    QuestionID = item.QuestionID,
                    AnswerOptionID = item.AnswerOptionID,
                    DefaultValue = item.DefaultValue,
                    Label = item.Label,
                    Order = item.Order,                    
                    //Required = item.Required,
                    //RouteConditionID = item.RouteConditionID,                    
                    RouteQuestionID = item.RouteQuestionID,
                    Type = item.Type.ToString()
                };
                if (item.RouteCondition != null)
                {
                    ao.Condition = new BusinessObjects.Condition()
                    {
                        Label = item.RouteCondition.Label,
                        OperatorType = item.RouteCondition.Operator.ToString(),
                        ValueType = item.RouteCondition.ValueType.ToString(),
                        Value1 = item.RouteCondition.Value1,
                        Value2 = item.RouteCondition.Value2
                    };
                }
                target.AnswerOptions.Add(ao);
            }            
        }

        public void Fill(BusinessObjects.Question source,  Models.Question target)
        {
            target.QuestionID = source.QuestionID;
            target.QuestionText = source.QuestionText;
            target.Explanation = source.Explanation;
            target.Required = source.Required;
            target.QuestionGroup = source.QuestionGroup;
            target.Label = source.Label;

            if (target.AnswerOptions == null && source.AnswerOptions != null && source.AnswerOptions.Count > 0)
                target.AnswerOptions = new List<Models.AnswerOption>();
            if (target.AnswerOptions != null)
                target.AnswerOptions.Clear();
            foreach (var item in source.AnswerOptions)
            {
                Models.AnswerOption ao = new Models.AnswerOption()
                {
                    QuestionID = item.QuestionID,
                    AnswerOptionID = item.AnswerOptionID,
                    DefaultValue = item.DefaultValue,
                    Label = item.Label,
                    Order = item.Order,                    
                    //Required = item.Required,
                    //RouteConditionID = item.RouteConditionID,                    
                    RouteQuestionID = item.RouteQuestionID,
                    Type = (Models.AnswerOption.OptionType)Enum.Parse(typeof(Models.AnswerOption.OptionType), item.Type),
                };
                if (item.Condition != null)
                {
                    ao.RouteCondition = new Models.Condition()
                    {
                        Label = item.Condition.Label,
                        Operator = (Models.Condition.OperatorType)Enum.Parse(typeof(Models.Condition.OperatorType), item.Condition.OperatorType),
                        ValueType = (Models.Condition.Value_Type)Enum.Parse(typeof(Models.Condition.Value_Type), item.Condition.ValueType),
                        Value1 = item.Condition.Value1,                        
                        Value2 = item.Condition.Value2
                    };
                }
                target.AnswerOptions.Add(ao);
            }
            target.AnswerOptions = target.AnswerOptions.OrderBy(ao => ao.Order).ToList();
        }
    }
}
