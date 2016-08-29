// -----------------------------------------------------------------------
// <copyright file="PatientEvent.cs" company="NHS Direct">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace NHSD.ElephantParade.Domain.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class PatientEvent
    {
        public delegate void PatientEventHandler(object sender, PatientEvent e);

        public enum EventTypes
        {
            PatientDetailsUpdated = 1,
            PatientEmailed = 2,
            GPEmailed = 3,
            PatientAdded,
            PatientAddedEmail,
            PatientDeleted
        }

        public int EventID
        { get; set; }

        public string StudyID
        { get; set; }

        /// <summary>
        /// Gets or sets the identifier for the user.
        /// </summary>
        [ScaffoldColumn(true)]
        public string PatientId { get; set; }


        public EventTypes EventCode
        { get; set; }

        public DateTime Date
        { get; set; }

        public string User
        { get; set; }

        public string Value1
        { get; set; }

        public string Value2
        { get; set; }

        public string Details
        { get; set; }
    }
}
