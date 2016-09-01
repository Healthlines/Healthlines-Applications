using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Questionnaires.Core.Services.Models;

namespace Questionnaires.Web.Helpers
{
    class RadioReader :IValueReader 
    {
        public IEnumerable<Core.Services.Models.Answer> Read(Core.Services.Models.QuestionSetPageItem question,string key, IDictionary<string, string> f)
        {
            Answer answer = new Answer();
            answer.QuestionID = question.QuestionID;
            answer.Type = AnswerOption.OptionType.radio;
            string[] vals = f[key].Split(new string[]{QuestionReader.Delimiter},StringSplitOptions.None);
            answer.Value = vals[0];
            answer.AnswerOptionID = int.Parse(vals[1]);
            List<Answer> answers = new List<Answer>();
            answers.Add(answer);
            return answers;
        }
    }
}
