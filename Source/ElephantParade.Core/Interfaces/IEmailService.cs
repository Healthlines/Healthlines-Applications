using System.Net.Mail;
namespace NHSD.ElephantParade.Core.Interfaces
{
    public interface IEmailService
    {
        void SendEmail(string recipientEmailAddress, string subject, string body, string attachmentFileName, bool enableSslFlag);

        void SendEmail(string[] recipientEmailAddress, string subject, string body, bool isBodyHtml, Attachment[] attachments, bool enableSslFlag);

        void setEmailEnableSsl(bool enableSslFlag);
    }
}
