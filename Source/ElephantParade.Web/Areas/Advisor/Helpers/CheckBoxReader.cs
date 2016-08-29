using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Questionnaires.Core.Services.Models;

namespace Questionnaires.Web.Helpers
{
    class CheckBoxReader:IValueReader
    {
        public IEnumerable<Answer> Read(Core.Services.Models.QuestionSetPageItem question, string key, IDictionary<string, string> f)
        {
            List<Answer> answers = new List<Answer>();
            
            string[] vals = f[key].Split(new string[] { "," }, StringSplitOptions.None);

            foreach (var item in vals)
            {
                string[] fields = item.Split(new string[] { QuestionReader.Delimiter }, StringSplitOptions.None);
                Answer answer = new Answer();
                answer.QuestionID = question.QuestionID;
                answer.Type = AnswerOption.OptionType.check;

                answer.Value = fields[0];
                answer.AnswerOptionID = int.Parse(fields[1]);
                answers.Add(answer);
            }            
            return answers;
        }
    }
}
