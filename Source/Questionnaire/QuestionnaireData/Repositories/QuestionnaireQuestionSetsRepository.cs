// -----------------------------------------------------------------------
// <copyright file="QuestionnaireQuestionSetsRepository.cs" company="NHS Direct">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Questionnaires.Core.BusinessObjects;
using Questionnaires.Core.DataAccess.Interfaces;

namespace Questionnaires.DAL.Repositories
{
    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class QuestionnaireQuestionSetsRepository :Repository<QuestionnaireQuestionSet>,IQuestionnaireQuestionSetsRepository
    {
        public QuestionnaireQuestionSetsRepository()
            : base(new DALContext())
        {

        }

        public QuestionnaireQuestionSetsRepository(IUnitOfWork unitOfWork)
            : base(unitOfWork)
        {

        }


    }
}
