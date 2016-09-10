// -----------------------------------------------------------------------
// <copyright file="QuestionConvertor.cs" company="NHS Direct">
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
    public class QuestionSetConvertor :  IConvertor<BusinessObjects.QuestionSet, Models.QuestionSet>
    {

        public Models.QuestionSet Convert(BusinessObjects.QuestionSet source)
        {     
            Models.QuestionSet questionSet = new Models.QuestionSet();
            this.Fill(source, questionSet);
            return questionSet;
        }
        
        public BusinessObjects.QuestionSet Convert(QuestionSet source)
        {
            BusinessObjects.QuestionSet questionSet = new BusinessObjects.QuestionSet();
            this.Fill(source,  questionSet);
            return questionSet;
        }
        
        public void Fill(BusinessObjects.QuestionSet source,  QuestionSet target)
        {
            target.CreatedBy = source.CreatedBy;
            target.CreatedDate = source.CreatedDate;
            target.Name = source.Name;
            target.QuestionSetID = source.QuestionSetID;

            if(target.QuestionSetQuestions ==null)
                target.QuestionSetQuestions = new List<Models.QuestionSetQuestion>();
            target.QuestionSetQuestions.Clear();
            foreach (var item in source.QuestionSetQuestions)
            {
                target.QuestionSetQuestions.Add(
                    GenericConvertor.Convert<Models.QuestionSetQuestion>(item)
                    );
            }
        }

        public void Fill(QuestionSet source,  BusinessObjects.QuestionSet target)
        {
            target.Name = source.Name;
            target.CreatedDate = source.CreatedDate;
            target.CreatedBy = source.CreatedBy;
            target.QuestionSetID = source.QuestionSetID;

            target.QuestionSetQuestions.Clear();
            foreach (var item in source.QuestionSetQuestions)
            {
                target.QuestionSetQuestions.Add(
                    GenericConvertor.Convert<BusinessObjects.QuestionSetQuestion>(item)
                    );
            }
        }
    }
}
