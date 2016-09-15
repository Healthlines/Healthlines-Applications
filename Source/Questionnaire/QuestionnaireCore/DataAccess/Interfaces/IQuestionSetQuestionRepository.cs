// -----------------------------------------------------------------------
// <copyright file="IQuestionRepository.cs" company="NHS Direct">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Questionnaires.Core.BusinessObjects;

namespace Questionnaires.Core.DataAccess.Interfaces
{   

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public interface IQuestionSetQuestionRepository : IRepository<QuestionSetQuestion>
    {
        IList<QuestionSetQuestion> QuestionSetQuestionsGetByPage(int questionSetID, int pageIndex);
    }

    
}
