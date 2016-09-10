using System;
using Questionnaires.Core.Services.Models;
namespace Questionnaires.Core.Services.Handlers
{
    interface IAnswerHandler
    {
        IPageable<Models.QuestionSetPageItem> Process(int questionnireID, System.Collections.Generic.IList<Questionnaires.Core.Services.Models.Answer> answers);
    }
}
