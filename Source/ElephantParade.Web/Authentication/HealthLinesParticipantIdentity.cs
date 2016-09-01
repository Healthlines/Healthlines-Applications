using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Security.Principal;
using System.Web.Security;

namespace NHSD.ElephantParade.Web.Authentication
{
    public class HealthLinesParticipantIdentity : IIdentity
    {
        #region Ctors
        public HealthLinesParticipantIdentity(string name, string displayName, string participantId)
        {
            this.Name = name;
            this.DisplayName = displayName;
            this.PatientId = participantId;
        }

        public HealthLinesParticipantIdentity(string name, UserInfo userInfo)
            : this(name, userInfo.DisplayName, userInfo.UserId)
        {
            if (userInfo == null) throw new ArgumentNullException("userInfo");
            this.PatientId = userInfo.PatientId;
            this.StudyID = userInfo.StudyID;
        }

        public HealthLinesParticipantIdentity(FormsAuthenticationTicket ticket)
            : this(ticket.Name, UserInfo.FromString(ticket.UserData))
        {
            if (ticket == null) throw new ArgumentNullException("ticket");
        }
        #endregion

        #region IIdentity
        public string Name { get; private set; }

        public string AuthenticationType
        {
            get { return "HealthLinesParticipant"; }
        }

        public bool IsAuthenticated
        {            
            get { return true; }
        }
        #endregion

        /// <summary>
        /// The display name of the user
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// the patient Id if applicable for the current user
        /// </summary>
        public string PatientId { get; set; }

        /// <summary>
        /// the study key
        /// </summary>
        public string StudyID { get; set; }

    }
}