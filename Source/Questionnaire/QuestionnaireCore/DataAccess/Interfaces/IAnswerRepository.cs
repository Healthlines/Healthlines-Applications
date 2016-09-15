using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Questionnaires.Core.BusinessObjects;

namespace Questionnaires.Core.DataAccess.Interfaces
{
    public interface  IAnswerSetRepository : IRepository<AnswerSet>
    {
        IAnswerRepository AnswerRepository { get; set; }

        IList<Answer> AnswersByPage(int answerSetId, int questionSetPageIndex);

        AnswerSet Get(int answerSetId);

        void AddHistoryEvent(int answerSetID, int currentQuestionID);

        IList<AnswerSetHistory> ListHistoryEvent(int answerSetID);

        void DeleteHistoryEvent(AnswerSetHistory answerSetHistory);
    }
}
