// -----------------------------------------------------------------------
// <copyright file="QuestionSetPageItemConvertor.cs" company="NHS Direct">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Questionnaires.Core.Services.Covertors
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Questionnaires.Core.Services.Models;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class QuestionSetPageItemConvertor
    {

        public static QuestionSetPageItem Convert(BusinessObjects.QuestionSetQuestion qsq)
        {
            QuestionConvertor questionConvertor = new QuestionConvertor();
            QuestionSetPageItem qpi = GenericConvertor.Convert<QuestionSetPageItem>(qsq);

            qpi.Question = questionConvertor.Convert(qsq.Question);
            return qpi;
        }

    }
}
