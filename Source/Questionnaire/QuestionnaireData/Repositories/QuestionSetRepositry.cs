// -----------------------------------------------------------------------
// <copyright file="QuestionSetRepository.cs" company="NHS Direct">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Text;
using Questionnaires.Core.BusinessObjects;
using Questionnaires.Core.DataAccess.Interfaces;

namespace Questionnaires.DAL.Repositories
{    

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class QuestionSetRepository : Repository<QuestionSet>, IQuestionSetRepository
    {
        #region Fields

        IQuestionSetQuestionRepository _questionSetQuestionRepository;

        #endregion

        #region Ctors

        public QuestionSetRepository()
            : this(new DALContext())
        {

        }

        public QuestionSetRepository(IUnitOfWork unitOfWork)
            : base(unitOfWork)
        {
            _questionSetQuestionRepository = new QuestionSetQuestionRepository(unitOfWork);
        }

        #endregion

        #region Properties

        public IQuestionSetQuestionRepository QuestionSetQuestionRepository
        {
            get
            {
                return _questionSetQuestionRepository;
            }
            set
            {
                _questionSetQuestionRepository = value;
            }
        }

        #endregion

        #region Methods

        public void Delete(int questionSetID)
        {
            QuestionSet qs = this.All().Where(q => q.QuestionSetID == questionSetID).FirstOrDefault();
            base.Delete(qs);            
        }

        public override void Delete(QuestionSet questionSet)
        {
            QuestionSet qs = this.All().Where(q => q.QuestionSetID == questionSet.QuestionSetID).FirstOrDefault();
            this.Delete(qs);            
        }

        public override void Update(QuestionSet questionSet)
        {
            QuestionSet qsToUpdate = this.All().Where(q => q.QuestionSetID == questionSet.QuestionSetID).FirstOrDefault();
            qsToUpdate.Name = questionSet.Name;
            qsToUpdate.CreatedBy = questionSet.CreatedBy;
            qsToUpdate.CreatedDate = questionSet.CreatedDate;

            this._questionSetQuestionRepository.Delete(qsq => qsq.QuestionSetID == questionSet.QuestionSetID);

            //get the QuestionSetQuestions into array as adding them to qsToUpdate will remove them from questionSet
            var items = questionSet.QuestionSetQuestions.ToArray();

            foreach (var item in items)            
                qsToUpdate.QuestionSetQuestions.Add(item);
          
            base.Update(qsToUpdate);
            this.UnitOfWork.SaveChanges();
        }


        public IList<QuestionSetQuestion> QuestionsByPage(int QuestionSetID, int questionSetPageIndex)
        {            
            return this.QuestionSetQuestionRepository.All().Where(qsq => qsq.QuestionSetID == QuestionSetID && qsq.Page == questionSetPageIndex).ToList();
        }


        public QuestionSet QuestionSetGet(int id)
        {
            return  base.All().Include(q => q.QuestionSetQuestions).Where(q => q.QuestionSetID == id).FirstOrDefault();
        }
        public QuestionSet QuestionSetGet(string name)
        {
            return base.All().Include(q => q.QuestionSetQuestions).Where(q => q.Name  == name).FirstOrDefault();
        }
        #endregion


        
    }
}
