// -----------------------------------------------------------------------
// <copyright file="Context.cs" company="NHS Direct">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace NHSD.ElephantParade.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Security.Principal;
    using System.Web;
    using System.Threading;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    internal class Context
    {
        //static NameValueCollection _cache;

        /// <summary>
        /// Returns the current user context for the authenticated user.
        /// </summary>
        internal static IPrincipal UserPrincipal
        {
            get
            {
                if (HttpContext.Current == null)
                    return Thread.CurrentPrincipal;
                else
                    return HttpContext.Current.User;
            }
            set
            {
                if (HttpContext.Current != null)
                    HttpContext.Current.User = value;
                Thread.CurrentPrincipal = value;
            }
        }

        //internal static NameValueCollection Cache
        //{
        //    get { return _cache; }
        //}
    }
}
