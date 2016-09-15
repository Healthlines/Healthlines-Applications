// -----------------------------------------------------------------------
// <copyright file="AnswerRepository.cs" company="NHS Direct">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Questionnaires.Core.BusinessObjects;
using Questionnaires.Core.DataAccess.Interfaces;
using System.Data.Entity;

namespace Questionnaires.DAL.Repositories
{    

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class AnswerSetRepository : Repository<AnswerSet>, IAnswerSetRepository
    {
        #region Fields

        IAnswerRepository _answerRepository = null;

        #endregion

        #region Ctors
        public AnswerSetRepository()
            : this(new DALContext())
        {

        }
                        
        public AnswerSetRepository(IUnitOfWork unitOfWork)
        :base(unitOfWork)
        {
            _answerRepository = new AnswerRepository(unitOfWork);
        }
        #endregion

        #region Properties
        public IAnswerRepository AnswerRepository
        {
            get
            {
                return _answerRepository;
            }
            set
            {
                _answerRepository = value;
            }
        }

        #endregion
               
        public IList<Answer> AnswersByPage(int answerSetId, int questionnairePageIndex)
        {
            var query = base.GetQuery<Answer>().Include(a=>a.Question).Where(a => a.AnswerSetID == answerSetId && a.Page == questionnairePageIndex);
            return query.ToList();            
        }


        public AnswerSet Get(int answerSetId)
        {
            return this.All().Where(aSet=>aSet.AnswerSetID == answerSetId).FirstOrDefault();
        }


        public void AddHistoryEvent(int answerSetID, int questionID)
        {
            AnswerSetHistory ah = new AnswerSetHistory();
            ah.AnswerSetID = answerSetID;
            ah.CurrentQuestionID = questionID;
            ah.DateTime = DateTime.Now;
            this.UnitOfWork.Context.Set<AnswerSetHistory>().Add(ah);
        }


        public IList<AnswerSetHistory> ListHistoryEvent(int answerSetID)
        {
            return this.UnitOfWork.Context.Set<AnswerSetHistory>().Where(a => a.AnswerSetID == answerSetID).ToList();    
        }


        public void DeleteHistoryEvent(AnswerSetHistory answerSetHistory)
        {
            this.UnitOfWork.Context.Set<AnswerSetHistory>().Remove(answerSetHistory); 
        }
    }
}
