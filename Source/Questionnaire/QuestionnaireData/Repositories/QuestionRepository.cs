// -----------------------------------------------------------------------
// <copyright file="QuestionSetRepository.cs" company="NHS Direct">
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
    public class QuestionRepository : Repository<Question>, IQuestionRepository
    {
        #region Fields
        IAnswerOptionRepository _answerOptionRepository;
        #endregion

        #region Ctors
        public QuestionRepository()
            : this(new DALContext())
        {

        }

        public QuestionRepository(IUnitOfWork unitOfWork)
        :base(unitOfWork)
        {
            _answerOptionRepository = new AnswerOptionRepository(unitOfWork);
        }

        #endregion

        #region Properties
       
        public IAnswerOptionRepository AnswerOptionRepository
        {
            get
            {
                return _answerOptionRepository;
            }
            set
            {
                _answerOptionRepository = value;
            }
        }

        #endregion

               
        public override void Delete(Question question)
        {
            Question qToDelete = this.All().Where(q => q.QuestionID == question.QuestionID).First();
            base.Delete(qToDelete);
            this.UnitOfWork.SaveChanges();
        }


        public IList<string> ListGroups()
        {
            return base.All().Select(q => q.QuestionGroup).Distinct().ToList();
        }


        public Question Get(int questionID)
        {
            return base.All().Where(q => q.QuestionID == questionID).FirstOrDefault();
        }

        public int? GetQuestionnairePageIndex(int questionID,int questionnaireID)
        {
            //base.All().Where(q=>q.QuestionID == questionID).Select(q=>q.QuestionSetQuestions.Where(qsq=>qsq.QuestionSet.QuestionnaireQuestionSets.Where(qqs=>qqs.QuestionnaireID == 1))
            
            //base.All().Where(q=>q.QuestionID == questionID).First().QuestionSetQuestions.Where(qsq=>qsq.QuestionSetID 
            
            throw new NotImplementedException();
        }
    }

    
}
