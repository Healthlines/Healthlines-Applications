using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using NHSD.ElephantParade.Domain.Properties;
using System.ComponentModel;

namespace NHSD.ElephantParade.Domain.Models
{
    public class InterventionSchedule
    {
        public int InterventionScheduleID { get; set; }
        public int ParticipantID { get; set; }
        public DateTime WindowOpen { get; set; }
        public DateTime WindowClose { get; set; }
        public string EventType { get; set; }
        public string Status { get; set; }
        public short Seq { get; set; }
        public int encounterID { get; set; }
    }
}
