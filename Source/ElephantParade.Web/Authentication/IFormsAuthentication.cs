using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;

namespace NHSD.ElephantParade.Web.Authentication
{
    public interface IFormsAuthentication
    {
        /// <summary>
        /// Forces signout from the authorization system.
        /// </summary>
        void Signout();

        /// <summary>
        /// Adds the encrypted <see cref="FormsAuthenticationTicket"/> to the response stream with an expiration of 20 minutes from <see cref="DateTime.Now"/>.
        /// </summary>
        /// <param name="httpContext"></param>
        /// <param name="authenticationTicket"></param>
        void SetAuthCookie(HttpContextBase httpContext, FormsAuthenticationTicket authenticationTicket);

        /// <summary>
        /// Adds the encrypted <see cref="FormsAuthenticationTicket"/> to the response stream with an expiration of 20 minutes from <see cref="DateTime.Now"/>.
        /// </summary>
        /// <param name="httpContext"></param>
        /// <param name="authenticationTicket"></param>
        void SetAuthCookie(HttpContext httpContext, FormsAuthenticationTicket authenticationTicket);

        /// <summary>
        /// Decrypts a ticket from a string and returns a <see cref="FormsAuthenticationTicket"/>
        /// </summary>
        /// <param name="encryptedTicket"></param>
        /// <returns></returns>
        FormsAuthenticationTicket Decrypt(string encryptedTicket);

        /// <summary>
        /// Encrypts a ticket and returns a string
        /// </summary>
        /// <param name="encryptedTicket"></param>
        /// <returns></returns>
        string Encrypt(FormsAuthenticationTicket ticket);
    }
}