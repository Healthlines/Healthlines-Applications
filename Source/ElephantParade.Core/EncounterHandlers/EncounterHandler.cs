// -----------------------------------------------------------------------
// <copyright file="Encounter1.cs" company="NHS Direct">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace NHSD.ElephantParade.Core.EncounterHandlers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Questionnaires.Core.Services;
    using NHSD.ElephantParade.Domain.Models;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class EncounterHandler        
    {
        protected IQuestionnaireService _questionnaireService;

        public EncounterHandler(IQuestionnaireService questionnaireService)
        {
            _questionnaireService = questionnaireService;
        }


        internal IList<ElephantParade.Domain.Models.ModuleLetter> ProcessLetters(Questionnaires.Core.Services.Models.AnswerSet answerSet)
        {            
            //get the modules that are part of questionnaire
            var questionnaire = _questionnaireService.QuestionnaireList().Where(q => q.QuestionnaireID == answerSet.QuestionnaireID).FirstOrDefault();

            List<ElephantParade.Domain.Models.ModuleLetter> results = new List<Domain.Models.ModuleLetter>();
            QuestionSetHandler moduleHandler = new QuestionSetHandler(_questionnaireService);
            
            var answers = _questionnaireService.AnswerList(answerSet.AnswerSetID);
            //process each module
            foreach (var item in questionnaire.QuestionSets)
	        {
                results.AddRange(moduleHandler.Process(item, answerSet, answers));
	        }

            //remove duplicates
            results = results.Distinct(new DistinctLetterComparer()).ToList();

            //ensure generic letter available
            if (results.Where(l => l.LetterActionData.LetterTemplate == Domain.Models.LetterType.DepressionGpGeneric).Count() == 0)
            {
                var letter = new ModuleLetter();
                letter.LetterActionData = new LetterActionData()
                {
                    LetterTarget = Domain.Models.LetterTarget.GP,
                    LetterTemplate = Domain.Models.LetterType.DepressionGpGeneric,
                    LetterValues = new Dictionary<string, object>()
                };
                results.Add(letter);
            }
            //ensure patient email available for all encounters
            if (results.Where(l => l.LetterActionData.LetterTemplate == Domain.Models.LetterType.DepressionPatientEmail).Count() == 0)
            {
                var letter = new ModuleLetter();
                letter.LetterActionData = new LetterActionData()
                {
                    LetterTarget = Domain.Models.LetterTarget.Patient,
                    LetterTemplate = Domain.Models.LetterType.DepressionPatientEmail,
                    LetterValues = new Dictionary<string, object>()
                };
                results.Add(letter);
            }
            
            return results;
        }

        class DistinctLetterComparer : IEqualityComparer<ModuleLetter>
        {
            public bool Equals(ModuleLetter x, ModuleLetter y)
            {
                return x.LetterActionData.LetterTemplate == y.LetterActionData.LetterTemplate &&
                    x.Required == y.Required;
            }

            public int GetHashCode(ModuleLetter obj)
            {
                return obj.LetterActionData.LetterTemplate.GetHashCode() ^
                    obj.Required.GetHashCode();
            }
        }

    }
}
