using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using Questionnaires.Core.Services.Models;
using System.Text;

namespace Questionnaires.Web.Helpers
{
    /// <summary>
    /// This class is tightly coupled to the partial views for answeroptions as it seeks the control ids
    /// </summary>
    public class QuestionReader
    {
        public const string Delimiter = ":";
        public const string Prefix = "qid";

        private static readonly IDictionary<AnswerOption.OptionType, IValueReader> Implementations = new Dictionary<AnswerOption.OptionType, IValueReader> 
                                                                                        { 
                                                                                            { AnswerOption.OptionType.radio, new RadioReader() }, 
                                                                                            { AnswerOption.OptionType.check, new CheckBoxReader()},
                                                                                            {AnswerOption.OptionType.checkunique, new CheckBoxReader()}
                                                                                        };

        public static IList<Answer> Read(IEnumerable<QuestionSetPageItem> questions, IDictionary<string, string> formCollection, string operatorID)
        {
           List<Answer> answers = new List<Answer>();

            foreach (var item in questions)
                answers.AddRange(ReadQuestion(item, formCollection));

            foreach (var answer in answers)
                answer.OperatorID = operatorID;
  
            return answers; 
        }

        public static IEnumerable< Answer> ReadQuestion(QuestionSetPageItem question, IDictionary<string, string> formCollection)
        {
            var keys  = (from k in formCollection.Keys
                         where k.Contains(string.Format("{0}{1}{2}{3}",Prefix , Delimiter , question.QuestionID.ToString(CultureInfo.InvariantCulture) , Delimiter)) 
                          select k);

            if (keys.Count() == 0 && question.Question.Required)
                throw new KeyNotFoundException(string.Format("Missing answer to question {0}", question.QuestionID));

            List<Answer> answerList = new List<Answer>();
            //ToDo: validate we dont have duplicates for types other than OptionType.check
            
            foreach (var key in keys)
            {   
                //get the answer type
                string[] vals = key.Split(new string[] { Delimiter },StringSplitOptions.None);
                AnswerOption.OptionType answerType;
                if(!Enum.TryParse(vals[2], out answerType))
                    throw new KeyNotFoundException(string.Format("Missing answerOptionType to question {0}", question.QuestionID));

                //read the answer
                answerList.AddRange(GetValueReader(answerType).Read(question, key, formCollection));                            
            }

            return answerList;
        }
                

        public static IValueReader GetValueReader(AnswerOption.OptionType type)
        {
              if(Implementations.ContainsKey(type))
              {
                  return Implementations[type];
              }
              else
              {
                  return new DefaultReader(type);
              }            
        }

        /// <summary>
        /// Creates a name attribute for AnswerOption items
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static string CreateName(AnswerOption model)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(Prefix);
            sb.Append(Delimiter);
            sb.Append(model.QuestionID.ToString(CultureInfo.InvariantCulture));
            sb.Append(Delimiter);
            sb.Append(model.Type.ToString());
            return sb.ToString();
        }

        public static string CreateName(QuestionSetPageItem model)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(Prefix);
            sb.Append(Delimiter);
            sb.Append(model.QuestionID.ToString(CultureInfo.InvariantCulture));
            return sb.ToString();
        }
    }
}