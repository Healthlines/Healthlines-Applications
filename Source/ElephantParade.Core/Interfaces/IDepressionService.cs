// -----------------------------------------------------------------------
// <copyright file="IDepressionService.cs" company="NHS Direct">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace NHSD.ElephantParade.Core.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public interface IDepressionService
        : IPatientService, IQuestionnaireResults
    {
        string QuestionnaireParticipantIDGet(string patientId);
        string QuestionnairePatientIDGet(string participantId);
    }
}
