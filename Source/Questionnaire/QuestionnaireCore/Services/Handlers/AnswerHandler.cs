// -----------------------------------------------------------------------
// <copyright file="AnswerHandler.cs" company="NHS Direct">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Questionnaires.Core.Services.Models;
using Questionnaires.Core.DataAccess.Interfaces;
using Questionnaires.Core.Services.Exceptions;
using Questionnaires.Core.Services.Covertors;
using System.Data.Entity;

namespace Questionnaires.Core.Services.Handlers
{   

    /// <summary>
    /// Handles incomming answers.
    /// Validates answers and returns the next set of questions to ask
    /// </summary>
    public class AnswerHandler //: IAnswerHandler
    {
        #region fields
        IQuestionRepository _questionRepository;
        IQuestionSetRepository _questionSetsRepository;
        IQuestionSetQuestionRepository _questionSetQuestionRepository;
        IQuestionnaireRepository _questionnaireRepository;
        #endregion

        #region Ctor
        public AnswerHandler(IQuestionRepository questionRepository ,IQuestionSetRepository questionSetsRepository,IQuestionnaireRepository questionnaireRepository)
        {
            _questionnaireRepository = questionnaireRepository;
            _questionRepository = questionRepository;
            _questionSetsRepository = questionSetsRepository;
            _questionSetQuestionRepository = questionSetsRepository.QuestionSetQuestionRepository;
        }
        #endregion

        #region Methods

        /// <summary>
        /// Validates answers and returns the next set of questions to ask
        /// </summary>
        /// <param name="answers"></param>
        /// <returns>questions to ask.</returns>
        /// <exception cref="MultipleRouteCondition"></exception>
        /// <exception cref="ArgumentException">answers are from different page sets</exception>
        public IPageable<Models.QuestionSetPageItem> Process(BusinessObjects.AnswerSet answerSet, IList<Answer> answers)
        {            
            if (answers == null )
                throw new ArgumentNullException("answers");
            
            BusinessObjects.QuestionSetQuestion currentQuestion = null;

            //check we have an answer for the current question
            if ( answerSet.CurrentQuestionID != null)
            {
                //get the current questionsetid.
                //Note - we are now assuming questions can only be in one question set at a time (linked by question groupname = questionset name) need to remove QuestionSetQuestion
                currentQuestion = _questionSetQuestionRepository.All().Where(qsq => qsq.QuestionID == answerSet.CurrentQuestionID.Value).First();

                if (currentQuestion.Question.Required &&  answers.Where(a => a.QuestionID == answerSet.CurrentQuestionID).Count() == 0)
                    throw new AnswerException("Current question requires an answer.");
            }
            else
            {
                //get the first questionset for the questionnaire
                int firstQuestionSetID = answerSet.Questionnaire.QuestionnaireQuestionSets.OrderBy(q => q.Order).Select(q => q.QuestionSetID).First();
                //get the first question for first questionset
                currentQuestion = _questionSetsRepository.QuestionSetQuestionRepository.All().Include(q => q.Question).Where(qsq => qsq.QuestionSetID == firstQuestionSetID).OrderBy(m => m.Page).ThenBy(m => m.Order).First();                
            }

            try
            {
                //validate the answers and get the QuestionSet page based on the last question answered
                //null is returned if no more pages are left to answer from the current questionSet
                ValidateAnswers(currentQuestion.QuestionID, currentQuestion.QuestionSetID, answers);
                
            }
            catch (Exception ex)
            {
                //validation failed 
                throw ex;
            }

            List<QuestionSetPageItem> newQuestions = new List<QuestionSetPageItem>();
            bool end;
            //check for a possible condition route 
            BusinessObjects.Question conditionQuestion = getConditionQuestion(answers, out end);
            if (!end)
            {
                if (conditionQuestion != null)
                {
                    //ToDo: We might need to check if the the routed question exists within this questionnaire but at the moment we are not

                    //find this question in a QuestionSet and get any more questions on the same page    
                    var qsqList = (from q in _questionSetsRepository.QuestionSetQuestionRepository.All()
                                   join jumpq in
                                       (
                                           (from qsq in _questionSetsRepository.QuestionSetQuestionRepository.All()
                                            join qs in _questionSetsRepository.All() on qsq.QuestionSetID equals qs.QuestionSetID
                                            where
                                            qsq.QuestionID == conditionQuestion.QuestionID
                                            select qsq
                                           )) on new { QuestionSetID = q.QuestionSetID } equals new { QuestionSetID = (Int32)jumpq.QuestionSetID }
                                   where
                                   q.Page == jumpq.Page
                                   select q).ToList();

                    //if the routed question is in this questionnaire..
                    if (qsqList.Count > 0)
                    {
                        QuestionConvertor questionConvertor = new QuestionConvertor();

                        //ensure the routed question is the first question on the page
                        newQuestions = SetStartPoint(qsqList, conditionQuestion.QuestionID).Select(
                            qsq => QuestionSetPageItemConvertor.Convert(qsq)).ToList();
                    }
                    else
                        throw new ConfigurationException(string.Format("The routed question id:{0} does not exist within the current questionnaire", conditionQuestion.QuestionID));

                }
                else
                {
                    //no condition question found  
                    // if another page of questions in the current QuestionSet then load them
                    var tmp = _questionSetsRepository.QuestionSetQuestionRepository.QuestionSetQuestionsGetByPage(currentQuestion.QuestionSetID, currentQuestion.Page + 1);
                    newQuestions = tmp.Select(qsq => QuestionSetPageItemConvertor.Convert(qsq)).ToList();
                }

                //if we have no questions to display then check if we have a new question set to start
                if (newQuestions.Count == 0)
                {
                    int? nextQS = GetNextQuestionSet(answerSet.QuestionnaireID, currentQuestion.QuestionSetID);
                    if (nextQS.HasValue)
                    {
                        //get the next questionsets page of questions
                        var page = _questionSetsRepository.QuestionSetQuestionRepository.All().Include(q => q.Question).Where(qsq => qsq.QuestionSetID == nextQS.Value).GroupBy(qsq => qsq.Page).FirstOrDefault();

                        newQuestions = page != null ? page.Select(qsq => QuestionSetPageItemConvertor.Convert(qsq)).ToList() : newQuestions;
                    }
                }
            }
            //set up return data
            Pageable<Models.QuestionSetPageItem> questionSetPageItem = new Pageable<QuestionSetPageItem>();
            questionSetPageItem.Page = newQuestions.OrderBy(q => q.Order).ToList();
            
            if (newQuestions.Count > 0)            
                questionSetPageItem.PageIndex = _questionnaireRepository.GetPageIndex(answerSet.QuestionnaireID, newQuestions[0].QuestionID) ?? 0;

            questionSetPageItem.PageCount = _questionnaireRepository.GetPageCount(answerSet.QuestionnaireID);
            
            return questionSetPageItem;
        }

        

        /// <summary>
        /// gets the id of the next questionSet for a given questionnaire ID
        /// </summary>
        /// <param name="questionnaireID"></param>
        /// <param name="currentQuestionSetID"></param>
        /// <returns></returns>
        private int? GetNextQuestionSet(int questionnaireID,int currentQuestionSetID)
        {
            BusinessObjects.Questionnaire questionnaire = _questionnaireRepository.All().Where(q => q.QuestionnaireID == questionnaireID).FirstOrDefault();
            var qqsList = questionnaire.QuestionnaireQuestionSets.OrderBy(qqs => qqs.Order).ToArray();

            for (int i = 0; i < qqsList.Length; i++)
            {
                if (qqsList[i].QuestionSetID == currentQuestionSetID)
                {
                    if (qqsList.Length > i+1 )
                        return qqsList[i + 1].QuestionSetID;
                }
            }
            return null;
        }
        /// <summary>
        /// Validates the provided answers
        /// </summary>
        /// <remarks>
        /// If more than one question answered it must be from a configured page
        /// If only one answer assume the answer could be from a routed question
        /// </remarks>
        /// <param name="answers"></param>
        /// <returns>page index for the next set of questions within the current QuestionSet. null if answers are for the last page</returns>
        /// <exception cref="ArgumentException">answers are from different page sets</exception>
        private void ValidateAnswers(int currentQuestionID, int currentQuestionSetID,IList<Answer> answers)
        {            
            var op = from a in answers where a.OperatorID ==null || a.OperatorID.Trim().Length == 0 select a;
            if (op.Count() != 0)
                throw new AnswerException("Missing OperatorID");
            
            //Validate answeroptions are valid
            foreach (var item in answers)
            {
                BusinessObjects.AnswerOption answerOption = _questionRepository.AnswerOptionRepository.All().Where(ao => ao.AnswerOptionID == item.AnswerOptionID && ao.QuestionID == item.QuestionID).FirstOrDefault();
                //if(_questionRepository.AnswerOptionRepository.All().Where(ao => ao.AnswerOptionID == item.AnswerOptionID && ao.QuestionID == item.QuestionID).Count()!=1)
                if (answerOption == null)
                    throw new AnswerException("Missing or invalid answer.AnswerOptionID");
                if (item.Value == null)
                    item.Value = answerOption.DefaultValue;

                //if the answer option type is a selection which has no value set, use the lable so the results will show something
                if ((item.Type == AnswerOption.OptionType.check || item.Type == AnswerOption.OptionType.radio)
                    && string.IsNullOrEmpty(item.Value.Trim()))
                {
                    item.Value = answerOption.Label;
                }
            }

            //validate the answers page

            
            //
            //check all the questions are in the current module and the same page                
            //
                
            //get the current question
            var question = _questionSetsRepository.QuestionSetQuestionRepository.All().Where(qsq => qsq.QuestionID == currentQuestionID ).First();

            //get all the questions for the current questions page.
            IList<BusinessObjects.QuestionSetQuestion> questionsSetQuestions = _questionSetsRepository.QuestionSetQuestionRepository.QuestionSetQuestionsGetByPage(currentQuestionSetID, question.Page);

            //Validate provided answers are from the same page set
            var answersFromDiffPage = (from a in answers where !(from q in questionsSetQuestions select q.QuestionID).Contains(a.QuestionID) select a).ToList();
                
            if (answersFromDiffPage.Count > 0)
                throw new AnswerExceptionNotInQuestionSetPage(string.Format("The provided answer for question {2} is not applicable for the Question Set: {0} Page: {1}. All answers must be for the same page of questions.", currentQuestionSetID.ToString(), question.Page.ToString(), answersFromDiffPage[0].QuestionID.ToString()));
                
            //
            //check if all the questions on the page have been answered. 
            //   
            questionsSetQuestions = SetStartPoint(questionsSetQuestions.ToList(), question.QuestionID);
            var questionsNotAnswered = (from q in questionsSetQuestions 
                                        where !(from a in answers select a.QuestionID).Contains(q.QuestionID)
                                            && q.Question.Required== true
                                        select q).ToList();

            if (questionsNotAnswered.Count != 0)
                throw new AnswerExceptionMissingAnswer("Missing answers to current page");
        }

        
                

        /// <summary>
        /// gets any conditions for a list of answer options answer
        /// </summary>
        /// <param name="answers"></param>
        /// <returns></returns>
        /// <exception cref="MultipleRouteCondition"></exception>
        private BusinessObjects.Question getConditionQuestion(IList<Answer> answers,out bool end)
        {
            end = false;
            var AnswerOptionIDs= answers.Select(a => a.AnswerOptionID).ToArray();
            //get AnswerOption conditions for supplied answers
            var conditionAnswerOptions = _questionRepository.AnswerOptionRepository.All().Where(
                ao =>  AnswerOptionIDs.Contains(ao.AnswerOptionID) &&  ao.RouteQuestionID!=null ).Select(ao=>ao).ToList();
            
            //error if page answers contain more than one possible route condition as we dont know which is the correct route
            if (conditionAnswerOptions.Count > 1)
            {
                //filter duplicate route id
                if (conditionAnswerOptions.Select(m => m.RouteConditionID).Distinct().Count() == 1)
                    conditionAnswerOptions = conditionAnswerOptions.Take(1).ToList();
                else
                    throw new MultipleRouteCondition(string.Format("MultipleRouteCondition for question {0} routed to questions {1}", answers[0].QuestionID, string.Concat(conditionAnswerOptions.Select(a => a.QuestionID.ToString() + ","))));
            }

            
            if (conditionAnswerOptions.Count>0)
            {
                var routedAnswerOption = conditionAnswerOptions[0];

                //check if we are checking a condition or just jumping to the specified question
                if (routedAnswerOption.Condition != null)
                {
                    // get the condition
                    Models.Condition condition = GenericConvertor.Convert<Models.Condition>(routedAnswerOption.Condition);
                    //get the specified answer 
                    var answer = answers.Where(a => a.AnswerOptionID == routedAnswerOption.AnswerOptionID).FirstOrDefault();

                    if (condition.Evaluate(answer.Value))
                    {
                        //check for end condition
                        if (routedAnswerOption.RouteQuestionID == 0)
                        {
                            end = true;
                            return null;
                        }
                        //get the routed question
                        return _questionRepository.Get(routedAnswerOption.RouteQuestionID.Value);
                    }
                }
                else
                {
                    //check for end condition
                    if (routedAnswerOption.RouteQuestionID == 0)
                    {
                        end = true;
                        return null;
                    }
                    //no condition so we just jump to the specified question                    
                    var q =  _questionRepository.Get(routedAnswerOption.RouteQuestionID.Value);
                    if (q == null)
                        throw new ConfigurationException(string.Format("Routed Question {0} does not exist. Please update AnswerOption {1}", routedAnswerOption.RouteQuestionID.Value, routedAnswerOption.AnswerOptionID));
                    return q;
                }
            }
            
            return null; 
        }
        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="qsqList"></param>
        /// <param name="startQuestionID"></param>
        /// <returns></returns>
        public static List<BusinessObjects.QuestionSetQuestion> SetStartPoint(List<BusinessObjects.QuestionSetQuestion> qsqList, int startQuestionID)
        {
            qsqList = qsqList.OrderBy(qsq => qsq.Order).ThenBy(qsq => qsq.QuestionSetQuestionID).ToList();
            int index = qsqList.FindIndex(qsq => qsq.QuestionID == startQuestionID);
            qsqList.RemoveRange(0, index);

            return qsqList;

        }
        
    }
}
