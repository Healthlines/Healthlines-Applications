using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net.Mail;
using System.Web.Mvc;
using NHSD.ElephantParade.Domain.Models;
using System.Configuration;
using System.Text;

namespace NHSD.ElephantParade.Web.Helpers
{
    public class EmailHelper
    {
        //Reset Password Email
        public static string PasswordResetEmailBody(System.Web.Security.MembershipUser user, PasswordResetRequestViewModel passwordResetRequestViewModel)
        {
            //The URL to go to the controller action to set new password
            string link = ConfigurationManager.AppSettings["BaseUrl"] + "Account/SetNewPassword/" + passwordResetRequestViewModel.PasswordResetRequestId;

            StringBuilder emailText = new StringBuilder();

            emailText.Append("<div style=\"font-family:arial;\"><p>Please be advised that the information sent in this e-mail by the Healthlines Service is only for the person named in the message. ");
            emailText.Append("If you are not this person, or you are not expecting information from us please disregard and delete the message.<p>");
            emailText.Append("<p>Dear " + user.UserName + "</p>");

            emailText.Append("<p>Thank you for using the Healthlines Service.</p>");
            emailText.Append("<p>We are sending you this email because:</p>");
            emailText.Append("<p><ul><li>you clicked \"Forgotten password\" on the Healthlines Service website, or</li>");
            emailText.Append("<li>your Healthlines Advisor has asked for your password to be reset.</li></ul></p>");

            emailText.Append("<p>Please click on the link below to change your password:</p>");
            emailText.Append("<p><a href='" + link + "'>" + link + "</a></p>");

            emailText.Append("<p>This link will take you to a webpage where you will be asked to enter a new password. Please note that this link will only remain active for 72 hours.</p>");

            emailText.Append("<p>Your user name has not changed. Your user name is " + user.UserName + ".</p>");

            emailText.Append("<p>Kind regards<p>");
            emailText.Append("<p>Healthlines Service Team</p>");

            emailText.Append("<p>The content of this message is provided for information purposes only, and is not intended as a substitute for a consultation with a health professional.</p>");
            emailText.Append("<p>If you have further queries connected with this information, please contact us on 0345 603 0897 or email us at: Snhs.healthlines@nhs.net and a Healthlines ");
            emailText.Append("Advisor will get back to you between the hours of 10.00 – 20.00 hrs weekdays and 10.00 -14.00 hrs on Saturdays.<p></div>");

            return emailText.ToString();
        }
    }
}