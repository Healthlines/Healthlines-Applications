using System;
using NHSD.ElephantParade.Domain.Models;

namespace NHSD.ElephantParade.Core.Interfaces
{
    public interface IPasswordResetRequestService
    {
        PasswordResetRequestViewModel GetPasswordResetRequestById(Guid passwordresetRequestId);
        PasswordResetRequestViewModel GetPasswordResetRequestByUser(string userName);
        PasswordResetRequestViewModel AddPasswordResetRequest(Domain.Models.PasswordResetRequestViewModel passwordResetRequestVM);
    }
}