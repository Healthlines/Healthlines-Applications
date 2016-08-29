using System;
using System.Web.Mvc;
using System.Web.Security;
using NHSD.ElephantParade.Core.Models;
using NHSD.ElephantParade.Web.Models;
using NHSD.ElephantParade.Web.Authentication;
using NHSD.ElephantParade.Core.Interfaces;
using System.Configuration;
using NHSD.ElephantParade.Web.Helpers;
using NHSD.ElephantParade.Domain.Models;

using System.Web.UI;

namespace NHSD.ElephantParade.Web.Controllers
{
    public class AccountController : BaseController
    {
        private IStudiesService _studiesServices;
        private IFormsAuthentication _formsAuthentication;
        private INonSecureEmailService _emailService;
        private IPageVisitedLogService _pageVisitedLogService;
        private IPasswordResetRequestService _passwordResetRequestService;
        private IMembershipService _membershipService;

        public AccountController(IFormsAuthentication formsAuthentication,
                                    IStudiesService patientService,
                                    INonSecureEmailService nonSecureEmailService,
                                    IPageVisitedLogService pageVisitedLogService,
                                    IPasswordResetRequestService passwordResetRequestService,
                                    IMembershipService membershipService)
        {
            _formsAuthentication = formsAuthentication;
            _studiesServices = patientService;
            _emailService = nonSecureEmailService;
            _pageVisitedLogService = pageVisitedLogService;
            _passwordResetRequestService = passwordResetRequestService;
            _membershipService = membershipService;
        }

        //
        // GET: /Account/LogOn
        [AllowAnonymousAttribute]
        public ActionResult LogOn()
        {
            if (User.IsInRole("Advisor"))
                return RedirectToAction("Index", "Home", new { Area = "Advisor" });
            else if (User.IsInRole("CVD") || User.IsInRole("Depression"))
                return RedirectToAction("Index", "Patient");
            else if (User.IsInRole("Administrator"))
                return RedirectToAction("Index", "Home", new { Area = "Administration" });
            else if (User.IsInRole("Uploader"))
                return RedirectToAction("StudyPatientDataUpload", "Administration", new { Area = "Administration" });

            //if the user is logged in the redirect to home page for role
            return View();
        }

        //
        // POST: /Account/LogOn
        [AllowAnonymousAttribute]
        [HttpPost]
        public ActionResult LogOn(LogOnModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                bool userValidated = _membershipService.UserValidated(model.UserName, model.Password);

                if (userValidated)
                {
                    //setup the ticket
                    UserInfo userInfo;
                    if (Roles.IsUserInRole(model.UserName, "CVD") || Roles.IsUserInRole(model.UserName, "Depression"))
                    {
                        var user = Membership.GetUser(model.UserName);

                        var patient = _studiesServices.GetPatientByEmail(user.Email);
                        if (patient == null)
                        {
                            ModelState.AddModelError("", "Failed to find patient record for " + user.Email);
                            return View(model);
                            //ToDo: get from resource file
                            throw new Exception("User authenticated but no study record found!");
                        }
                        userInfo = new UserInfo() { DisplayName = patient.DisplayName, UserId = model.UserName, StudyID = patient.StudyID, PatientId = patient.PatientId };
                    }
                    else
                        userInfo = new UserInfo() { DisplayName = model.UserName, UserId = model.UserName };

                    //apply the ticket
                    _formsAuthentication.SetAuthCookie(this.HttpContext, UserAuthenticationTicketBuilder.CreateAuthenticationTicket(model.UserName, userInfo, model.RememberMe));

                    if (Url.IsLocalUrl(returnUrl) && returnUrl.Length > 1 && returnUrl.StartsWith("/")
                        && !returnUrl.StartsWith("//") && !returnUrl.StartsWith("/\\"))
                    {
                        return Redirect(returnUrl);
                    }
                    else
                    {
                        if (Roles.IsUserInRole(model.UserName, "Advisor"))
                            return RedirectToAction("Index", "Home", new { Area = "Advisor" });
                        else if (Roles.IsUserInRole(model.UserName, "CVD") || Roles.IsUserInRole(model.UserName, "Depression"))
                            return RedirectToAction("Index", "Patient");
                        else if (Roles.IsUserInRole(model.UserName, "Administrator"))
                            return RedirectToAction("Index", "Home", new { Area = "Administration" });
                        else if (Roles.IsUserInRole(model.UserName, "Uploader"))
                            return RedirectToAction("StudyPatientDataUpload", "Administration", new { Area = "Administration" });
                    }
                }
                else
                {
                    var userInfo = Membership.GetUser(model.UserName);
                    if (userInfo != null && userInfo.IsLockedOut)
                        ModelState.AddModelError("", "Too many login attempts. Account has been locked.");
                    else
                        ModelState.AddModelError("", "User name or password incorrect");
                }
            }

            // If we got this far, something failed, redisplay form
            //As this won't get captured in the page visited log (we're not authenticated)
            //write login attempt to table.
            PageVisitedLogViewModel pageVisitedLogViewModel = new PageVisitedLogViewModel
            {
                DateVisited = DateTime.Now,
                IPAddress = HttpContext.Request.UserHostAddress,
                PageURL = HttpContext.Request.RawUrl,
                Username = model.UserName
            };

            DataHelper.UpdatePageVisitedLog(pageVisitedLogViewModel, _pageVisitedLogService);

            return View(model);
        }

        //
        // GET: /Account/LogOff

        public ActionResult LogOff()
        {
            FormsAuthentication.SignOut();

            return RedirectToAction("Index", "Home");
        }


        //
        // Get: /Account/ForgottenPassword
        [AllowAnonymousAttribute]
        public ActionResult ForgottenPassword()
        {
            return View();
        }

        //
        // GET: /Account/Register
        [AllowAnonymousAttribute]
        public ActionResult Register()
        {
            return View();
        }

        //
        // GET: /Account/ChangePassword

        [Authorize]
        public ActionResult ChangePassword()
        {
            return View();
        }

        //
        // POST: /Account/ChangePassword

        [Authorize]
        [HttpPost]
        public ActionResult ChangePassword(ChangePasswordModel model)
        {
            if (ModelState.IsValid)
            {

                // ChangePassword will throw an exception rather
                // than return false in certain failure scenarios.
                bool changePasswordSucceeded;
                try
                {
                    MembershipUser currentUser = Membership.GetUser(User.Identity.Name, true /* userIsOnline */);
                    changePasswordSucceeded = currentUser.ChangePassword(model.OldPassword, model.NewPassword);
                }
                catch (Exception)
                {
                    changePasswordSucceeded = false;
                }

                if (changePasswordSucceeded)
                {
                    return RedirectToAction("ChangePasswordSuccess");
                }
                else
                {
                    ModelState.AddModelError("", "The current password is incorrect or the new password is invalid.");
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ChangePasswordSuccess
        [AllowAnonymousAttribute]
        public ActionResult ChangePasswordSuccess()
        {
            return View();
        }

        // Notify user the email to reset password has been sent
        [AllowAnonymousAttribute]
        public ActionResult ResetEmailSent()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymousAttribute]
        public ActionResult ForgottenPassword(PasswordResetRequestViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Get username from provided email address;
                    string userName = Membership.GetUserNameByEmail(model.Email);

                    // If username exist get the membership user to reset the password
                    if (userName != null)
                    {
                        // Write the user as the username and generate a guid with a time stamp (automatically done on save).
                        model.UserName = userName;

                        var modelRetrieved = _passwordResetRequestService.AddPasswordResetRequest(model);

                        MembershipUser currentUser = Membership.GetUser(userName);

                        if (model.Email.ToLower() == currentUser.Email.ToLower())
                        {
                            // Send the email to the user for resetting the password
                            _emailService.SendEmail(currentUser.Email, "Healthlines Service Website Account", EmailHelper.PasswordResetEmailBody(currentUser, modelRetrieved), "", false);

                            return RedirectToAction("ResetEmailSent", "Account");
                        }
                        else
                            ModelState.AddModelError(string.Empty, "The email address entered does not exist.");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "The email address entered does not exist.");
                    }

                }
                catch (Exception e)
                {
                    ModelState.AddModelError(string.Empty, "Fail to send email message, please call the administrator. The error is: "
                        + e.Message + " and further info (if any): " + e.InnerException);
                }
            }
            return View();
        }

        [AllowAnonymousAttribute]
        public ActionResult SetNewPassword(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                ModelState.AddModelError(string.Empty, "The link you clicked to reach this page is invalid, or it has expired. To get a new link, please click Forgotten Password on the Home screen.");
                return View();
            }

            Guid passwordResetRequestId;
            bool validGuid = Guid.TryParse(id, out passwordResetRequestId);
            
            if (!validGuid)
            {
                ModelState.AddModelError(string.Empty, "The link you clicked to reach this page is invalid, or it has expired. To get a new link, please click Forgotten Password on the Home screen.");
                return View();
            }

            PasswordResetRequestViewModel passwordResetRequestVM = new PasswordResetRequestViewModel();
            passwordResetRequestVM = _passwordResetRequestService.GetPasswordResetRequestById(passwordResetRequestId);

            // Check against the validity of the user request (i.e. is it still there and within the last 24 hours)
            if (passwordResetRequestVM == null || passwordResetRequestVM.Date < DateTime.Now.AddHours(-72))
            {
                return View("PasswordResetExpired");
            }

            return View();
        }

        [HttpPost]
        [AllowAnonymousAttribute]
        public ActionResult SetNewPassword(string id, ChangePasswordModel model)
        {
            if (string.IsNullOrEmpty(id))
            {
                //no id passed - user must have not copied link correctly / something else has gone wrong. Get them to try again.
                ModelState.AddModelError(string.Empty, "The link you clicked to reach this page is invalid, or it has expired. To get a new link, please click Forgotten Password on the Home screen.");
                return View();
            }

            Guid passwordResetRequestId;
            bool validGuid = Guid.TryParse(id, out passwordResetRequestId);
            if (!validGuid)
            {
                ModelState.AddModelError(string.Empty, "The link you clicked to reach this page is invalid, or it has expired. To get a new link, please click Forgotten Password on the Home screen.");
                return View();
            }

            PasswordResetRequestViewModel passwordResetRequestVM = _passwordResetRequestService.GetPasswordResetRequestById(passwordResetRequestId);

            // Check against the validity of the user request (i.e. is it still in database and within the last 72 hours)
            if (passwordResetRequestVM == null || passwordResetRequestVM.Date <= DateTime.Now.AddHours(-72))
            {
                ModelState.AddModelError(string.Empty, "The link you clicked to reach this page is invalid, or it has expired. To get a new link, please click Forgotten Password on the Home screen.");
                return View();
            }
            //if username exist and is equal to the person requested then get that membership user to reset the password
            else if (string.IsNullOrEmpty(passwordResetRequestVM.UserName))
            {
                ModelState.AddModelError(string.Empty, "There is a problem with your account, you cannot set a new password.");
                return View();
            }
            else if (string.IsNullOrEmpty(model.NewPassword) || model.NewPassword.Length < 6)
            {
                ModelState.AddModelError(string.Empty, "Please enter a valid password, this must be at least 6 characters long.");
                return View();
            }
            else if (model.NewPassword != model.ConfirmPassword)
            {
                ModelState.AddModelError(string.Empty, "Your new password and confirmation of that password do not match.");
                return View();
            }
            else
            {
                //all validation checks have passed! Change password.
                MembershipUser user = Membership.GetUser(passwordResetRequestVM.UserName);

                // First reset the password for the user, then change it to the one requested by them. 
                user.ChangePassword(user.ResetPassword(), model.NewPassword);
                //ToDo: close or delete completed password reset request

                return RedirectToAction("ChangePasswordSuccess");
            }
        }

        // For the advisor eo set a new password for a patient when requested
        [HttpPost]
        [AllowAnonymousAttribute]
        public ActionResult SendSetPasswordEmailForPatient(PasswordResetRequestViewModel model, string returnUrl)
        {
            var email = Request.QueryString["email"];

            if (!string.IsNullOrEmpty(email))
            {
                var userName = Membership.GetUserNameByEmail(email);

                // If username exist get the membership user to reset the password
                if (userName != null)
                {
                    MembershipUser currentUser = Membership.GetUser(userName);
                    //if the advisor is resetting the password unlock the user account if locked
                    if (User.IsInRole("Advisor"))
                    {
                        if (currentUser.IsLockedOut)
                            currentUser.UnlockUser();
                    }
                    // write the user as the username and generate a guid with a time stamp (automatically done on save).
                    model.UserName = userName;

                    var modelRetrieved = _passwordResetRequestService.AddPasswordResetRequest(model);

                    if (model.Email.ToLower() == currentUser.Email.ToLower())
                    {
                        bool emailSsl;
                        Boolean.TryParse(ConfigurationManager.AppSettings["NonSecureSMTPSslEnabled"], out emailSsl);
                        // Send the email to the user for resetting the password
                        _emailService.SendEmail(currentUser.Email, "Request to Reset Password For Healthlines", EmailHelper.PasswordResetEmailBody(currentUser, modelRetrieved), "", emailSsl);

                        return RedirectToAction("ResetEmailSentForPatientSuccess", "Account", new { returnUrl = returnUrl, UserName = userName });
                    }
                }
            }
            return View();
        }

        [AllowAnonymousAttribute]
        public ActionResult ResetEmailSentForPatientSuccess()
        {
            return View();
        }

        [Authorize(Roles = "Administrator, Advisor")]
        public ActionResult ChangeEmailAddressForUser(string email, string returnUrl)
        {
            return View();
        }

        [Authorize(Roles = "Administrator, Advisor")]
        [HttpPost]
        public ActionResult ChangeEmailAddressForUser(string email, PatientEmailAddressViewModel patientEmailAddressVM, string returnUrl)
        {
            var existingEmail = patientEmailAddressVM.ExistingEmailAddress;
            var newEmail = patientEmailAddressVM.NewEmailAddress;

            if (!string.IsNullOrEmpty(existingEmail) && !string.IsNullOrEmpty(newEmail))
            {
                var userName = Membership.GetUserNameByEmail(existingEmail);

                // If username exist and same as their existing email address then the user can select CVD or Depression databases to set the new email address
                if (userName != null && email.ToLower() == existingEmail.ToLower())
                {
                    patientEmailAddressVM.ReturnURL = returnUrl;
                    TempData["patientObject"] = patientEmailAddressVM; // Pass the view model to TempData, so that the success screen can get it.

                    if (_membershipService.UpdateMembershipUserEmailAddress(patientEmailAddressVM, userName, LoginHelper.GetLoggedInUser()))
                    {
                        return RedirectToAction("ChangeEmailAddressForUserSuccess");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "A patient record with the same email address already exists in the database.");
                        return View();
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "The existing email address entered and patient's actual email address do not match.");
                    return View();
                }
            }
            return View();
        }

        [Authorize(Roles = "Administrator, Advisor")]
        [HttpGet]
        public ActionResult ChangeEmailAddressForUserSuccess()
        {
            PatientEmailAddressViewModel patientEmailAddressVM = (PatientEmailAddressViewModel)TempData["patientObject"];
            return View(patientEmailAddressVM);
        }

        #region Status Codes
        private static string ErrorCodeToString(MembershipCreateStatus createStatus)
        {
            // See http://go.microsoft.com/fwlink/?LinkID=177550 for
            // a full list of status codes.
            switch (createStatus)
            {
                case MembershipCreateStatus.DuplicateUserName:
                    return "User name already exists. Please enter a different user name.";

                case MembershipCreateStatus.DuplicateEmail:
                    return "A user name for that e-mail address already exists. Please enter a different e-mail address.";

                case MembershipCreateStatus.InvalidPassword:
                    return "The password provided is invalid. Please enter a valid password value.";

                case MembershipCreateStatus.InvalidEmail:
                    return "The e-mail address provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidAnswer:
                    return "The password retrieval answer provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidQuestion:
                    return "The password retrieval question provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidUserName:
                    return "The user name provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.ProviderError:
                    return "The authentication provider returned an error. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                case MembershipCreateStatus.UserRejected:
                    return "The user creation request has been canceled. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                default:
                    return "An unknown error occurred. Please verify your entry and try again. If the problem persists, please contact your system administrator.";
            }
        }
        #endregion
    }
}
