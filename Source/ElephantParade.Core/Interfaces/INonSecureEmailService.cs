using System.Net.Mail;
namespace NHSD.ElephantParade.Core.Interfaces
{
    public interface INonSecureEmailService
    {
        void SendEmail(string recipientEmailAddress, string subject, string body, string attachmentFileName, bool enableSslFlag);

        void SendEmail(string[] recipientEmailAddress, string subject, string body, bool isBodyHtml, Attachment[] attachments, bool enableSslFlag);
    }
}
