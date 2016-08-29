using System.Linq;
using NHSD.ElephantParade.DAL.EntityModels;
using NHSD.ElephantParade.DAL.Infrastructure;
using NHSD.ElephantParade.DAL.Interfaces;
using System;

namespace NHSD.ElephantParade.DAL
{
    public class PasswordResetRequestRepository : RepositoryBase<PasswordResetRequest>, IPasswordResetRequestRepository
    {
        public PasswordResetRequestRepository() : this(new HealthlinesRepositoryContext()) { }

        public PasswordResetRequestRepository(IRepositoryContext repositoryContext) : base(repositoryContext) { }

        // Get the latest record from the id
        public PasswordResetRequest GetPasswordResetRequestById(Guid passwordResetRequestId)
        {
            return ObjectSet.Where(p => p.PasswordResetRequestId == passwordResetRequestId
                                        ).ToList().FirstOrDefault();
        }

        // Get the latest record from the name
        public PasswordResetRequest GetPasswordResetRequestByName(string userName)
        {
            return ObjectSet.Where(p => p.UserName == userName).OrderByDescending(
                                        p => p.DateOfRequest).ToList().FirstOrDefault();
        }
    }
}
