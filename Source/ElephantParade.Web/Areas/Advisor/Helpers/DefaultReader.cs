using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Questionnaires.Core.Services.Models;

namespace Questionnaires.Web.Helpers
{
    public class DefaultReader : IValueReader 
    {
        private AnswerOption.OptionType _answerOptionType;

        public DefaultReader(AnswerOption.OptionType type)
        {
            _answerOptionType = type;
        }

        public IEnumerable< Core.Services.Models.Answer> Read(Core.Services.Models.QuestionSetPageItem question, string key, IDictionary<string, string> f)
        {            
            Answer answer = new Answer();
            answer.QuestionID = question.QuestionID;
            answer.Type = _answerOptionType;
            answer.Value = f[key];
            var fields = key.Split(new string[] { QuestionReader.Delimiter }, StringSplitOptions.None);
            answer.AnswerOptionID = int.Parse(fields[3]);
            List<Answer> answers =  new List<Answer>();
            answers.Add(answer); 
            return answers;
        }
    }
}