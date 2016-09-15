using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Questionnaires.Core.BusinessObjects;

namespace Questionnaires.Core.DataAccess.Interfaces
{
    public interface  IAnswerRepository : IRepository<Answer>
    {
        void Delete(int answerID);
    }
}
