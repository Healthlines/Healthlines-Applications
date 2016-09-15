using System;
using System.ServiceModel;
using System.Collections.Generic;
using Questionnaires.Core.Services.Models;

namespace Questionnaires.Core.Services
{
    [ServiceContract]
    public interface IQuestionnaireManagementService
    {

        #region Questionnaires
        [OperationContract]
        Questionnaire QuestionnaireGet(int questionnaireID);
        [OperationContract]
        IList<Models.Questionnaire> QuestionnaireList();
        [OperationContract]
        Questionnaire QuestionnaireCreate(string name, string userID);
        [OperationContract]
        void QuestionnaireUpdate( Questionnaire questionnaire);
        [OperationContract]
        void QuestionnaireDelete(int questionnaireID);
        #endregion

        #region Question Sets
        [OperationContract]
        QuestionSet QuestionSetCreate(string name, string createdBy);
        [OperationContract]
        QuestionSet QuestionSetCreate(QuestionSet questionSet);
        [OperationContract]
        void QuestionSetUpdate(Models.QuestionSet Questionnaire);
        [OperationContract]
        IList<Models.QuestionSet> QuestionSetList();        
        [OperationContract]
        void QuestionSetDelete(Models.QuestionSet questionSet);
        //this method is assuming names are unique 
        [OperationContract]
        QuestionSet QuestionSetGet(string name);
        [OperationContract]
        void QuestionSetDelete(int questionSetId);
        [OperationContract]
        QuestionSet QuestionSetGet(int id);
    

        [OperationContract]
        QuestionSetQuestion QuestionSetQuestionSave(int questionSetID, int questionID, int pageNumber, int order);
        [OperationContract]
        QuestionSetQuestion QuestionSetQuestionSave(Models.QuestionSetQuestion questionSetQuestion);
        [OperationContract]
        IList<Models.QuestionSetQuestion> QuestionSetQuestionList(int questionSetID);
        [OperationContract]
        IList<Models.QuestionSetQuestion> QuestionSetQuestionListByQuestionID(int questionID);
        [OperationContract]
        IPageable<QuestionSetPageItem> QuestionSetGetPage(int questionSetID, int page);
        [OperationContract]
        void QuestionSetQuestionDelete(Models.QuestionSetQuestion questionSetQuestion);

        #endregion

        #region Questions
        [OperationContract]
        Question QuestionCreate(Models.Question question);
        [OperationContract]
        void QuestionDelete(Models.Question question);
        [OperationContract]
        Models.Question QuestionUpdate(Models.Question question);

        //This is a quick fix as groups are now tighly coupled to QuestionSet Names. Needs to be refactored.
        [OperationContract]
        void QuestionUpdateQuestionGroup(string oldGroupName, string newGroupName);

        [OperationContract]
        IList<Models.Question> QuestionList();
        [OperationContract]
        IList<Models.Question> QuestionList(string questionGroup);
        [OperationContract]
        IList<String> QuestionListGroups();
        [OperationContract]
        Question QuestionGet(int questionID);
        #endregion

        #region AnswerSets

        IList<AnswerSetAnswer> AnswerListByQuestion(int questionID);
        void AnswerDelete(int answerID);
        void AnswerDelete(int answerSetID ,int questionID);
        #endregion









        
    }        
}
