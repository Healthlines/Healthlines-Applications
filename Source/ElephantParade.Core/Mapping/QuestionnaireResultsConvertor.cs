using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web;
using Questionnaires.Core.Services.Models;
using NHSD.ElephantParade.Domain.Models;
using NHSD.ElephantParade.DAL.EntityModels;

namespace NHSD.ElephantParade.Core.Mapping
{
    public class QuestionnaireResultsConvertor
    {
        private DAL.Interfaces.IQuestionnaireActionLetterRepository _questionnaireActionLetterRepository;
        CultureInfo _cinfo = Thread.CurrentThread.CurrentCulture;

        public QuestionnaireResultsConvertor(DAL.Interfaces.IQuestionnaireActionLetterRepository _questionnaireActionLetterRepository)
        {
            // TODO: Complete member initialization
            this._questionnaireActionLetterRepository = _questionnaireActionLetterRepository;
        }

        public QuestionnaireResults Convert(AnswerSet answerSet, IList<AnswerSetAnswer> answers,string StudyID)
        {
            QuestionnaireResults result = new QuestionnaireResults();
            

            result.StudyID = StudyID;
            result.StartDate = answerSet.StartDate;
            result.EndDate = answerSet.EndDate;
            result.Completed = answerSet.Status == AnswerSet.State.Completed;
            result.Operator = answerSet.OperatorID;
            result.Participant = answerSet.ParticipantID;
            result.QuestionnaireTitle = answerSet.QuestionnaireTitle;
            result.QuestionnaireID = answerSet.QuestionnaireID.ToString(_cinfo);
            result.ResultSetID = answerSet.AnswerSetID.ToString(_cinfo);

            List<QuestionnaireResults.Question> questions = new List<QuestionnaireResults.Question>();
            foreach (var item in answers )
            {
                questions.Add(new QuestionnaireResults.Question()
                {
                    QuestionTitle = item.QuestionLabel,
                    QuestionExplaination = item.QuestionExplainationText,
                    QuestionText = item.QuestionText,
                    Date = item.Date,
                    OperatorID = item.OperatorID,
                    Answers = item.Values.Select(v=>v.Value).ToArray()
                });
            }
            result.Questions = questions;
            string resultsSetID = answerSet.AnswerSetID.ToString(_cinfo);
            var q = (from al in _questionnaireActionLetterRepository.GetQueryable()
                     where al.StudyID == StudyID && al.ResultsSetID == resultsSetID //&& al.ProcessedDate == null
                     select al);

            result.Letters = new List<QuestionnaireLetterAction>();
            foreach (var item in q)
            {
                result.Letters.Add(this.ConvertFromLetterAction(item));
            }        

            return result;
        }


        internal QuestionnaireSession Convert(AnswerSet answerSet, string StudyID, string patientID, QuestionnaireSession result = null)
        {
            if(result ==null)
                result = new QuestionnaireResults();

            result.StudyID = StudyID;
            result.StartDate = answerSet.StartDate;
            result.EndDate = answerSet.EndDate;
            result.Completed = answerSet.Status == AnswerSet.State.Completed;
            result.Operator = answerSet.OperatorID;
            result.Participant = patientID;
            result.QuestionnaireTitle = answerSet.QuestionnaireTitle;
            result.QuestionnaireID = answerSet.QuestionnaireID.ToString(_cinfo);
            result.ResultSetID = answerSet.AnswerSetID.ToString(_cinfo);

            string resultsSetID = answerSet.AnswerSetID.ToString(_cinfo);
            var q = (from al in _questionnaireActionLetterRepository.GetQueryable()
                     where al.StudyID == StudyID && al.ResultsSetID == resultsSetID //&& al.ProcessedDate == null
                     select al);

            result.Letters = new List<QuestionnaireLetterAction>();
            foreach (var item in q)
            {
                result.Letters.Add(this.ConvertFromLetterAction(item));
            }            

            return result;
        }



        #region QuestionnaireLetterAction Conversions
        public Domain.Models.QuestionnaireLetterAction ConvertFromLetterAction(QuestionnaireActionLetter letterAction)
        {
            Domain.Models.QuestionnaireLetterAction la = new QuestionnaireLetterAction();
            la.FileID = letterAction.FileID;
            la.ID = letterAction.ID;
            la.LetterTarget = (LetterTarget)Enum.Parse(typeof(LetterTarget), letterAction.Recipient);
            la.LetterTemplate =  (LetterType)Enum.Parse(typeof(LetterType),letterAction.LetterID);
            la.PatientID = letterAction.PatientID;
            la.ProcessedBy = letterAction.ProcessedBy;
            la.ProcessedDate = letterAction.ProcessedDate;
            la.ResultSetID = letterAction.ResultsSetID;
            la.StudyID = letterAction.StudyID;
            return la;
        }

        public QuestionnaireActionLetter ConvertFromLetterAction(Domain.Models.QuestionnaireLetterAction letterAction)
        {
            QuestionnaireActionLetter la = new QuestionnaireActionLetter();
            la.FileID = letterAction.FileID;
            la.ID = letterAction.ID;
            la.Recipient = letterAction.LetterTarget.ToString();
            la.LetterID = letterAction.LetterTemplate.ToString();
            la.PatientID = letterAction.PatientID;
            la.ProcessedBy = letterAction.ProcessedBy;
            la.ProcessedDate = letterAction.ProcessedDate;
            la.ResultsSetID = letterAction.ResultSetID;
            la.StudyID = letterAction.StudyID;
            return la;
        }
        #endregion
    }
}