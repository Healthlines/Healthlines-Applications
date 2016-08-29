// -----------------------------------------------------------------------
// <copyright file="ICvdService.cs" company="NHS Direct">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace NHSD.ElephantParade.Core.Interfaces
{
    using System;
    using System.Linq;
    using NHSD.ElephantParade.Core.Models;
    using System.Collections.Generic;
    using NHSD.ElephantParade.Domain.Models;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public interface ICvdService
        :IPatientService ,IQuestionnaireResults
    {
        ParticipantScore AddParticipantScore(Domain.Models.ParticipantScore participantScoreViewModel, string userId);
        InterventionSchedule AddInterventionSchedule(Domain.Models.InterventionSchedule interventionScheduleViewModel);
    }
}
