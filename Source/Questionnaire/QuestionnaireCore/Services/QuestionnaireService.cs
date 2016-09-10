// -----------------------------------------------------------------------
// <copyright file="QuestionnaireService.cs" company="NHS Direct">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Text;
using Questionnaires.Core.DataAccess.Interfaces;
using Questionnaires.Core.Services.Covertors;
using Questionnaires.Core.Services.Models;
using Questionnaires.Core.Services.Exceptions;
using Questionnaires.Core.Services.Handlers;
using System.Transactions;
namespace Questionnaires.Core.Services
{


    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class QuestionnaireService : IQuestionnaireService
    {
        #region Fields

        private IQuestionnaireRepository _questionnaireRepository;
        private IQuestionSetRepository _questionSetsRepository;
        private IQuestionRepository  _questionRepository;
        private IQuestionSetQuestionRepository _questionSetQuestionsRepository;
        IAnswerSetRepository _answerSetRepository;
        IAnswerRepository _answerRepository;

        private QuestionConvertor _questionConvertor = new QuestionConvertor();
        private QuestionSetConvertor _questionSetConvertor = new QuestionSetConvertor();
        
        #endregion

        #region Constructor

        public QuestionnaireService(IQuestionnaireRepository questionnaireRepository,
            IQuestionSetRepository questionSetRepository, 
            IQuestionRepository questionRepository, 
            //IQuestionSetQuestionRepository questionSetQuestionRepository, 
            //IAnswerOptionRepository answerOptionRepository,
            IAnswerSetRepository answerSetRepository)
        {
            if (questionnaireRepository == null 
                || questionSetRepository == null 
                || questionRepository == null
                || questionSetRepository.QuestionSetQuestionRepository == null                 
                || answerSetRepository == null)
                throw new ArgumentNullException();

            _questionnaireRepository = questionnaireRepository;
            _questionSetsRepository = questionSetRepository;
            _questionRepository = questionRepository;
            _questionSetQuestionsRepository = _questionSetsRepository.QuestionSetQuestionRepository;
            _answerSetRepository = answerSetRepository;
            _answerRepository = answerSetRepository.AnswerRepository;

            //ensure contexts are the same so query joins will work 
            _questionSetQuestionsRepository.UnitOfWork = _questionnaireRepository.UnitOfWork;
            _questionSetsRepository.UnitOfWork = _questionnaireRepository.UnitOfWork;
            _questionRepository.UnitOfWork = _questionnaireRepository.UnitOfWork;

            _answerRepository.UnitOfWork = _answerSetRepository.UnitOfWork;

        }

        #endregion

        #region Session Methods

        public Models.AnswerSet AnswerSetStart(string participantID, string operatorID, DateTime startDate, int questionnaireID)
        {
            BusinessObjects.AnswerSet answerSet = new BusinessObjects.AnswerSet(){
                 ParticipantID = participantID,
                 OperatorID = operatorID,
                 QuestionnaireID = questionnaireID,
                 Completed = false,
                 StartDate = startDate
            };
            _answerSetRepository.Create(answerSet);
            _answerRepository.UnitOfWork.SaveChanges();

            var title = _questionnaireRepository.All().Where(a => a.QuestionnaireID == questionnaireID).Select(q => q.Name).FirstOrDefault();

            return AnswerSetConvertor.Convert(answerSet, title);
        }

        public AnswerSet AnswerSetGet(int id)
        {
            var answerSet = _answerSetRepository.All().Where(a => a.AnswerSetID == id).FirstOrDefault();
            if (answerSet == null)
                return null;
            return AnswerSetConvertor.Convert(answerSet);
        }

        public IList<Models.AnswerSet> AnswerSetList(string participantID)
        {
            var list = _answerSetRepository.All().Where(a => a.ParticipantID == participantID).Include(a=>a.Questionnaire).ToList();
            return list.Select(a => AnswerSetConvertor.Convert(a)).ToList();
        }

        IList<Models.AnswerSet> IQuestionnaireService.AnswerSetList(string participantID, int questionnaireID)
        {
            var list = _answerSetRepository.All().Where(a => a.ParticipantID == participantID && a.QuestionnaireID == questionnaireID).Include(a=>a.Questionnaire).ToList();
            return list.Select(a => AnswerSetConvertor.Convert(a)).ToList();
        }

        public void AnswerSetClose(int answerSetID,string reason)
        {
            var answerset =_answerSetRepository.All().Where(a => a.AnswerSetID == answerSetID).FirstOrDefault();
            if (answerset != null)
            {
                answerset.EndDate = DateTime.Now;
                _answerSetRepository.UnitOfWork.SaveChanges();
            }
            else
                throw new ArgumentException(string.Format("Cannot close AnswerSet, AnswerSetID {0} not found.", answerSetID));
        }

        public IPageable<QuestionSetPageItem> AnswerSetGetActiveQuestion(int answerSetID)
        {
            var answerSet = _answerSetRepository.All().Where(a => a.AnswerSetID == answerSetID).Include(q=>q.Questionnaire).FirstOrDefault();
            if (answerSet == null)
                throw new AnswerSetException(string.Format("Invalid AnswerID {0}. No answer set found.",answerSetID),answerSetID );


            List<BusinessObjects.QuestionSetQuestion > qsqList = new List<BusinessObjects.QuestionSetQuestion>();
            //check if the answer set is complete 
            if (answerSet.Completed == false)
            {
                if (answerSet.CurrentQuestionID.HasValue && answerSet.CurrentQuestionID > 0)
                {
                    //check if the current question has been used more than once. This should no longer be posible but check anyway
                    //ToDo: Remove QuestionSetQuestion entity , merge page and order into question entity and link question to questionset
                    var currentQSQs = (from qsq in _questionSetQuestionsRepository.All()                                       
                                       where qsq.QuestionID == answerSet.CurrentQuestionID
                                       select qsq).ToList();
                    if (currentQSQs.Count > 1)
                        throw new AnswerSetException(string.Format("The questionnaire {0} has been misconfigured. Question {1} is asked more than once",answerSet.AnswerSetID ,answerSet.CurrentQuestionID),answerSet.AnswerSetID);
                    

                    //get the current questions questionSet data   
                    var currentQSQ = currentQSQs.FirstOrDefault();
                    if (currentQSQ == null)
                        throw new AnswerSetException(string.Format("Current Question {0} Not Found for answerset {1} cannot continue",answerSet.CurrentQuestionID,answerSet.AnswerSetID), answerSet.AnswerSetID);

                    var page = currentQSQ.Page;

                    //get other questions on the same page as the current quesiton 
                    var qPageList = _questionSetsRepository.QuestionSetQuestionRepository.QuestionSetQuestionsGetByPage(currentQSQ.QuestionSetID, page);
                    
                    // check if the current question was found in the questionnaire
                    if (qPageList != null && qPageList.Count > 0)
                        //trim off any questions that are before the current question on the page
                        qsqList = AnswerHandler.SetStartPoint(new List<BusinessObjects.QuestionSetQuestion>(qPageList), answerSet.CurrentQuestionID.Value);
                    else
                    {
                        //ToDo: handle this condition
                        //the current question was not found in the questionnaire so place it on its own page.
                        throw new AnswerSetException("Question not in the current questionnaire possibly due to a jump condition.", answerSet.AnswerSetID);
                    }
                }
                else
                {
                    //we are at the beginning of the questionnaire 
                    
                    //get the first questionset for the questionnaire
                    int firstQuestionSetID = answerSet.Questionnaire.QuestionnaireQuestionSets.OrderBy(q => q.Order).Select(q=>q.QuestionSetID).First();

                    //get the first page of questions
                    var page = _questionSetsRepository.QuestionSetQuestionRepository.All().Include(q => q.Question).Where(qsq => qsq.QuestionSetID == firstQuestionSetID).OrderBy(m=>m.Page).GroupBy(qsq => qsq.Page).FirstOrDefault();
                    qsqList = page != null ? page.Select(q => q).ToList() : qsqList;
                }
            }

            
            IPageable<QuestionSetPageItem> questionSetPage = new Pageable<QuestionSetPageItem>();
            questionSetPage.PageCount = _questionnaireRepository.GetPageCount(answerSet.QuestionnaireID);
            if (answerSet.CurrentQuestionID.HasValue)
            {
                int? pageIndex = _questionnaireRepository.GetPageIndex(answerSet.QuestionnaireID, answerSet.CurrentQuestionID.Value);
                questionSetPage.PageIndex = pageIndex ?? 0;
            }
            else
                questionSetPage.PageIndex = 1;
            questionSetPage.Page = qsqList.Select(q => QuestionSetPageItemConvertor.Convert(q)).OrderBy(q=>q.Order).ToList();
         
            return questionSetPage;
        }
        #endregion

        #region Answers
        /// <summary>
        /// Saves the answers and supplies new questions
        /// </summary>
        /// <param name="answerSetId"></param>
        /// <param name="answers"></param>
        /// <returns></returns>
        /// <exception cref="MultipleRouteException"></exception>
        /// <exception cref="ArgumentNullException">missing answers</exception>
        /// <exception cref="ArgumentException">answers are from different or invalid page sets</exception>
        public Models.IPageable<Models.QuestionSetPageItem> AnswerAddByPage(int answerSetId, IList<Models.Answer> answers)
        {
            if(answers ==null)
                throw new ArgumentNullException("no answers provided");

            //get the session information            
            BusinessObjects.AnswerSet answerSet = _answerSetRepository.Get(answerSetId);

            

            //validate the answers and get any possible new questions
            AnswerHandler answerHandler = new AnswerHandler(_questionRepository, _questionSetsRepository,_questionnaireRepository);
            IPageable<Models.QuestionSetPageItem> questionnairePage = answerHandler.Process(answerSet, answers);

            //convert to data model answers
            var answersToAdd = answers.Select(a => GenericConvertor.Convert<BusinessObjects.Answer>(a)).ToList();

            //save the provided answers
            DateTime answeredDate = DateTime.Now;
            var answerContext = _answerRepository.UnitOfWork.Context;
            //share the same context so we save all changes at same time as transaction scope is raising its 
            _answerRepository.UnitOfWork.Context = _answerSetRepository.UnitOfWork.Context;
            //using (TransactionScope scope = new TransactionScope())
            //{
                //_answerRepository.UnitOfWork.Context.Database.Connection.Open();
                foreach (var item in answersToAdd)
                {
                    item.AnswerSetID = answerSetId;
                    item.Date = answeredDate;
                    item.Page = questionnairePage.PageIndex > 0 ? questionnairePage.PageIndex - 1 : questionnairePage.PageCount;
                    _answerRepository.Create(item);
                }
                //_answerRepository.UnitOfWork.SaveChanges();
                
                //set the current question so we have a resume point 
                if (questionnairePage.Page.Count() > 0)
                {
                    answerSet.CurrentQuestionID = questionnairePage.Page.First().QuestionID;
                    //add to the history so we can navigate back
                    _answerSetRepository.AddHistoryEvent(answerSet.AnswerSetID, answerSet.CurrentQuestionID.Value);
                }
                else
                    answerSet.CurrentQuestionID = null;

                //check if we are complete
                if (questionnairePage.Page.Count() == 0)
                {
                    answerSet.Completed = true;
                    answerSet.EndDate = answeredDate;
                    _answerSetRepository.Update(answerSet);                    
                }
                _answerSetRepository.UnitOfWork.SaveChanges();
                //restore original context
                _answerRepository.UnitOfWork.Context = answerContext;
                //scope.Complete();
            //}
            return questionnairePage;
        }

        public IList<Models.AnswerSetAnswer> AnswerList(int answerSetId)
        {
            var list = _answerSetRepository.AnswerRepository.All().Where(a => a.AnswerSetID == answerSetId).ToList();
            //return list.GroupBy(g => g.QuestionID).Select(a => AnswerConvertor.Convert(a.Select(an => an).ToList())).ToList(); 

            //get the answers grouped by questions and date (allows for repeated answers)  
            var all = (from q in list
                       group q by new { q.QuestionID, q.Date, q.Page }
                           into grp
                           select new
                           {
                               grp.Key.QuestionID,
                               grp.Key.Date,
                               grp.Key.Page,
                               Answers = grp.Select(q => q).ToList()
                           }).ToList();
            //take only the newest answer group for each question 
            var newestAnswers = (from q in all
                                 group q by new { q.QuestionID ,q.Page}
                                     into grp
                                     select new
                                     {
                                         grp.Key.QuestionID,
                                         Answers = grp.OrderByDescending(a => a.Date).First().Answers
                                     }
                     ).ToList();

            //convert the answers 
            var answers = newestAnswers.Select(a => AnswerConvertor.Convert(a.Answers)).ToList();

            return answers;             
        }
        
        public IList<Models.AnswerSetAnswer> AnswerListByPage(int answerSetId, int questionSetPageIndex)
        {
            var list = _answerSetRepository.AnswersByPage(answerSetId, questionSetPageIndex).ToList();

            //get the answers grouped by questions and date (allows for repeated answers)  
            var all = (from q in list
                       group q by new { q.QuestionID, q.Date, q.Page }
                           into grp
                           select new
                           {
                               grp.Key.QuestionID,
                               grp.Key.Date,
                               grp.Key.Page,
                               Answers = grp.Select(q => q).ToList()
                           }).ToList();
            //take only the newest answer group for each question 
            var newestAnswers = (from q in all
                                 group q by new { q.QuestionID, q.Page }
                                     into grp
                                     select new
                                     {
                                         grp.Key.QuestionID,
                                         Answers = grp.OrderByDescending(a => a.Date).First().Answers
                                     }
                     ).ToList();

            //convert the answers 
            var answers = newestAnswers.Select(a => AnswerConvertor.Convert(a.Answers)).ToList();

            return answers;
        }

        public void AnswerDelete(int answerID)
        {
            _answerSetRepository.AnswerRepository.Delete(answerID);
            _answerSetRepository.AnswerRepository.UnitOfWork.SaveChanges();
        }

        
        
        
        
        #endregion

        #region QuestionSet Methods

        public IList<Models.Questionnaire> QuestionnaireList()
        {
            var qs = _questionnaireRepository.All().ToList();       
            QuestionnaireConvertor convertor = new QuestionnaireConvertor();
            return qs.Select(q => convertor.Convert(q)).ToList();
        }

        #endregion



        public void SetPreviousQuestion(int answerSetID)
        {
            var answerSet = _answerSetRepository.Get(answerSetID);
            if (answerSet.EndDate != null)
            {
                throw new InvalidOperationException("This Questionnaire is complete)");
            }

            List<Questionnaires.Core.BusinessObjects.AnswerSetHistory> history = null;
            if(answerSet.CurrentQuestionID!=null)
            {
                history = _answerSetRepository.ListHistoryEvent(answerSet.AnswerSetID).OrderByDescending(h=>h.DateTime).ToList();
            }
            if (history!=null && history.Count() > 0)
            {
                if (history.Count() > 1)
                    answerSet.CurrentQuestionID = history[1].CurrentQuestionID;
                else
                    answerSet.CurrentQuestionID = null;
                //todo: delete previous answers so they are not included in the results
                //_answerSetRepository.AnswerRepository.Delete(a => a.QuestionID == history[0].CurrentQuestionID && a.AnswerSetID == answerSetID);
                _answerSetRepository.DeleteHistoryEvent(history[0]);
                _answerSetRepository.UnitOfWork.SaveChanges();
            }
            
        }
    }
}
