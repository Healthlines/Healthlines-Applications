// -----------------------------------------------------------------------
// <copyright file="IQuestionnaireRepository.cs" company="NHS Direct">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Questionnaires.Core.DataAccess.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public interface IQuestionnaireRepository : IRepository<Questionnaires.Core.BusinessObjects.Questionnaire>
    {
        IQuestionnaireQuestionSetsRepository QuestionnaireQuestionSetsRepository{get;}
        //IQuestionSetQuestionRepository QuestionSetQuestionsRepository { get; }

        int GetPageCount(int questionnaireID);
        int? GetPageIndex(int questionnaireID, int questionID);

        void Delete(int questionnaireID);
    }
}
