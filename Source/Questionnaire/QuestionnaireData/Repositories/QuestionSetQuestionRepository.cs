// -----------------------------------------------------------------------
// <copyright file="QuestionSetQuestionRepository.cs" company="NHS Direct">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Text;
using Questionnaires.Core.DataAccess.Interfaces;
using Questionnaires.Core.BusinessObjects;
namespace Questionnaires.DAL.Repositories
{
    

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class QuestionSetQuestionRepository : Repository<QuestionSetQuestion >, IQuestionSetQuestionRepository
    {
        #region Ctors

        public QuestionSetQuestionRepository()
            : this(new DALContext())
        {

        }
        public QuestionSetQuestionRepository(IUnitOfWork unitOfWork)
        :base(unitOfWork)
        {

        }

        #endregion

        #region Methods

        public override void Update(QuestionSetQuestion TObject)
        {
            
            base.Update(TObject);
        }

        public override void Delete(QuestionSetQuestion TObject)
        {
            QuestionSetQuestion qsqToDelete = this.All().Where(qsq => qsq.QuestionSetQuestionID == TObject.QuestionSetQuestionID).FirstOrDefault();
            base.Delete(qsqToDelete);
            this.UnitOfWork.SaveChanges();
        }

        public IList<QuestionSetQuestion> QuestionSetQuestionsGetByPage(int questionSetID, int pageIndex)
        {
            return base.All().Include(qsq=>qsq.Question).Where(qsq => qsq.QuestionSetID == questionSetID && qsq.Page == pageIndex).ToList();
        }


        public IList<QuestionSetQuestion> GetQuestionnairePageQuestions(int questionID, int questionnairID)
        {

            throw new NotImplementedException();
        }

        #endregion
    }
}
