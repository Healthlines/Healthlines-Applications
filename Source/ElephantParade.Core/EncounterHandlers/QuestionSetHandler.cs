// -----------------------------------------------------------------------
// <copyright file="QuestionSetHandler.cs" company="NHS Direct">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace NHSD.ElephantParade.Core.EncounterHandlers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using NHSD.ElephantParade.Domain.Models;
    using Questionnaires.Core.Services.Models;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class QuestionSetHandler
    {
        private Questionnaires.Core.Services.IQuestionnaireService _questionnaireService;

        public QuestionSetHandler(Questionnaires.Core.Services.IQuestionnaireService _questionnaireService)
        {            
            this._questionnaireService = _questionnaireService;
        }


        internal IEnumerable<ModuleLetter> Process(Questionnaires.Core.Services.Models.QuestionnaireQuestionSet item, Questionnaires.Core.Services.Models.AnswerSet answerSet,IList<AnswerSetAnswer> answers)
        {
            List<ModuleLetter> moduleLetters = new List<ModuleLetter>();
            
            switch (item.QuestionSetID)
            {
                case 9://Opening – initial patient contact
                    moduleLetters = ProcessOpeningInitial(answers, answerSet);
                    break;
                case 25://NHS Direct Symptomatic Assessment module
                    moduleLetters = ProcessSymptomaticAssessment(answers, answerSet);
                    break;
                case 39://Meds review Part 2 (Encs 2-10) module
                    moduleLetters = ProcessMedicinesReviewP2(answers, answerSet);
                    break;
                case 43://Encounter 3 module Part 2
                    moduleLetters = ProcessEncounter3Mod(answers, answerSet);
                    break;
                case 13://Encounter 4 module Part 1
                    
                    break;
                case 29://Encounter 4 module Part 2

                    break;
                case 31://Encounter 4 module Part 3

                    break;
                case 46://Encounter 4 module Part 4

                    break;
                case 32://Encounter 4 module Part 5
                    moduleLetters = ProcessEncounter4Mod(answers, answerSet);
                    break; 
                case 48://Encounter 8 module Part 2
                    moduleLetters = ProcessEncounter8Mod(answers, answerSet);
                    break;
                case 28://Encounter 9 module Part 1
                    moduleLetters = ProcessEncounter9Mod(answers, answerSet);
                    break;
                case 57://Encounter 10 module Part 2
                    moduleLetters = ProcessEncounter10Mod(answers, answerSet);
                    break;
                case 16://Closing script module
                    moduleLetters = ProcessClosingScript(answers, answerSet);
                    break;
                case 14://PHQ9 process module
                    moduleLetters = ProcessPhq9PprocessMod(answers, answerSet);
                    break;
                case 61://Non-scheduled calls module Part 1
                    moduleLetters = ProcessNonScheduledMod(answers, answerSet);
                    break;
                default:
                    break;
            }
               
            return moduleLetters;
        }

               
                
        #region Modules 

        /// <summary>
        /// Encounter 3 module Part 2
        /// </summary>
        /// <param name="answers"></param>
        /// <param name="answerSet"></param>
        /// <returns></returns>
        private List<ModuleLetter> ProcessEncounter3Mod(IList<AnswerSetAnswer> answers, AnswerSet answerSet)
        {
            List<ModuleLetter> letters = new List<ModuleLetter>();
            //if (answerSet.QuestionnaireTitle.ToLower().Contains("encounter 3"))
            if (answerSet.QuestionnaireID == 8)
            {
                var letter = new ModuleLetter();
                letter.LetterActionData = new LetterActionData()
                {
                    LetterTarget = Domain.Models.LetterTarget.Patient,
                    LetterTemplate = Domain.Models.LetterType.DepressionPatientEmail,
                    LetterValues = new Dictionary<string, object>()
                };
                //is letter required
                var triggerAnswer = answers.Where(a => a.QuestionLabel == "3.3.2").Select(a => a).ToList().FirstOrDefault();
                if (triggerAnswer != null)
                {
                    letter.Required = true;
                    int i = 0;
                    foreach (var item in triggerAnswer.Values)
                    {
                        //add the selected option to the letter data so it can use used by the consuming service as a prompt
                        letter.LetterActionData.LetterValues.Add("Action " + i + " for 3.3.2", item.Value);
                    }
                }

                letters.Add(letter);
            }

            return letters;
        }

        /// <summary>
        /// Possible Letters:
        ///     - GpInitialAssessment: Conditions - only for encounter 1,   Question 53.9
        ///                                                                     -Answer value: Send ODLM book (if requested book based CBT)
        ///                                                                     -Answer value: Send LLTTFi workbooks (if patient unable to print)
        ///                                                                     -Answer value: Send patient’s user name and password for LLTTFi via email 
        ///     -
        /// </summary>
        /// <param name="answers"></param>
        /// <returns></returns>
        private List<ModuleLetter> ProcessOpeningInitial(IList<Questionnaires.Core.Services.Models.AnswerSetAnswer> answers, Questionnaires.Core.Services.Models.AnswerSet answerSet)
        {
            List<ModuleLetter> letters = new List<ModuleLetter>();

            //GpInitialAssessment Letter - only for encounter 1
            //--------------------------
            //if (answerSet.QuestionnaireTitle.ToLower().Contains("encounter 1"))
            if (answerSet.QuestionnaireID == 7)
            {
                ModuleLetter letter = new ModuleLetter();

                letter.LetterActionData = new LetterActionData()
                {
                    LetterTarget = Domain.Models.LetterTarget.GP,
                    LetterTemplate = Domain.Models.LetterType.DepressionGpInterventionStart,
                    LetterValues = new Dictionary<string, object>()
                };
                letter.Required = true;
                letters.Add(letter);

               

                letter = new ModuleLetter();
                letter.LetterActionData = new LetterActionData()
                {
                    LetterTarget = Domain.Models.LetterTarget.Patient,
                    LetterTemplate = Domain.Models.LetterType.DepressionPatientEmail,
                    LetterValues = new Dictionary<string, object>()
                };
                //is letter required
                var triggerAnswer = answers.Where(a => a.QuestionLabel == "53.9").Select(a => a).ToList().FirstOrDefault();
                if (triggerAnswer!=null)
                {
                    letter.Required = true;
                    int i = 0;
                    foreach (var item in triggerAnswer.Values)
	                {
                        i++;
                        //add the selected option to the letter data so it can use used by the consuming service as a prompt
                        letter.LetterActionData.LetterValues.Add("Action " + i + " for 53.9", item.Value);
	                }                    
                }
                
                letters.Add(letter);
            }

            return letters;
        }

        /// <summary>
        /// not currently needed
        /// </summary>
        /// <param name="answers"></param>
        /// <param name="answerSet"></param>
        /// <returns></returns>
        private List<ModuleLetter> ProcessClosingScript(IList<Questionnaires.Core.Services.Models.AnswerSetAnswer> answers, Questionnaires.Core.Services.Models.AnswerSet answerSet)
        {
            List<ModuleLetter> letters = new List<ModuleLetter>();
            
            return letters;
        }

        /// <summary>
        /// id: 25 "NHS Direct symptomatic assessment":
        /// </summary>
        /// <param name="answers"></param>
        /// <param name="answerSet"></param>
        /// <returns></returns>
        private List<ModuleLetter> ProcessSymptomaticAssessment(IList<Questionnaires.Core.Services.Models.AnswerSetAnswer> answers, Questionnaires.Core.Services.Models.AnswerSet answerSet)
        {
            List<ModuleLetter> letters = new List<ModuleLetter>();

            var letter = new ModuleLetter();
            letter.LetterActionData = new LetterActionData()
            {
                LetterTarget = Domain.Models.LetterTarget.GP,
                LetterTemplate = Domain.Models.LetterType.DepressionGpGeneric,
                LetterValues = new Dictionary<string, object>()
            };

            //is letter required
            var triggerAnswers = answers.Where(a => a.QuestionLabel == "13.4").Select(a => a).ToList();
            if (triggerAnswers.Count() > 0)
            {
                letter.Required = true;
            }

            letters.Add(letter);

            return letters;
        }
        /// <summary>
        /// ID:39 "Medicines review Enc 2-10 Part 2":
        /// </summary>
        /// <param name="answers"></param>
        /// <param name="answerSet"></param>
        /// <returns></returns>
        private List<ModuleLetter> ProcessMedicinesReviewP2(IList<Questionnaires.Core.Services.Models.AnswerSetAnswer> answers, Questionnaires.Core.Services.Models.AnswerSet answerSet)
        {
            List<ModuleLetter> letters = new List<ModuleLetter>();

            //"Letter to GP generic - depression"
            //-----------------------------------
            var letter = new ModuleLetter();              
            letter.LetterActionData = new LetterActionData()
            {
                LetterTarget = Domain.Models.LetterTarget.GP,
                LetterTemplate = Domain.Models.LetterType.DepressionGpMedication,
                LetterValues = new Dictionary<string, object>()
            };

            var triggerLabels = new[] { "18.2a", "18.5a", "18.6d", "18.9e2", "18.9c", "18.9d1", "18.9e1" };

            //is letter required
            var triggerAnswers = answers.Where(a => triggerLabels.Contains(a.QuestionLabel)).ToList();
            if (triggerAnswers.Any())
                letter.Required = true;

            //Question 18.8 has an answer other than "HIA: Finished"
            triggerAnswers = answers.Where(a => a.QuestionLabel == "18.8" 
                                            && !a.Values.Where(v => v.Value.Contains("HIA: Finished")).Any()
                                            && !a.Values.Where(v => v.Value.ToLower().Contains("i have a problem with remembering to take my medicine")).Any()
                                            && !a.Values.Where(v => v.Value.ToLower().Contains("i have practical problems with taking my medicine")).Any()
                                            && !a.Values.Where(v => v.Value.ToLower().Contains("prescription costs are an issue")).Any()
                                            ).ToList();

            if (triggerAnswers.Any())            
                letter.Required = true;
            
            letters.Add(letter);

            //""Letter to GP - suicide - depression.docx"
            //-----------------------------------
            letter = new ModuleLetter();
            letter.LetterActionData = new LetterActionData()
            {
                LetterTarget = Domain.Models.LetterTarget.GP,
                LetterTemplate = Domain.Models.LetterType.DepressionGpSuicidalFeelings,
                LetterValues = new Dictionary<string, object>()
            };
                  

            triggerAnswers = answers.Where(a => a.QuestionLabel == "18.6c" && a.Values.Where(v => v.Value.ToLower().Contains("end call")).Count() > 0).Select(a => a).ToList();
            if (triggerAnswers.Count() > 0)
                letter.Required = true;
            triggerAnswers = answers.Where(a => a.QuestionLabel == "18.8g2b").Select(a => a).ToList();
            if (triggerAnswers.Count() > 0)
                letter.Required = true;
            triggerAnswers = answers.Where(a => a.QuestionLabel == "18.8g2c" && a.Values.Where(v => v.Value.ToLower().Contains("end call")).Count() > 0).Select(a => a).ToList();
            if (triggerAnswers.Count() > 0)
                letter.Required = true;

            letters.Add(letter);

            return letters;
        }

        /// <summary>
        /// ID:32 "Encounter 4 module Part 5"
        /// </summary>
        /// <param name="answers"></param>
        /// <param name="answerSet"></param>
        /// <returns></returns>
        private List<ModuleLetter> ProcessEncounter4Mod(IList<Questionnaires.Core.Services.Models.AnswerSetAnswer> answers, Questionnaires.Core.Services.Models.AnswerSet answerSet)
        {
            List<ModuleLetter> letters = new List<ModuleLetter>();
            var letter = new ModuleLetter();
            

            //if (answerSet.QuestionnaireTitle.ToLower().Contains("encounter 4"))
            if (answerSet.QuestionnaireID == 11)
            {
                letter = GetPHQ9(answers);
                
                //is "Generate standard GP Letter – PHQ9" selected?
                var triggerAnswers = answers.Where(a => a.QuestionLabel == "4.4").Select(a => a).ToList();
                if (triggerAnswers.Count() > 0)
                    letter.Required = true; 
                letters.Add(letter);

                letter = new ModuleLetter();
                letter.LetterActionData = new LetterActionData()
                {
                    LetterTarget = Domain.Models.LetterTarget.Patient,
                    LetterTemplate = Domain.Models.LetterType.DepressionPatientEmail,
                    LetterValues = new Dictionary<string, object>()
                };
                //is letter required
                var triggerAnswer = triggerAnswers.FirstOrDefault();
                if (triggerAnswer != null)
                {
                    letter.Required = true;                    
                    foreach (var item in triggerAnswer.Values)
                    {
                        //add the selected option to the letter data so it can use used by the consuming service as a prompt
                        letter.LetterActionData.LetterValues.Add("Action", item.Value);
                    }
                }

                letters.Add(letter);
            }

            return letters;
        }

        /// <summary>
        /// ID:14 "PHQ9 process module"
        /// </summary>
        /// <param name="answers"></param>
        /// <param name="answerSet"></param>
        /// <returns></returns>
        private List<ModuleLetter> ProcessPhq9PprocessMod(IList<AnswerSetAnswer> answers, AnswerSet answerSet)
        {
            List<ModuleLetter> letters = new List<ModuleLetter>();
            
            //""Letter to GP - suicide - depression.docx"
            //-----------------------------------
            ModuleLetter suicideLetter = new ModuleLetter();            
            suicideLetter.LetterActionData = new LetterActionData()
            {
                LetterTarget = Domain.Models.LetterTarget.GP,
                LetterTemplate = Domain.Models.LetterType.DepressionGpSuicidalFeelings,
                LetterValues = new Dictionary<string, object>()
            };
            ModuleLetter phq9Letter = new ModuleLetter();
            phq9Letter = GetPHQ9(answers);

            var triggerAnswers = answers.Where(a =>  a.QuestionLabel == "15.1" ||
                                                //a.QuestionLabel == "15.1a" ||
                                                //a.QuestionLabel == "15.1b" ||
                                                a.QuestionLabel == "15.2" ||
                                                //a.QuestionLabel == "15.2a" ||
                                                //a.QuestionLabel == "15.2b" ||
                                                a.QuestionLabel == "15.3.0" 
                                                //a.QuestionLabel == "15.5a" ||
                                                //a.QuestionLabel == "15.5b"
                                                ).Select(a => a).ToList();
            if (triggerAnswers.Count() > 0)
            {
                suicideLetter.Required = true;
                phq9Letter.Required = true;
            }

            triggerAnswers = answers.Where(a => a.QuestionLabel == "15.1b" && a.Values.Where(v => v.Value == "End call").Count() > 0).Select(a => a).ToList();
            letters.Add(suicideLetter);
            letters.Add(phq9Letter);

            return letters;
        }

        /// <summary>
        /// ID:48 "Encounter 8 module Part 2"
        /// </summary>
        /// <param name="answers"></param>
        /// <param name="answerSet"></param>
        /// <returns></returns>
        private List<ModuleLetter> ProcessEncounter8Mod(IList<AnswerSetAnswer> answers, AnswerSet answerSet)
        {
            List<ModuleLetter> letters = new List<ModuleLetter>();
            var letter = new ModuleLetter();


            //if (answerSet.QuestionnaireTitle.ToLower().Contains("encounter 8"))
            if (answerSet.QuestionnaireID == 26)
            {
                letter = GetPHQ9(answers);

                //is "Generate standard GP Letter – PHQ9" selected?
                var triggerAnswers = answers.Where(a => a.QuestionLabel == "8.1b" || a.QuestionLabel== "8.1.1ab"
                                                    //&& a.Values.Where(v => v.Value.Contains("PHQ9 score")).Count() > 0
                                                    ).Select(a => a).ToList();
                if (triggerAnswers.Count() > 0)
                    letter.Required = true;
                letters.Add(letter);



                //""Letter to GP at end of CBT course - depression
                //-----------------------------------
                letter = new ModuleLetter();
                letter.LetterActionData = new LetterActionData()
                {
                    LetterTarget = Domain.Models.LetterTarget.GP,
                    LetterTemplate = Domain.Models.LetterType.DepressionGpCbtCourse,
                    LetterValues = new Dictionary<string, object>()
                };
              
                triggerAnswers = answers.Where(a => a.QuestionLabel == "8.6").Select(a => a).ToList();
                if (triggerAnswers.Count() > 0)
                    letter.Required = true;
                letters.Add(letter);

                //ToDo: 8.1.1ab  PHQ9 comparison score /decision to re-do LLTTFi
            }

            return letters;
        }

        /// <summary>
        /// ID28: "Encounter 9 module Part 1"
        /// </summary>
        /// <param name="answers"></param>
        /// <param name="answerSet"></param>
        /// <returns></returns>
        private List<ModuleLetter> ProcessEncounter9Mod(IList<AnswerSetAnswer> answers, AnswerSet answerSet)
        {
            List<ModuleLetter> letters = new List<ModuleLetter>();

            //if (answerSet.QuestionnaireTitle.ToLower().Contains("encounter 9"))
            if (answerSet.QuestionnaireID == 21)
            {
                var letter = new ModuleLetter();
                letter = GetPHQ9(answers);

                //is "Generate standard GP Letter – PHQ9" selected?
                var triggerAnswers = answers.Where(a => a.QuestionLabel == "9.4" ).Select(a => a).ToList();
                if (triggerAnswers.Count() > 0)
                    letter.Required = true;
                letters.Add(letter);

            }
            return letters;
        }

        /// <summary>
        /// ID 57: "Encounter 10 module Part 2"
        /// </summary>
        /// <param name="answers"></param>
        /// <param name="answerSet"></param>
        /// <returns></returns>
        private List<ModuleLetter> ProcessEncounter10Mod(IList<AnswerSetAnswer> answers, AnswerSet answerSet)
        {
            List<ModuleLetter> letters = new List<ModuleLetter>();

            //if (answerSet.QuestionnaireTitle.ToLower().Contains("encounter 9"))
            if (answerSet.QuestionnaireID == 29)
            {
                var letter = GetPHQ9(answers);
                var triggerAnswers = answers.Where(a => a.QuestionLabel == "10.3b").Select(a => a).ToList();
                if (triggerAnswers.Count() > 0)
                    letter.Required = true;
                letters.Add(letter);

                letter = new ModuleLetter()
                {
                    LetterActionData = new LetterActionData()
                    {
                        LetterTarget = Domain.Models.LetterTarget.GP,
                        LetterTemplate = Domain.Models.LetterType.DepressionGpInterventionEnd,
                        LetterValues = new Dictionary<string, object>()
                    }
                };
                triggerAnswers = answers.Where(a => a.QuestionLabel == "10.9a" || a.QuestionLabel == "10.9b" || a.QuestionLabel == "10.9c").Select(a => a).ToList();
                if (triggerAnswers.Count() > 0)
                    letter.Required = true;
                letters.Add(letter);
            }
            return letters;
        }

        /// <summary>
        /// ID 61: Non-scheduled calls module Part 1
        /// </summary>
        /// <param name="answers"></param>
        /// <param name="answerSet"></param>
        /// <returns></returns>
        private List<ModuleLetter> ProcessNonScheduledMod(IList<AnswerSetAnswer> answers, AnswerSet answerSet)
        {
            List<ModuleLetter> letters = new List<ModuleLetter>();

            //if (answerSet.QuestionnaireTitle.ToLower().Contains("non-scheduled"))
            if (answerSet.QuestionnaireID == 30)
            {
                var letter = new ModuleLetter()
                {
                    LetterActionData = new LetterActionData()
                    {
                        LetterTarget = Domain.Models.LetterTarget.GP,
                        LetterTemplate = Domain.Models.LetterType.DepressionGpSuicidalFeelings,
                        LetterValues = new Dictionary<string, object>()
                    }
                };
                var triggerAnswers = answers.Where(a => (a.QuestionLabel == "26.0a" && a.Values.Where(v => v.Value.ToLower().Contains("yes (third unsuccessful attempt) – end call")).Count() > 0)
                                                      || (a.QuestionLabel == "26.10d" && a.Values.Where(v => v.Value.ToLower().Contains("send letter to gp")).Count() > 0)
                                                      || (a.QuestionLabel == "26.14a1" )
                                                      || (a.QuestionLabel == "26.14b1")
                                            ).Select(a => a).ToList();
                if (triggerAnswers.Count() > 0)
                    letter.Required = true;
                letters.Add(letter);

                letter = new ModuleLetter()
                {
                    LetterActionData = new LetterActionData()
                    {
                        LetterTarget = Domain.Models.LetterTarget.GP,
                        LetterTemplate = Domain.Models.LetterType.DepressionGpGeneric,
                        LetterValues = new Dictionary<string, object>()
                    }
                };
                triggerAnswers = answers.Where(a => (a.QuestionLabel == "26.10b" && a.Values.Where(v => v.Value.ToLower().Contains("yes to consent")).Count() > 0)
                                                 //|| (a.QuestionLabel == "26.14b1")
                                                 || (a.QuestionLabel == "26.20" && a.Values.Where(v => v.Value.ToLower().Contains("end call")).Count() > 0)
                                                 || (a.QuestionLabel == "26.21" )
                                            ).Select(a => a).ToList();
                if (triggerAnswers.Count() > 0)
                    letter.Required = true;
                letters.Add(letter);
            }
            return letters;
        }

        #endregion


        #region Support Methods

        private ModuleLetter GetPHQ9(IList<Questionnaires.Core.Services.Models.AnswerSetAnswer> answers)
        {
            
            bool scoreInvalid = false;            

            int score = 0;
            DateTime date = DateTime.Now;
            //q1
            var triggerAnswers = answers.Where(a => a.QuestionLabel == "PHQ-9_1").OrderByDescending(a=>a.Date).Select(a => a).ToList();
            if (triggerAnswers.Count() == 1 && triggerAnswers.First().Values.Count() == 1)
            {
                score += int.Parse(triggerAnswers.First().Values.First().Value);
                date = triggerAnswers.First().Date;
            }
            else
                scoreInvalid = true;//invalid phq9 answers
            //q2
            triggerAnswers = answers.Where(a => a.QuestionLabel == "PHQ-9_2").Select(a => a).ToList();
            if (triggerAnswers.Count() == 1 && triggerAnswers.First().Values.Count() == 1)
                score += int.Parse(triggerAnswers.First().Values.First().Value);
            else
                scoreInvalid = true;//invalid phq9 answers
            //q3
            triggerAnswers = answers.Where(a => a.QuestionLabel == "PHQ-9_3").Select(a => a).ToList();
            if (triggerAnswers.Count() == 1 && triggerAnswers.First().Values.Count() == 1)
                score += int.Parse(triggerAnswers.First().Values.First().Value);
            else
                scoreInvalid = true;//invalid phq9 answers
            //q4
            triggerAnswers = answers.Where(a => a.QuestionLabel == "PHQ-9_4").Select(a => a).ToList();
            if (triggerAnswers.Count() == 1 && triggerAnswers.First().Values.Count() == 1)
                score += int.Parse(triggerAnswers.First().Values.First().Value);
            else
                scoreInvalid = true;//invalid phq9 answers
            //q5
            triggerAnswers = answers.Where(a => a.QuestionLabel == "PHQ-9_5").Select(a => a).ToList();
            if (triggerAnswers.Count() == 1 && triggerAnswers.First().Values.Count() == 1)
                score += int.Parse(triggerAnswers.First().Values.First().Value);
            else
                scoreInvalid = true;//invalid phq9 answers
            //q6
            triggerAnswers = answers.Where(a => a.QuestionLabel == "PHQ-9_6").Select(a => a).ToList();
            if (triggerAnswers.Count() == 1 && triggerAnswers.First().Values.Count() == 1)
                score += int.Parse(triggerAnswers.First().Values.First().Value);
            else
                scoreInvalid = true;//invalid phq9 answers
            //q7
            triggerAnswers = answers.Where(a => a.QuestionLabel == "PHQ-9_7").Select(a => a).ToList();
            if (triggerAnswers.Count() == 1 && triggerAnswers.First().Values.Count() == 1)
                score += int.Parse(triggerAnswers.First().Values.First().Value);
            else
                scoreInvalid = true;//invalid phq9 answers
            //q8
            triggerAnswers = answers.Where(a => a.QuestionLabel == "PHQ-9_8").Select(a => a).ToList();
            if (triggerAnswers.Count() == 1 && triggerAnswers.First().Values.Count() == 1)
                score += int.Parse(triggerAnswers.First().Values.First().Value);
            else
                scoreInvalid = true;//invalid phq9 answers
            //q9
            triggerAnswers = answers.Where(a => a.QuestionLabel == "PHQ-9_9").Select(a => a).ToList();
            if (triggerAnswers.Count() == 1 && triggerAnswers.First().Values.Count() == 1)
                score += int.Parse(triggerAnswers.First().Values.First().Value);
            else
                scoreInvalid = true;//invalid phq9 answers

            //if all ok set the score
            if (!scoreInvalid)
            {
                //""Letter to GP - PHQ9 score - depression"
                //-----------------------------------
                var letter = new ModuleLetter();
                letter.LetterActionData = new LetterActionData()
                {
                    LetterTarget = Domain.Models.LetterTarget.GP,
                    LetterTemplate = Domain.Models.LetterType.DepressionGpPHQ9,
                    LetterValues = new Dictionary<string, object>()
                };
                letter.LetterActionData.LetterValues.Add("PHQ9Score", score);
                letter.LetterActionData.LetterValues.Add("PHQ9Date", date);
                return letter;
            }
            else
            {
                //""Letter to GP - PHQ9 score - depression"
                //-----------------------------------
                var letter = new ModuleLetter();
                letter.LetterActionData = new LetterActionData()
                {
                    LetterTarget = Domain.Models.LetterTarget.GP,
                    LetterTemplate = Domain.Models.LetterType.DepressionGpPHQ9UserInput,
                    LetterValues = new Dictionary<string, object>()
                };
                return letter;
            }
            
        }

        #endregion
    }
}
