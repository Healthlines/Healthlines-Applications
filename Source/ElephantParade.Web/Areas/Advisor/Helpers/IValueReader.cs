using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Questionnaires.Core.Services.Models;

namespace Questionnaires.Web.Helpers
{
    public interface IValueReader
    {
        IEnumerable<Answer> Read(QuestionSetPageItem question,string key, IDictionary<string, string> f);
    }
}