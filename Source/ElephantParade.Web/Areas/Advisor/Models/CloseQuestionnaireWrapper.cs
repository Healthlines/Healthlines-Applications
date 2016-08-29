using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NHSD.ElephantParade.Domain.Models;
using NHSD.ElephantParade.Core.Models;

namespace NHSD.ElephantParade.Web.Areas.Advisor.Models
{
    public class CloseQuestionnaireWrapper
    {
        public Questionnaires.Core.Services.Models.AnswerSet AnswerSet { get; set; }
        
        public CallbackViewModel Callback { get; set; }

        public QuestionnaireActions QuestionnaireAction { get; set; }
    }
}