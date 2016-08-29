using System.Collections.Generic;
using System.Configuration;
using System.Web.Security;
using NHSD.ElephantParade.Core.Interfaces;
using NHSD.ElephantParade.Core.Models;
using NHSD.ElephantParade.Domain.Models;
using System.Web;

namespace NHSD.ElephantParade.Core
{
    /// <summary>
    /// Service for managing users of the system.
    /// </summary>
    public class MembershipService : IMembershipService
    {
        #region fields
        private INonSecureEmailService _emailService;
        private IStudiesService _studiesService;
        #endregion

        public MembershipService(INonSecureEmailService emailService, IStudiesService studiesService)
        {
            _emailService = emailService;
            _studiesService = studiesService;
        }

        /// <summary>
        /// Get a list of all users that have been added into AD for the system
        /// </summary>
        /// <returns></returns>
        public IList<UserViewModel> ExistingADUsers()
        {
            IList<UserViewModel> list = new List<UserViewModel>();

            var adUsers = Roles.GetUsersInRole("Advisor");

            foreach(var user in adUsers)
            {
                list.Add(new UserViewModel{ UserName = user });
            }

            return list;
        }

        /// <summary>
        /// Check the configuration - to see if we have two membership providers, or one
        /// </summary>
        /// <returns>true if dual login is enabled</returns>
        private static bool DualLoginEnabled()
        {
            bool dualLogin = false;

            //see if we should attempt a dual login
            if (ConfigurationManager.AppSettings["DualLogin"] != null)
                bool.TryParse(ConfigurationManager.AppSettings["DualLogin"], out dualLogin);

            return dualLogin;
        }

        /// <summary>
        /// Check all providers to see if user is valid.
        /// </summary>
        public bool UserValidated(string userName, string password)
        {
            bool userValidated = false;

            if (DualLoginEnabled())
            {
                //try AD first
                MembershipProvider mProvider = Membership.Providers["ADMembershipProvider"];
                userValidated = mProvider.ValidateUser(userName, password);

                if (!userValidated)
                {
                    mProvider = Membership.Providers["AspNetSqlMembershipProvider"];
                    userValidated = mProvider.ValidateUser(userName, password);
                }
            }
            if (!userValidated)
            {
                //we are still not validated so try default set up in config file.
                //as we are using explicit implementations above, 
                //this covers any other implementations that might get set up in the future.
                userValidated = Membership.ValidateUser(userName, password);
            }

            return userValidated;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="user">User to create</param>
        /// <param name="provider">Membership provider to create the user under</param>
        /// <returns></returns>
        public MembershipCreateStatus CreateMembershipUser(UserViewModel user, string provider = "AspNetSqlMembershipProvider")
        {
            MembershipProvider mProvider = Membership.Providers[provider];

            if (mProvider == null)
                return MembershipCreateStatus.ProviderError;

            // Create new user.
            MembershipCreateStatus status;
            try
            {
                if (provider.ToLower() == "admembershipprovider")
                {
                    //ensure user exists in AD
                    int usersWithUserName;
                    mProvider.FindUsersByName(user.UserName, 0, 1, out usersWithUserName);

                    if (usersWithUserName < 1)
                    {
                        return MembershipCreateStatus.InvalidUserName;
                    }
                }

                mProvider.CreateUser(user.UserName, user.Password, user.EmailAddress, null, null, true, null, out status);
            }
            catch (MembershipCreateUserException e)
            {
                return e.StatusCode;
            }
            
            //add user to roles
            if (user.Advisor)
                Roles.AddUserToRole(user.UserName, "Advisor");

            //user can only be either a supervisor or admin.
            if (user.Administrator)
                Roles.AddUserToRole(user.UserName, "Administrator");
            else if (user.Supervisor)
                Roles.AddUserToRole(user.UserName, "Supervisor");

            //add any other roles
            if (user.Roles != null && user.Roles.Length != 0)
            {
                Roles.AddUserToRoles(user.UserName, user.Roles);
            }

            return MembershipCreateStatus.Success;
        }

        public MembershipCreateStatus CreateMembershipUser(string email, string username, string password, string role)
        {
            UserViewModel userViewModel = new UserViewModel
            {
                Administrator = false,
                Advisor = false,
                EmailAddress = email,
                Password = password,
                Roles = new[] { role },
                Supervisor = false,
                UserName = username
            };

            return CreateMembershipUser(userViewModel, "AspNetSqlMembershipProvider");
        }

        public string GeneratePassword(int length, int numberOfNonAlphanumericChars)
        {
            return Membership.GeneratePassword(length, numberOfNonAlphanumericChars);
        }

        public string GetUserNameByEmail(string email)
        {
            return Membership.GetUserNameByEmail(email);
        }

        public void DeleteMembershipUserByUserName(string userName)
        {
            //try AD first
            MembershipProvider mProvider = Membership.Providers["ADMembershipProvider"];

            MembershipUser user = null;
            if(mProvider !=null)
                user = mProvider.GetUser(userName, false);

            if (user != null && !string.IsNullOrEmpty(user.UserName))
            {
                //only remove roles, we don't want to (attempt to) delete them from AD!
                Roles.RemoveUserFromRoles(userName, Roles.GetRolesForUser(userName));
            }
            else
            {
                mProvider = Membership.Providers["AspNetSqlMembershipProvider"];
                mProvider.DeleteUser(userName, true);
            }
        }

        public void DeleteMembershipUserByEmail(string email)
        {
            var u = Membership.GetUserNameByEmail(email);
            if (u != null)
            {
                Membership.DeleteUser(u);
            }
        }

        public bool UpdateMembershipUserEmailAddress(Domain.Models.PatientEmailAddressViewModel patientEmailAddressVM, 
                                                                                            string userName, string loginUser)
        {
            // Get patient type
            var pObj = _studiesService.GetPatientByEmail(userName);
            var userInDatabase = _studiesService.GetPatientByEmail(patientEmailAddressVM.NewEmailAddress);

            if (pObj != null)
                pObj.Email = patientEmailAddressVM.NewEmailAddress;
            else
                return false;

            if (userInDatabase == null)
            {
                _studiesService.UpdatePatient(pObj, loginUser);

                // Delete the existing membership user
                DeleteMembershipUserByUserName(userName);

                // Create the new user account (username/email) with the generated password and send an email to the user 
                // containing log-in credentials and instructions.
                if (pObj.StudyID == "CVD")
                    CreateCVDAccount(pObj);
                else if (pObj.StudyID == "Depression")
                    CreateDepressionAccount(pObj);

                return true;
            }
            else
                return false;
        }

        /// <summary>
        /// Creates a membership account with Depression role and emails the patient with account details
        /// </summary>
        /// <param name="newPatient"></param>
        public void CreateDepressionAccount(StudyPatient newPatient)
        {
            // Generate a new 12-character password with 1 non-alphanumeric character.
            string username = newPatient.Email;
            string password = GeneratePassword(12, 1);

            // Create the membership user here with a generated password and send to the user the initial login credentials
            var status = CreateMembershipUser(newPatient.Email, username, password, "Depression");
            if (status != MembershipCreateStatus.Success)
                throw new MembershipCreateUserException(status);

            SendEmailToUser(newPatient, username, password);
        }

        /// <summary>
        /// Creates a membership account with CVD role and emails the patient with account details
        /// </summary>
        /// <param name="newPatient"></param>
        public void CreateCVDAccount(StudyPatient newPatient)
        {
            // Generate a new 12-character password with 1 non-alphanumeric character.
            string username = newPatient.Email;
            string password = Membership.GeneratePassword(12, 1);

            // Create the membership user here with a generated password and send to the user the initial login credentials
            var status = CreateMembershipUser(newPatient.Email, username, password, "CVD");
            if (status != MembershipCreateStatus.Success)
                throw new MembershipCreateUserException(status);

            SendEmailToUser(newPatient, username, password);
        }

        public void SendEmailToUser(StudyPatient newPatient, string username, string password)
        {
            // Construct the text as drafted by the Healthlines team and to be passed in the function for creating user.
            string emailText = "<div style=\"font-family:arial;\"><p>Please be advised that the information sent in this e-mail by the NHS Direct Healthlines Service";
            emailText += " is only for the person named in the message. If you are not this person, or you are not expecting";
            emailText += " information from us please disregard and delete the message.</p>";

            emailText += "<p>Dear " + newPatient.Forename + "</p>";
            emailText += "<p>Thank you for registering with the Healthlines Service. ";
            emailText += "<p>We have pleasure in providing you with information about the Healthlines Service website.</p>";
            emailText += "<p>You can access the Healthlines Service website by clicking on this link: ";
            emailText += "<a href='" + ConfigurationManager.AppSettings["BaseUrl"] + "'>" + ConfigurationManager.AppSettings["BaseUrl"] + "</a>";
            emailText += "To enter the website and access the information on it, you need your own user name and password.</p>";
            emailText += "<p>After you click on the link above, at the right-hand side of the screen you will see two boxes where you need to enter your user name and password.</p>";
            emailText += "Your user name and password are personal to you and are shown below:";
            emailText += "<p>User name: <span style=\"padding-left:300px;\"> <b>" + HttpUtility.HtmlEncode(username) + "</b></span></p>" +
            "<p>Password: <span style=\"padding-left:300px;\"> <b>" + password + "</b></span></p>"
            + "<p>You need to enter your user name and password exactly as they are shown here (including any capital letters).</p>"
            + "<p>When you have entered the website, you can change your password to a word that is easier to remember. "
            + "Please click on \"Change Password\" at the top right-hand corner of your screen and then follow the instructions on the page.</p>"
            + "<p>One of our Healthlines Advisors will call you in the next few weeks to tell you more about the Healthlines Service and the website. "
            + "In the meantime, we hope you will find the website useful and informative.</p>"
            + "<p>Kind regards</p>"
            + "<p>Healthlines Service Team</p>"
            + "<p>The content of this message is provided for information purposes only, and is not intended as a substitute for a consultation with a health professional.</p>"
            + "<p>If you have further queries connected with this information, please contact us on 0345 603 0897 or email us at: Snhs.healthlines@nhs.net and a Healthlines "
            + "Advisor will get back to you between the hours of 10.00 – 20.00 hrs weekdays and 10.00 -14.00 hrs on Saturdays.</p></div>";

            _emailService.SendEmail(newPatient.Email, "Instructions to access the Healthlines Service Website", emailText, null, false);
        }
    }
}
