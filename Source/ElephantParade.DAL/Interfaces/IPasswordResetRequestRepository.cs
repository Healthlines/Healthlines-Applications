using System;
using NHSD.ElephantParade.DAL.EntityModels;

namespace NHSD.ElephantParade.DAL.Interfaces
{
    public interface IPasswordResetRequestRepository : IRepository<PasswordResetRequest>
    {
        PasswordResetRequest GetPasswordResetRequestById(Guid passwordResetRequestId);
        PasswordResetRequest GetPasswordResetRequestByName(string userName);
    }
}
