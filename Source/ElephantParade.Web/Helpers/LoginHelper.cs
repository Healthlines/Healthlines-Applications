using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NHSD.StringFunctions;

namespace NHSD.ElephantParade.Web.Helpers
{
    public class LoginHelper
    {
        /// <summary>
        /// Get the username for the current user. Without Domain, and in lower case.
        /// </summary>
        public static string GetLoggedInUser()
        {
            string loggedInName = HttpContext.Current.User.Identity.Name;
            return StringFunctions.StringFunctions.FilterOutDomain(loggedInName).ToLower();
        }
    }
}