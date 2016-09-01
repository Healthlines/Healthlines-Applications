using NHSD.ElephantParade.Core.Interfaces;

namespace NHSD.ElephantParade.Core
{
    public class NonSecureEmailService : EmailService, INonSecureEmailService
    {
        public NonSecureEmailService(string host, int port, string senderAccountUsername, string senderAccountPassword, string defaultSenderEmailAddress)
            : base(host, port, senderAccountUsername, senderAccountPassword, defaultSenderEmailAddress)
        { }
    }
}
