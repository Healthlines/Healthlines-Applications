// -----------------------------------------------------------------------
// <copyright file="DateProvider.cs" company="NHS Direct">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Questionnaires.Core.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Data.Entity;
    using Questionnaires.Core.Services;
    using Questionnaires.Core.BusinessObjects;
    using Questionnaires.Core.DataAccess.Interfaces;
    using Questionnaires.Core.BusinessObjects.Interfaces;
    using System.Data;
    using System.Data.Entity.Infrastructure;
    using Questionnaires.Core.Services.Covertors;
    using System.Collections;
    using Questionnaires.Core.Services.Handlers;


    /// <summary>
    /// TODO: Update summary.
    /// </summary>    
    public class QuestionnaireManagementService : IQuestionnaireManagementService
    {
        #region Fields
        private IQuestionnaireRepository _questionnaireRepository;
        private IQuestionSetRepository _questionSetsRepository;
        private IQuestionRepository  _questionRepository;
        private IQuestionSetQuestionRepository _questionSetQuestionsRepository;
        private IAnswerSetRepository _answerSetRepository;

        //setup member medel convertors so the can be easily changed into interfaces and passed in if required
        private QuestionConvertor _questionConvertor = new QuestionConvertor();
        private QuestionSetConvertor _questionSetConvertor = new QuestionSetConvertor();
        private QuestionnaireConvertor _questionnaireConvertor = new QuestionnaireConvertor();
        #endregion

        #region Constructor

        public QuestionnaireManagementService(IQuestionnaireRepository questionnaireRepository,
            IQuestionSetRepository questionSetRepository,
            IQuestionRepository questionRepository,
            //IQuestionSetQuestionRepository questionSetQuestionRepository,
            //IAnswerOptionRepository answerOptionRepository,
            IAnswerSetRepository answerSetRepository)
        {
            if (questionnaireRepository == null || questionSetRepository == null || questionRepository == null || questionSetRepository.QuestionSetQuestionRepository == null)
                throw new ArgumentNullException();
            _questionnaireRepository = questionnaireRepository;
            _questionSetsRepository = questionSetRepository;
            _questionRepository = questionRepository;             
            _questionSetQuestionsRepository = questionSetRepository.QuestionSetQuestionRepository;
            _answerSetRepository = answerSetRepository;
        }

        #endregion

        #region QuestionnaireSets

        #region QuestionSet

        public Models.QuestionSet QuestionSetCreate(string name, string createdBy)
        {
            QuestionSet qs = new QuestionSet();
            qs.CreatedBy = createdBy;
            qs.CreatedDate = DateTime.Now;
            qs.Name = name;
            QuestionSet newqs = _questionSetsRepository.Create(qs);
            _questionSetsRepository.UnitOfWork.SaveChanges();

            return _questionSetConvertor.Convert(newqs);          
        }

        public Models.QuestionSet QuestionSetCreate(Models.QuestionSet questionSet)
        {
            if (questionSet.QuestionSetID == 0)
                throw new ArgumentException("questionSetId has been set");

            QuestionSet createdQs = _questionSetsRepository.Create(_questionSetConvertor.Convert(questionSet));
            _questionSetsRepository.UnitOfWork.SaveChanges();

            return _questionSetConvertor.Convert(createdQs);
        }

        public Models.QuestionSet QuestionSetGet(int id)
        {            
            var qs = _questionSetsRepository.QuestionSetGet(id);
            if (qs == null)
                return null;
            return _questionSetConvertor.Convert(qs);
        }
        public Models.QuestionSet QuestionSetGet(string name)
        {
            var qs = _questionSetsRepository.QuestionSetGet(name);
            if (qs == null)
                return null;
            return _questionSetConvertor.Convert(qs);
        }

        public IList<Models.QuestionSet> QuestionSetList()
        {
            var qs = _questionSetsRepository.All().Include(q => q.QuestionSetQuestions).ToList();
            return qs.Select(q => _questionSetConvertor.Convert(q)).ToList();
        }

        /// <summary>
        /// Updates a question set to match the provided object map
        /// </summary>
        /// <remarks>as no client object tracking the Database hit will be high for this method we drop the previous map and recreate</remarks>
        /// <param name="questionSet"></param>
        public void QuestionSetUpdate(Models.QuestionSet questionSet)
        {
            if (questionSet.QuestionSetID == 0)
                throw new ArgumentException("invalid question set");            
            QuestionSet updatedQuestionSet = _questionSetConvertor.Convert(questionSet);

            
            _questionSetsRepository.Update(updatedQuestionSet);                
        }

        public void QuestionSetDelete(Models.QuestionSet qs)
        {
            QuestionSetDelete(qs.QuestionSetID);    
        }

        public void QuestionSetDelete(int questionSetID)
        {
            _questionSetsRepository.Delete(questionSetID);
            _questionSetsRepository.UnitOfWork.SaveChanges();
        }
        #endregion

        #region QuestionSetQuestions

        public Models.QuestionSetQuestion QuestionSetQuestionSave(Models.QuestionSetQuestion questionSetQuestion)
        {
            return QuestionSetQuestionSave(questionSetQuestion.QuestionSetID, questionSetQuestion.QuestionID, questionSetQuestion.Page, questionSetQuestion.Order);
        }

        public Models.QuestionSetQuestion QuestionSetQuestionSave(int questionSetId, int questionID, int pageNumber, int order)
        {
            BusinessObjects.QuestionSetQuestion qsqToUpdate = _questionSetsRepository.QuestionSetQuestionRepository.All().Where(qsq => qsq.QuestionSetID == questionSetId && qsq.QuestionID == questionID).FirstOrDefault();

            if (qsqToUpdate == null)
            {
                qsqToUpdate = new QuestionSetQuestion() { QuestionSetID = questionSetId, QuestionID = questionID, Page = pageNumber, Order = order };
                qsqToUpdate = _questionSetsRepository.QuestionSetQuestionRepository.Create(qsqToUpdate);
            }
            else
            {
                qsqToUpdate.Page = pageNumber;
                qsqToUpdate.Order = order;
                _questionSetsRepository.QuestionSetQuestionRepository.Update(qsqToUpdate);
            }
            _questionSetsRepository.UnitOfWork.SaveChanges();
            return GenericConvertor.Convert<Models.QuestionSetQuestion>(qsqToUpdate);
        }

        public IList<Models.QuestionSetQuestion> QuestionSetQuestionList(int questionSetID)
        {
            var qs = _questionSetsRepository.QuestionSetQuestionRepository.All().Where(qsq => qsq.QuestionSetID == questionSetID).ToList();
            return qs.Select(qsq=> GenericConvertor.Convert<Models.QuestionSetQuestion>(qsq)).ToList();
        }

        public IList<Models.QuestionSetQuestion> QuestionSetQuestionListByQuestionID(int questionID)
        {
            var qs = _questionSetsRepository.QuestionSetQuestionRepository.All().Where(qsq => qsq.QuestionID == questionID).ToList();
            return qs.Select(qsq => GenericConvertor.Convert<Models.QuestionSetQuestion>(qsq)).ToList();
        }

        public void QuestionSetQuestionDelete(Models.QuestionSetQuestion questionSetQuestion)
        {
            _questionSetsRepository.QuestionSetQuestionRepository.Delete(new QuestionSetQuestion() { QuestionSetQuestionID = questionSetQuestion.QuestionSetQuestionID.Value });
            //_questionSetQuestionsRepository.Delete(new QuestionSetQuestion() { QuestionSetQuestionID = questionSetQuestion.QuestionSetQuestionID.Value });
            _questionSetsRepository.QuestionSetQuestionRepository.UnitOfWork.SaveChanges();
        }

        
        public Models.IPageable<Models.QuestionSetPageItem> QuestionSetGetPage(int questionSetID, int pageIndex)
        {
            var tmp = _questionSetQuestionsRepository.QuestionSetQuestionsGetByPage(questionSetID, pageIndex);
            var pageData = tmp.Select(qsq => QuestionSetPageItemConvertor.Convert(qsq)).ToList();

            Models.Pageable<Models.QuestionSetPageItem> page = new Models.Pageable<Models.QuestionSetPageItem>();
            page.Page = pageData;
            page.PageIndex = pageIndex;
            page.PageCount = _questionSetQuestionsRepository.All().Where(qsq => qsq.QuestionSetID == questionSetID).Select(q => q.Page).Distinct().Count();

            return page;
        }
        #endregion               

        #endregion

        #region Questions

        public Models.Question QuestionCreate(Models.Question mQuestion)
        {
            var question = _questionConvertor.Convert(mQuestion);
            Question q = _questionRepository.Create(question);
            _questionRepository.UnitOfWork.SaveChanges();
            return _questionConvertor.Convert(q);
        }

        public IList<Models.Question> QuestionList()
        {
            var ql = _questionRepository.All().Include(q => q.AnswerOptions).ToList();
            return ql.Select(q => _questionConvertor.Convert(q)).ToList();
        }
        public Models.Question QuestionGet(int questionID)
        {
            Question q = _questionRepository.Get(questionID);
            if(q!=null)
                return _questionConvertor.Convert(q);

            return null;
        }

        public IList<Models.Question> QuestionList(string questionGroup)
        {
            var ql = _questionRepository.All().Include(q => q.AnswerOptions).Where(q=>q.QuestionGroup == questionGroup).ToList();
            return ql.Select(q => _questionConvertor.Convert(q)).ToList();
        }
                
        public Models.Question QuestionUpdate(Models.Question question)
        {
            BusinessObjects.Question dbQuestion = _questionRepository.All().Where(q => q.QuestionID == question.QuestionID).Include(q => q.AnswerOptions).FirstOrDefault();

            if (dbQuestion == null)
                throw new ArgumentException(string.Format("invalid questionID {0}.", question.QuestionID));

            //Note: the following as this will delete all AnswerOptions and recreate them with different IDs

            _questionRepository.AnswerOptionRepository.Delete(ao => ao.QuestionID == dbQuestion.QuestionID);
            _questionConvertor.Fill(question,  dbQuestion);
            
            _questionRepository.UnitOfWork.SaveChanges();
            return _questionConvertor.Convert(dbQuestion);
        }

        /// <summary>
        /// Renames a question group name
        /// </summary>
        /// <remarks>
        /// This is a quick fix as groups are now tightly coupled to QuestionSet Names. Needs to be refactored.
        /// </remarks>
        /// <param name="oldGroupName"></param>
        /// <param name="newGroupName"></param>
        public void QuestionUpdateQuestionGroup(string oldGroupName, string newGroupName)
        {
            var qs = _questionRepository.All().Where(q => q.QuestionGroup == oldGroupName);
            foreach (var item in qs)
            {
                item.QuestionGroup = newGroupName;
            }
            _questionRepository.UnitOfWork.SaveChanges();
        }

        public void QuestionDelete(Questionnaires.Core.Services.Models.Question question)
        {
            Question q = new Question() { QuestionID = question.QuestionID };            
            _questionRepository.Delete(q);
            _questionRepository.UnitOfWork.SaveChanges();
        }

        public IList<string> QuestionListGroups()
        {
            return _questionRepository.ListGroups();
        }

        #endregion

        #region Answer Methods

        public IList<Models.AnswerSetAnswer> AnswerListByQuestion(int questionID)
        {
            var list = _answerSetRepository.AnswerRepository.All().Where(a => a.QuestionID == questionID).ToList();
            return list.Select(a => GenericConvertor.Convert<Models.AnswerSetAnswer>(a)).ToList();
        }

        public void AnswerDelete(int answerID)
        {
            _answerSetRepository.AnswerRepository.Delete(answerID);
            _answerSetRepository.AnswerRepository.UnitOfWork.SaveChanges();
        }
        public void AnswerDelete(int answerSetID, int questionID)
        {
            _answerSetRepository.AnswerRepository.Delete(a => a.QuestionID == questionID && a.AnswerSetID == answerSetID);
            throw new NotImplementedException();
        }
        #endregion



        #region Questionnaires
        public Models.Questionnaire QuestionnaireGet(int questionnaireID)
        {
            var questionnaire = _questionnaireRepository.All().Where(q => q.QuestionnaireID == questionnaireID).FirstOrDefault();
            return _questionnaireConvertor.Convert(questionnaire);
        }

        public IList<Models.Questionnaire> QuestionnaireList()
        {
            var qs = _questionnaireRepository.All().ToList();
            return qs.Select(q => _questionnaireConvertor.Convert(q)).ToList();
        }

        public Models.Questionnaire QuestionnaireCreate(string name, string userID)
        {
            Questionnaire q = new Questionnaire();
            q.Name = name;
            q.CreatedBy = userID;
            q.CreationDate = DateTime.Now;
            _questionnaireRepository.Create(q);
            _questionnaireRepository.UnitOfWork.SaveChanges();
            return _questionnaireConvertor.Convert(q);
        }

        public void QuestionnaireUpdate(Models.Questionnaire questionnaire)
        {
            if (questionnaire.QuestionnaireID == 0)
                throw new ArgumentException("invalid questionnaire. (ID = 0)");
            
            Questionnaire updatedQuestionnaire = _questionnaireConvertor.Convert(questionnaire);
            _questionnaireRepository.Update(updatedQuestionnaire);
            _questionnaireRepository.UnitOfWork.SaveChanges();
        }

        public void QuestionnaireDelete(int questionnaireID)
        {
            _questionnaireRepository.Delete(questionnaireID);
            _questionnaireRepository.UnitOfWork.SaveChanges();
        }
        #endregion


        
    }
}
