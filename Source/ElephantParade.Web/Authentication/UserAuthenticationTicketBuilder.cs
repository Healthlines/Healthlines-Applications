using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using NHSD.ElephantParade.Web.Models;
using NHSD.ElephantParade.Domain.Models;


namespace NHSD.ElephantParade.Web.Authentication
{
    public class UserAuthenticationTicketBuilder
    {
        /// <summary>
        /// Creates a new <see cref="FormsAuthenticationTicket"/> from a user.
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        /// <remarks>
        /// Encodes the <see cref="UserInfo"/> into the <see cref="FormsAuthenticationTicket.UserData"/> property
        /// of the authentication ticket.  This can be recovered by using the <see cref="UserInfo.FromString"/> method.
        /// </remarks>
        public static FormsAuthenticationTicket CreateAuthenticationTicket(string userName, StudyPatient user, bool isPersistent)
        {
            UserInfo userInfo = CreateUserContextFromPatient(user);
            userInfo.UserId = userName;
            return CreateAuthenticationTicket(userName, userInfo, isPersistent);
        }

        public static FormsAuthenticationTicket CreateAuthenticationTicket(string userName, UserInfo userInfo, bool isPersistent)
        {           
            var ticket = new FormsAuthenticationTicket(
                1,
                userName,
                DateTime.Now,
                DateTime.Now.Add(FormsAuthentication.Timeout),
                isPersistent,
                userInfo.ToString());

            return ticket;
        }

        private static UserInfo CreateUserContextFromPatient(StudyPatient user)
        {
            var userContext = new UserInfo
            {
                PatientId = user.PatientId,
                StudyID = user.StudyID,
                DisplayName = user.DisplayName           
            };

            return userContext;
        }
    }
}