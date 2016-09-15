// -----------------------------------------------------------------------
// <copyright file="QuestionnaireRepository.cs" company="NHS Direct">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Questionnaires.Core.DataAccess.Interfaces;
using Questionnaires.Core.BusinessObjects;

namespace Questionnaires.DAL.Repositories
{    

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class QuestionnaireRepository :Repository<Questionnaires.Core.BusinessObjects.Questionnaire> ,IQuestionnaireRepository
    {
        private IQuestionnaireQuestionSetsRepository _questionnaireQuestionSetsRepository;

        public QuestionnaireRepository()
            :this(new DALContext())
        {

        }
        public QuestionnaireRepository(IUnitOfWork unitOfWork)
            : this(new QuestionnaireQuestionSetsRepository(unitOfWork))
        {
        }

        public QuestionnaireRepository(QuestionnaireQuestionSetsRepository questionSetRepoistory)
            : base(questionSetRepoistory.UnitOfWork)
        {
            _questionnaireQuestionSetsRepository = questionSetRepoistory;
        }


        public IQuestionnaireQuestionSetsRepository QuestionnaireQuestionSetsRepository
        {
            get { return _questionnaireQuestionSetsRepository; }
        }


        public int GetPageCount(int questionnaireID)
        {
            var count = base.All().Where(qa => qa.QuestionnaireID == questionnaireID)
                .FirstOrDefault()
                .QuestionnaireQuestionSets
                .Sum(qqs => qqs.QuestionSet.QuestionSetQuestions
                        .Select(qsq => qsq.Page)
                            .Distinct()
                            .Count()
                    );
         
            return count;
        }

        public int? GetPageIndex(int questionnaireID, int questionID)
        {
            Questionnaires.Core.BusinessObjects.Questionnaire questionaire = base.All().Where(q => q.QuestionnaireID == questionnaireID).First();

            int page = 0;
            bool questionFound = false;
            foreach (QuestionnaireQuestionSet item in questionaire.QuestionnaireQuestionSets)
            {
                var q = (from qsq in item.QuestionSet.QuestionSetQuestions
                        where qsq.QuestionID == questionID
                        select qsq).FirstOrDefault();
                //if the question is not in this set then add all the pages
                if (q == null)
                    page = page + item.QuestionSet.QuestionSetQuestions.Select(qsq => qsq.Page).Distinct().Count();
                else
                {
                    var pages = item.QuestionSet.QuestionSetQuestions.Select(qsq => qsq.Page).Distinct().OrderBy(i => i).ToArray();
                    page = page + Array.IndexOf(pages, q.Page)+1;
                    questionFound = true;
                    break;
                }                
            }    
            
            if (page > 0 && questionFound)
                return page;
            else
                return null;
        }

        public override void Update(Questionnaire questionnaire)
        {
            Questionnaire qsToUpdate = this.All().Where(q => q.QuestionnaireID == questionnaire.QuestionnaireID).FirstOrDefault();
            qsToUpdate.Name = questionnaire.Name;
            qsToUpdate.CreatedBy = questionnaire.CreatedBy;
            qsToUpdate.CreationDate = questionnaire.CreationDate;

            this._questionnaireQuestionSetsRepository.Delete(qqs => qqs.QuestionnaireID == questionnaire.QuestionnaireID);
            
            //get the QuestionSetQuestions into array as adding them to qsToUpdate will remove them from questionSet
            var items = questionnaire.QuestionnaireQuestionSets.ToArray();

            foreach (var item in items)
                qsToUpdate.QuestionnaireQuestionSets.Add(item);

            base.Update(qsToUpdate);
            this.UnitOfWork.SaveChanges();
        }

        public void Delete(int questionnaireID)
        {
            var questionnaire = base.All().Where(q => q.QuestionnaireID == questionnaireID).FirstOrDefault();
            base.Delete(questionnaire);            
        }
    }
}
