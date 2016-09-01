// -----------------------------------------------------------------------
// <copyright file="IQuestionnaireResults.cs" company="NHS Direct">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace NHSD.ElephantParade.Core.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using NHSD.ElephantParade.Domain.Models;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public interface IQuestionnaireResults
    {
        /// <summary>
        /// gets the questionnire ParticipantID based on the patientID
        /// </summary>
        /// <param name="patientId"></param>
        /// <returns></returns>
        //string QuestionnaireParticipantIDGet(string patientID);

        /// <summary>
        /// returns a list of questionnaire sessions for a patient
        /// </summary>
        /// <param name="patientId"></param>
        /// <returns></returns>
        IList<QuestionnaireSession> QuestionnaireSessionList(string patientId);
        
        /// <summary>
        /// returns a questionnaire session with the questions and selected answers 
        /// </summary>
        /// <param name="resultSetId"></param>
        /// <returns></returns>
        QuestionnaireResults QuestionnaireResultGet(string resultSetId);
        /// <summary>
        /// returns a list of letter actions and data needed to generate the letters
        /// </summary>
        /// <param name="resultSetId"></param>
        /// <returns></returns>
        QuestionnaireActions QuestionnaireActionsGet(string resultSetId);
        /// <summary>
        /// returns a list of letter actions and data needed to generate the letters
        /// </summary>
        /// <param name="resultSetID"></param>
        /// <returns></returns>
        IList<QuestionnaireLetterAction> QuestionnaireActionsGetByPatient(string patientId);
        ///// <summary>
        ///// Check for any unprocessed letters or actions
        ///// </summary>
        ///// <param name="patientID"></param>
        ///// <returns></returns>
        //void QuestionnaireActionsRefresh(string resultSetID);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="studyId"></param>
        /// <param name="resultsSetId"></param>
        /// <param name="letterId"></param>
        /// <param name="processedBy"></param>
        /// <param name="fileId"></param>
        void QuestionnaireActionLetterProcessed(string studyId, string resultsSetId, string letterId, string recipient, string processedBy, int? fileId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="letterActionId"></param>
        /// <param name="processedBy"></param>
        /// <param name="fileId"></param>
        void QuestionnaireActionLetterProcessed(int letterActionId, string processedBy, int? fileId);
    }
}
