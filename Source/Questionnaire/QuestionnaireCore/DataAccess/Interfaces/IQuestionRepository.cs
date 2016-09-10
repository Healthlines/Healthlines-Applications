// -----------------------------------------------------------------------
// <copyright file="IQuestionRepository.cs" company="NHS Direct">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Questionnaires.Core.DataAccess.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;    
    using Questionnaires.Core.BusinessObjects;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public interface IQuestionRepository : IRepository<Question>
    {
        IAnswerOptionRepository AnswerOptionRepository { get; set; }


        IList<string> ListGroups();
        Question Get(int questionID);
    }

    
}
