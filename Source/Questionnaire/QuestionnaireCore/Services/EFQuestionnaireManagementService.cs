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
    //using StructureMap;
    using Questionnaires.Core.Services;
    using Questionnaires.Core.BusinessObjects;
    using Questionnaires.Core.DataAccess.Interfaces;
    using Questionnaires.Core.BusinessObjects.Interfaces;
    using System.Data;
    using System.Data.Entity.Infrastructure;
    


    /// <summary>
    /// 
    /// Not Currently working 
    /// 
    /// Service to provide untracked POCO objects from the entity framework
    /// 
    /// It is intended that object change tracking be done on the client and 
    /// passed back to the this service to use and perssist to the EF context.
    /// 
    /// </summary>    
    public class EFQuestionnaireManagementService : IEFQuestionnaireManagementService
    {
        private IQuestionSetRepository _questionSetsRepository;
        private IQuestionRepository  _questionRepository;
        private IQuestionSetQuestionRepository _questionSetQuestionsRepository;

        public EFQuestionnaireManagementService(IQuestionSetRepository questionSetRepository,IQuestionRepository questionRepository,IQuestionSetQuestionRepository questionSetQuestionRepository)
        {
            _questionSetsRepository = questionSetRepository;
            _questionRepository = questionRepository;             
            _questionSetQuestionsRepository = questionSetQuestionRepository;

            //_questionSetsRepository.UnitOfWork.LazyLoadingEnabled = false;
            //_questionRepository.UnitOfWork.LazyLoadingEnabled = false;
            //_questionSetQuestionsRepository.UnitOfWork.LazyLoadingEnabled = false;

            //disable the proxies so the objects are not lazy loaded from the DAL context and are returned an pure POCO
            _questionSetsRepository.UnitOfWork.ProxyCreationEnabled = false;
            _questionRepository.UnitOfWork.ProxyCreationEnabled = false;
            _questionSetQuestionsRepository.UnitOfWork.ProxyCreationEnabled = false;
        }

        #region QuestionnaireSets

        public IList<QuestionSet> QuestionnaireList()
        {  
            return  _questionSetsRepository.All().Include(q=>q.QuestionSetQuestions).AsNoTracking().ToList();
        }


        public QuestionSet QuestionnaireCreate(string name, string createdBy)
        {
            QuestionSet qs = new QuestionSet();
            qs.CreatedBy = createdBy;
            qs.CreatedDate = DateTime.Now;
            qs.Name = name;
            QuestionSet newqs = _questionSetsRepository.Create(qs);
            _questionSetsRepository.UnitOfWork.SaveChanges();
           
            return qs;
        }

        public void QuestionnaireDelete(int qsID)
        {
            var qs = new QuestionSet { QuestionSetID = qsID };
            QuestionnaireDelete(qs);
        }

        public void QuestionnaireDelete(QuestionSet qs)
        {
            _questionSetsRepository.UnitOfWork.Context.Entry(qs).State = System.Data.EntityState.Deleted;
            _questionSetsRepository.Delete(qs);
            _questionSetsRepository.UnitOfWork.SaveChanges();
        }
        #endregion
        
        #region Questions

        public  Question QuestionCreate(Question question)
        {            
            Question q = _questionRepository.Create(question);
     
            _questionRepository.UnitOfWork.SaveChanges();
            return q;              
        }

        public  IList<Question> QuestionList()
        {
            return _questionRepository.All().AsNoTracking().ToList();            
        }

        //public static IList<Question> ListByGroup(string groupName)
        //{
        //    IQuestionRepository qr = RepositoryFactory.GetQuestionRepository();
        //    //using (IQuestionRepository qr = RepositoryFactory.GetQuestionRepository())
        //    //{
        //    return qr.Filter(q => q.QuestionGroup == groupName).ToList();
        //    //}
        //}

        public void QuestionDelete(Question question)
        {
            _questionRepository.UnitOfWork.Context.Entry(question).State = System.Data.EntityState.Deleted;
            _questionRepository.Delete(question);
            _questionRepository.UnitOfWork.SaveChanges();            
        }

        #endregion

        //public static IQuestionnaireManagementService GetInstance()
        //{
        //    return ObjectFactory.GetInstance<IQuestionnaireManagementService>();

        //}

        public IList<QuestionSetQuestion> QuestionnaireQuestionsList(int questionSetID)
        {
            return _questionSetQuestionsRepository.All().Where(q => q.QuestionSetID == questionSetID).ToList();
            
        }


        //public void QuestionnaireSave(QuestionSet questionSet)
        //{
        //    bool state = _questionSetsRepository.UnitOfWork.ProxyCreationEnabled;
        //    //enable proxy  so it traces the object graph for us
        //    _questionSetsRepository.UnitOfWork.ProxyCreationEnabled = true;
        //    try
        //    {
        //        _questionSetsRepository.UnitOfWork.Context.Entry(questionSet).State = System.Data.EntityState.Added;

        //        if (questionSet.QuestionSetId > 0)
        //            _questionSetsRepository.UnitOfWork.Context.Entry(questionSet).State = System.Data.EntityState.Modified;

        //        foreach (var item in questionSet.QuestionSetQuestions)
        //        {
        //            if (item.QuestionSetQuestionId > 0)
        //                _questionSetsRepository.UnitOfWork.Context.Entry(item).State = System.Data.EntityState.Modified;

        //        }


        //        _questionSetsRepository.Update(questionSet);
        //        _questionSetsRepository.UnitOfWork.SaveChanges();
        //    }
        //    catch (Exception)
        //    {

        //        throw;
        //    }
        //    finally
        //    {
        //        _questionSetsRepository.UnitOfWork.ProxyCreationEnabled = state;
        //    }
            
            
        //}

        /// <summary>
        /// Persists changes for the whole tree of objects 
        /// -- should we do this more granular to reduce complexity of change tracking requirements
        /// </summary>
        /// <param name="questionSet"></param>
        public void QuestionnaireSave(QuestionSet questionSet)
        {
            bool state = _questionSetsRepository.UnitOfWork.ProxyCreationEnabled;
            //enable proxy  so it traces the object graph for us
            _questionSetsRepository.UnitOfWork.ProxyCreationEnabled = true;
            try
            {               

                ApplyChanges<QuestionSet>(questionSet, _questionSetsRepository.UnitOfWork);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                _questionSetsRepository.UnitOfWork.ProxyCreationEnabled = state;
            }
        }

        /*
         * #
         * #
         * #
         * # Client side change tracking stuff. Is it worth doing this???????
         * # 
         * #
         * #
         * #
         */

        #region ChnageTrackingCode

        private static void ApplyChanges<TEntity>(TEntity root,IUnitOfWork unitOfWork)
            where TEntity : class, IObjectWithState
        {
            
            unitOfWork.Context.Set<TEntity>().Add(root);
            CheckForEntitiesWithoutStateInterface(unitOfWork.Context);
            foreach (var entry in unitOfWork.Context.ChangeTracker.Entries<IObjectWithState>())
            {
                IObjectWithState stateInfo = entry.Entity;
                entry.State = ConvertState(stateInfo.State);
                if (stateInfo.State == State.Unchanged)
                {
                    ApplyPropertyChanges(entry.OriginalValues, stateInfo.OriginalValues);
                }
            }
            unitOfWork.Context.SaveChanges();
        }
        
        private static void ApplyPropertyChanges(DbPropertyValues values, Dictionary<string, object> originalValues)
        {
            foreach (var originalValue in originalValues)
            {
                if (originalValue.Value is Dictionary<string, object>)
                {
                    ApplyPropertyChanges((DbPropertyValues)values[originalValue.Key],(Dictionary<string, object>)originalValue.Value);
                }
                else
                {
                    values[originalValue.Key] = originalValue.Value;
                }
            }
        }

        private static void CheckForEntitiesWithoutStateInterface( DbContext context)
        {
            var entitiesWithoutState =  from e in context.ChangeTracker.Entries()
                                        where !(e.Entity is IObjectWithState)
                                        select e;
            if (entitiesWithoutState.Any())
            {
                throw new NotSupportedException("All entities must implement IObjectWithState");
            }
        }

        public static EntityState ConvertState(State state)
        {
            switch (state)
            {
                case State.Added:
                    return EntityState.Added;
                case State.Deleted:
                    return EntityState.Deleted;
                default:
                    return EntityState.Unchanged;
            }
        }
        #endregion


      
    }
}
