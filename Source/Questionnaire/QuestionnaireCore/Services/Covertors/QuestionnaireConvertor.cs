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
    public class QuestionnaireConvertor : IConvertor<BusinessObjects.Questionnaire, Models.Questionnaire>
    {

        

        public Models.Questionnaire Convert(BusinessObjects.Questionnaire source)
        {
            Models.Questionnaire questionnaire = new Models.Questionnaire();
            this.Fill(source, questionnaire);
            return questionnaire;
        }

        public BusinessObjects.Questionnaire Convert(Questionnaire source)
        {
            BusinessObjects.Questionnaire questionnaire = new BusinessObjects.Questionnaire();
            this.Fill(source, questionnaire);
            return questionnaire;
        }

        public void Fill(BusinessObjects.Questionnaire source, Questionnaire target)
        {
           

            target.CreatedBy = source.CreatedBy;
            target.CreationDate = source.CreationDate;
            target.Name = source.Name;
            target.QuestionnaireID = source.QuestionnaireID;

            if(target.QuestionSets ==null)
                target.QuestionSets = new List<Models.QuestionnaireQuestionSet>();
            target.QuestionSets.Clear();
            foreach (var item in source.QuestionnaireQuestionSets)
            {
                target.QuestionSets.Add(
                    ConvertQuestionnaireQuestionSet(item)
                    );
            }
        }

        public void Fill(Questionnaire source, BusinessObjects.Questionnaire target)
        {
            target.Name = source.Name;
            target.CreationDate = source.CreationDate;
            target.CreatedBy = source.CreatedBy;
            target.QuestionnaireID = source.QuestionnaireID;

            target.QuestionnaireQuestionSets.Clear();
            foreach (var item in source.QuestionSets)
            {
                target.QuestionnaireQuestionSets.Add(
                    GenericConvertor.Convert<BusinessObjects.QuestionnaireQuestionSet>(item)
                    );
            }
        }

        private Models.QuestionnaireQuestionSet ConvertQuestionnaireQuestionSet(BusinessObjects.QuestionnaireQuestionSet qqs)
        {
            Models.QuestionnaireQuestionSet newQQS = new QuestionnaireQuestionSet()
            {
                Order = qqs.Order,
                QuestionnaireID = qqs.QuestionnaireID,
                QuestionSetID = qqs.QuestionSetID,
                QuestionSetName = qqs.QuestionSet.Name
            };
            return newQQS;
        }
    }
}
