// -----------------------------------------------------------------------
// <copyright file="DepressionService.cs" company="NHS Direct">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

using System.Globalization;

namespace NHSD.ElephantParade.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Interfaces;
    using DAL.Interfaces;
    using Mapping;
    using Questionnaires.Core.Services;
    using Domain.Models;
    using DAL;
    using DAL.EntityModels;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class DepressionService
        :PatientService, IDepressionService
    {
        public const string DEPRESSION_STUDYID = "Depression";
        private IQuestionnaireService _questionnaireService;
        private IQuestionnaireActionLetterRepository _questionnaireActionLetterRepository;

        public override string StudyID
        {
            get
            {
                return DEPRESSION_STUDYID;//base.StudyID;
            }
            set
            {
                throw new InvalidOperationException();
                //base.StudyID = value;
            }
        }

        public DepressionService(IQuestionnaireService questionnaireService)
            : this(questionnaireService,null)
        {
            
        }
        public DepressionService(IQuestionnaireService questionnaireService, IQuestionnaireActionLetterRepository questionnaireActionLetterRepository)
            : base()
        {
            _questionnaireService = questionnaireService;
            _questionnaireActionLetterRepository = questionnaireActionLetterRepository??new QuestionnaireActionLetterRepository();
        }

        public DepressionService(string depressionStudyId, 
            IPatientRepository patientRepository,
            IPatientMedicationRepository patientMedicationRepository,
            IGpPracticeAddressRepository gpPracticeAddressRepository,
            IQuestionnaireService questionnaireService
            )
            :base(patientRepository,patientMedicationRepository,gpPracticeAddressRepository)
        {
            _questionnaireService = questionnaireService;
        }

        #region IQuestionnaireResults
        public string QuestionnaireParticipantIDGet(string patientId)
        {
            return string.Format("{0}:{1}", StudyID, patientId);
        }

        public string QuestionnairePatientIDGet(string participantId)
        {
            var prams = participantId.Split(new[] { ":" }, StringSplitOptions.None);
            return prams.Count() == 2 ? prams[1] : null;
        }

        /// <summary>
        /// returns a list of questionnaire sessions for a patient
        /// </summary>
        /// <param name="patientId"></param>
        /// <returns></returns>
        public IList<QuestionnaireSession> QuestionnaireSessionList(string patientId)
        {
            var answerSets = _questionnaireService.AnswerSetList(QuestionnaireParticipantIDGet(patientId));

            var handler = new EncounterHandlers.EncounterHandler(_questionnaireService);
            foreach (var item in answerSets)
            {
                if (item.Status == Questionnaires.Core.Services.Models.AnswerSet.State.Completed)
                {
                    //ensure the questionnarie has been processed for actions
                    ProcessQuestionnaireActionLetters(item, handler.ProcessLetters(item));
                }
            }

            QuestionnaireResultsConvertor convertor = new QuestionnaireResultsConvertor(_questionnaireActionLetterRepository);            
            var results = answerSets.Select(a => convertor.Convert(a, DEPRESSION_STUDYID, patientId)).ToList();
            return results;
        }

        
        /// <summary>
        /// returns a questionnaire session with the questions and selected answers 
        /// </summary>
        /// <param name="resultSetId"></param>
        /// <returns></returns>
        public QuestionnaireResults QuestionnaireResultGet(string resultSetId)
        {
            
            QuestionnaireResultsConvertor convertor = new QuestionnaireResultsConvertor(_questionnaireActionLetterRepository);
            var results = convertor.Convert(_questionnaireService.AnswerSetGet(int.Parse(resultSetId)), _questionnaireService.AnswerList(int.Parse(resultSetId)), DEPRESSION_STUDYID);
            return results;
        }

        /// <summary>
        /// returns a list of letter actions and data needed to generate the letters
        /// </summary>
        /// <param name="resultSetId"></param>
        /// <returns></returns>
        public QuestionnaireActions QuestionnaireActionsGet(string resultSetId)
        {
            var answerSet = _questionnaireService.AnswerSetGet(int.Parse(resultSetId));                 
            var handler = new EncounterHandlers.EncounterHandler(_questionnaireService);
            Domain.Models.QuestionnaireActions result = new QuestionnaireActions();

            //get actions data to be made available to the letter generator service 
            result.LetterActionData = handler.ProcessLetters(answerSet);

            //ensure the questionnaires required actions letters are saved so we know when required letters have not been processed
            ProcessQuestionnaireActionLetters(answerSet, result.LetterActionData.ToList());

            //load the QuestionnaireActions objects QuestionnaireSession data
            QuestionnaireResultsConvertor convertor = new QuestionnaireResultsConvertor(_questionnaireActionLetterRepository);            
            convertor.Convert(answerSet, DEPRESSION_STUDYID, QuestionnairePatientIDGet(answerSet.ParticipantID),result);
            
            return result;
        }

        public IList<QuestionnaireLetterAction> QuestionnaireActionsGetByPatient(string patientId)
        {
            IList<QuestionnaireLetterAction> result = new List<QuestionnaireLetterAction>();
            var letterActions = _questionnaireActionLetterRepository.GetQueryable().Where(l => l.StudyID == StudyID && l.PatientID == patientId).ToList();
            
            QuestionnaireResultsConvertor convertor = new QuestionnaireResultsConvertor(_questionnaireActionLetterRepository);     
            //load the AnswerSet Data with the required letter info
            foreach (var item in letterActions)
            {
                result.Add(convertor.ConvertFromLetterAction(item));
            }

            return result;
        }

        public void QuestionnaireActionLetterProcessed(string studyId, string resultsSetId, string letterId, string recipient, string processedBy, int? fileId)
        {
            var letterActions = _questionnaireActionLetterRepository.GetQueryable().Where(l => l.StudyID == studyId && l.ResultsSetID == resultsSetId && l.LetterID == letterId && l.ProcessedDate == null && l.Recipient == recipient).ToList();
            
            var letterAction = letterActions.OrderByDescending(l => l.ID).FirstOrDefault();
            if (letterAction == null)
            {
                var answerSet = QuestionnaireResultGet(resultsSetId);
                var patientId = QuestionnairePatientIDGet(answerSet.Participant);
                letterAction = new QuestionnaireActionLetter { StudyID = studyId, ResultsSetID = resultsSetId, LetterID = letterId, Recipient = recipient, PatientID = patientId };
                _questionnaireActionLetterRepository.Add(letterAction);
            }

            letterAction.ProcessedDate = DateTime.Now;
            letterAction.ProcessedBy = processedBy;
            letterAction.FileID = fileId;
            _questionnaireActionLetterRepository.SaveChanges();
        }
        
        public void QuestionnaireActionLetterProcessed(int letterActionId,string processedBy,int? fileId)
        {
            var letterAction = _questionnaireActionLetterRepository.GetQueryable().Where(l => l.ID == letterActionId).FirstOrDefault();
            if (letterAction == null)
                throw new ArgumentException("Invalid letterActionID" + letterActionId.ToString(CultureInfo.InvariantCulture));

            letterAction.ProcessedDate = DateTime.Now;
            letterAction.ProcessedBy = processedBy;
            letterAction.FileID = fileId;
            _questionnaireActionLetterRepository.SaveChanges();
        }

        //public void QuestionnaireActionsRefresh(string resultSetID)
        //{
        //    var answerSet = _questionnaireService.AnswerSetGet(int.Parse(resultSetID));
        //    //ensure the questionnaire has been processed for actions
        //    ProcessQuestionnaireActions(answerSet);
        //}

        /// <summary>
        /// ensure a answer set has been processed so required letters are saved as required
        /// </summary>
        /// <param name="answerSet"></param>
        private void ProcessQuestionnaireActionLetters(Questionnaires.Core.Services.Models.AnswerSet answerSet, IList<ElephantParade.Domain.Models.ModuleLetter> letterActions)
        {
            var patientId = QuestionnairePatientIDGet(answerSet.ParticipantID);
            var strAnswerSetId = answerSet.AnswerSetID.ToString(CultureInfo.InvariantCulture);
            
            // Clear unprocessed letters for this answerset, any required letters will be readded but letters no longer needed (user changes answers to questions)
            //---------------------
            //find all unprocessed required letters for this answerset 
            var patientLetters = _questionnaireActionLetterRepository.GetQueryable().Where(l => l.PatientID == patientId && l.ResultsSetID == strAnswerSetId && l.ProcessedDate == null).ToList();
            //delete them
            foreach (var item in patientLetters)
            {
                _questionnaireActionLetterRepository.Delete(item);
            }
            _questionnaireActionLetterRepository.SaveChanges();
          
            //process Encounter
            //---------------------            
            
            //get all patient letters requested and sent
            patientLetters = _questionnaireActionLetterRepository.GetQueryable().Where(l => l.PatientID == patientId).ToList();

            foreach (var item in letterActions)
	        {
                //if letter required and not already requested or sent then request it
                var lname = item.LetterActionData.LetterTemplate.ToString();
                if (item.Required && patientLetters.Where(l=>l.LetterID == lname).Count()==0)
                {
                    QuestionnaireActionLetter aletter = new QuestionnaireActionLetter
                                                            {
                        LetterID = item.LetterActionData.LetterTemplate.ToString(),
                        Recipient = item.LetterActionData.LetterTarget.ToString(),
                        ResultsSetID = answerSet.AnswerSetID.ToString(CultureInfo.InvariantCulture),
                        StudyID = StudyID ,
                        PatientID = patientId
                    };
                    _questionnaireActionLetterRepository.Add(aletter);
                }
	        }
            _questionnaireActionLetterRepository.SaveChanges();
        }

       
        
        #endregion















        
    }
}
