// -----------------------------------------------------------------------
// <copyright file="AnswerRepository.cs" company="NHS Direct">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Questionnaires.DAL.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Questionnaires.Core.BusinessObjects;
    using Questionnaires.Core.DataAccess.Interfaces;
    using System.Linq.Expressions;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class AnswerRepository : Repository<Answer>, IAnswerRepository
    {
        public AnswerRepository()
            : this(new DALContext())
        {

        }

        public AnswerRepository(IUnitOfWork unitOfWork)
        :base(unitOfWork)
        {
            
        }


        public  void Delete(int answerID)
        {
            var answer = base.All().Where(a => a.AnswerID == answerID).FirstOrDefault();
            base.Delete(answer);         
        }

        public override void Delete(Expression<Func<Answer, bool>> predicate)
        {
            var answers = base.All().Where(predicate);
            foreach (var answer in answers)
            {
                base.Delete(answer); 
            }
        }
    }
}
