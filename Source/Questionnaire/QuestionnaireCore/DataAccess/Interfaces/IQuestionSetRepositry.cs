// -----------------------------------------------------------------------
// <copyright file="IQuestionSetRepository.cs" company="NHS Direct">
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
    public interface  IQuestionSetRepository : IRepository<QuestionSet>
    {
        IQuestionSetQuestionRepository QuestionSetQuestionRepository {get;set;}

        void Delete(int questionSetID);

        IList<QuestionSetQuestion> QuestionsByPage(int questionSetId, int questionSetPageIndex);

        QuestionSet QuestionSetGet(int id);
        QuestionSet QuestionSetGet(string name);
        //int PageCount(int questionSetID);


       
    }
}
