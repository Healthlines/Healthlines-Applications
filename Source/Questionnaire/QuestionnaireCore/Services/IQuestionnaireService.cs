// -----------------------------------------------------------------------
// <copyright file="IQuestionnaireService.cs" company="NHS Direct">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Questionnaires.Core.Services.Models;
using System.ServiceModel;

namespace Questionnaires.Core.Services
{

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    [ServiceContract]
    public interface IQuestionnaireService
    {

        #region Questionnaires


        //#region QuestionSets
        /// <summary>
        /// Gets available question sets
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        IList<Models.Questionnaire> QuestionnaireList();
        /// <summary>
        /// Lists a page of questions 
        /// </summary>
        /// <param name="questionSetID"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        //[OperationContract]
        //IPageable<QuestionSetPageItem> QuestionSetQuestionsGetByPage(int questionSetID, int page);
        #endregion

        #region AnswerSets (Sessions)
        /// <summary>
        /// Starts a new answer set for a questionnaire
        /// </summary>
        /// <param name="participantID"></param>
        /// <param name="operatorID"></param>
        /// <param name="startDate"></param>
        /// <param name="questionSetID"></param>
        /// <returns></returns>   
        [OperationContract]
        AnswerSet AnswerSetStart(string participantID, string operatorID, DateTime startDate, int questionnaireID);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [OperationContract]
        AnswerSet AnswerSetGet(int id);
        /// <summary>
        /// Lists answer sets for a participant
        /// </summary>
        /// <param name="participantID"></param>
        /// <returns></returns>  
        [OperationContract]
        IList<Models.AnswerSet> AnswerSetList(string participantID);      
        /// <summary>
        /// 
        /// </summary>
        /// <param name="participantID"></param>
        /// <param name="questionnaireID"></param>
        /// <returns></returns>
        [OperationContract]
        IList<Models.AnswerSet> AnswerSetList(string participantID, int questionnaireID);
        /// <summary>
        /// Returns the next set of unanswerd questions for a answerset
        /// </summary>
        /// <param name="answerSetID"></param>
        /// <returns></returns>
        /// <exception cref="QuestionnaireService.Core.Services.Exceptions.AnswerSetException"></exception>
        [OperationContract]
        IPageable<QuestionSetPageItem> AnswerSetGetActiveQuestion(int answerSetID);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="questionSetID"></param>
        /// <param name="reason"></param>
        [OperationContract]
        void AnswerSetClose(int answerSetID, string reason);

        [OperationContract]
        void SetPreviousQuestion(int answerSetID);
        #endregion

        #region Answers

        /// <summary>
        /// Saves an list of answers
        /// </summary>
        /// <param name="answers"></param>
        /// <returns>the next set of questions based on the supplied answers</returns>
        /// <exception cref="InvalidAnswers">Answers are missing from question sequence or more than one answer results in routing</exception>
        [OperationContract]
        IPageable<QuestionSetPageItem> AnswerAddByPage(int answerSetId, IList<Answer> answers);
        
        /// <summary>
        /// Lists answers saved for an answerSet
        /// </summary>
        /// <param name="answerSetId"></param>
        /// <returns></returns>
        [OperationContract]
        IList<AnswerSetAnswer> AnswerList(int answerSetId);
        //[OperationContract]
        //IList<AnswerSetAnswer> AnswerListByQuestion(int questionID);

        /// <summary>
        /// Lists answers saved for an answerSet
        /// </summary>
        /// <param name="answerSetId"></param>
        /// <returns></returns>
        [OperationContract]
        IList<AnswerSetAnswer> AnswerListByPage(int answerSetId, int questionnairPageIndex);

        /// <summary>
        /// deletes an answer
        /// </summary>
        /// <param name="answerID"></param>
        [OperationContract]
        void AnswerDelete(int answerID);

        #endregion


        
    }
}
