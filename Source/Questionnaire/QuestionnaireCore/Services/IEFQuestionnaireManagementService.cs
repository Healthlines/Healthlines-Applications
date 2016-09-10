using System;
namespace Questionnaires.Core.Services
{

    /// <summary>
    /// Not Currently working
    /// </summary>
    public interface IEFQuestionnaireManagementService
    {
        Questionnaires.Core.BusinessObjects.Question QuestionCreate(Questionnaires.Core.BusinessObjects.Question question);
        void QuestionDelete(Questionnaires.Core.BusinessObjects.Question question);
        System.Collections.Generic.IList<Questionnaires.Core.BusinessObjects.Question> QuestionList();
        Questionnaires.Core.BusinessObjects.QuestionSet QuestionnaireCreate(string name, string createdBy);
        System.Collections.Generic.IList<Questionnaires.Core.BusinessObjects.QuestionSet> QuestionnaireList();

        void QuestionnaireDelete(BusinessObjects.QuestionSet qs);

        System.Collections.Generic.IList<BusinessObjects.QuestionSetQuestion> QuestionnaireQuestionsList(int p);

        void QuestionnaireSave(BusinessObjects.QuestionSet Questionnaire);
    }
}
