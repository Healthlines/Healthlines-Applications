using System.Net;
using System.Net.Mail;
using NHSD.ElephantParade.Core.Interfaces;

namespace NHSD.ElephantParade.Core
{
    //Some methods below taken originally from NHSD.Wolfpack - and changed to fit in this project.
    //Refer back if necessary.
    /// <summary>
    /// 
    /// </summary>
    public class EmailService : IEmailService
    {       
        protected string _host;
        protected int _port;
        protected string _senderEmailAddress;
        protected string _senderAccountUsername;
        protected string _senderAccountPassword;
        protected SmtpClient mailClient;
        protected MailMessage email;

        public EmailService()
        {
        }

        public EmailService(string host, int port, string senderAccountUsername, string senderAccountPassword, string defaultSenderEmailAddress)
        {
            _host = host;
            _port = port;

            _senderAccountUsername = senderAccountUsername;
            _senderAccountPassword = senderAccountPassword;
            _senderEmailAddress = defaultSenderEmailAddress;
        }

        public void setEmailEnableSsl(bool enableSslFlag)
        {
            mailClient.EnableSsl = enableSslFlag;
        }

        public void SendEmail(string recipientEmailAddress, string subject, string body, string attachmentFilepath, bool enableSslFlag)
        {
            using (mailClient = new SmtpClient())
            {
                using (email = new MailMessage())
                {
                    //copy data to email object
                    email.To.Add(recipientEmailAddress);
                    email.Subject = subject;
                    email.IsBodyHtml = true;
                    email.Body = body;
                    email.From = new MailAddress(_senderEmailAddress, "Healthlines Service Notification");
                    if (!string.IsNullOrEmpty(attachmentFilepath))
                        email.Attachments.Add(new Attachment(attachmentFilepath));

                    //setup mail client to send via host
                    NetworkCredential basicAuthenticationInfo = new NetworkCredential(_senderAccountUsername, _senderAccountPassword);
                    mailClient.Host = _host;
                    setEmailEnableSsl(enableSslFlag);
                    mailClient.Port = _port;
                    mailClient.UseDefaultCredentials = false;
                    mailClient.Credentials = basicAuthenticationInfo;
                    mailClient.Send(email);
                }
            }
        }

        public void SendEmail(string[] recipientEmailAddress, string subject, string body, bool isBodyHtml, Attachment[] attachments, bool enableSslFlag)
        {
            using (mailClient = new SmtpClient())
            {
                using (MailMessage email = new MailMessage())
                {
                    //copy data to email object
                    foreach (var item in recipientEmailAddress)
                    {
                        email.Bcc.Add(item);
                    }
                    
                    email.Subject = subject;
                    email.Body = body;
                    email.IsBodyHtml = isBodyHtml;
                    email.From = new MailAddress(_senderEmailAddress, "Healthlines Service Notification");
                    if(attachments!=null)
                        foreach (var item in attachments)
                        {
                            email.Attachments.Add(item);
                        }                        

                    //setup mail client to send via host
                    NetworkCredential basicAuthenticationInfo = new NetworkCredential(_senderAccountUsername, _senderAccountPassword);
                    mailClient.Host = _host;
                    setEmailEnableSsl(enableSslFlag);
                    mailClient.Port = _port;
                    mailClient.UseDefaultCredentials = false;
                    mailClient.Credentials = basicAuthenticationInfo;
                    mailClient.Send(email);
                }
            }
        }
    }
}
