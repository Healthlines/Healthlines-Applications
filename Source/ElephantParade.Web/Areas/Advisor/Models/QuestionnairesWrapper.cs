using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Questionnaires.Core.Services.Models;
using NHSD.ElephantParade.Domain.Models;

namespace Questionnaires.Web.Models
{
    /// <summary>
    /// this is used to display questionnaire models for a patient
    /// </summary>
    public class QuestionnaireSetsWrapper
    {
        public IList<Questionnaires.Core.Services.Models.Questionnaire> Questionnaires { get; set; }
        //public IList<NHSD.ElephantParade.Domain.Models.QuestionnaireLetterAction> PatientLetterActions { get; set; }
        public IList<QuestionnaireSession> QuestionnaireSessions { get; set; }
        public string ParticipantID { get; set; }
        public string StudyID { get; set; }
        public string PatientID { get; set; }
    }
}