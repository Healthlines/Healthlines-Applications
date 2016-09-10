using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Questionnaires.Core.Services.Models
{
    public class Questionnaire
    {
        public int QuestionnaireID
        {
            get;
            set;
        }

        public string Name
        {
            get;
            set;
        }

        public DateTime CreationDate
        {
            get; set; 
        }

        public string CreatedBy
        {
            get;
            set;
        }


        public ICollection<QuestionnaireQuestionSet> QuestionSets { get; set; }
    }
}
