using System;
using NHSD.ElephantParade.Core.Interfaces;
using NHSD.ElephantParade.Domain.Models;
using NHSD.ElephantParade.Core.Mapping;
using NHSD.ElephantParade.DAL.Interfaces;
using NHSD.ElephantParade.DAL;

namespace NHSD.ElephantParade.Core
{
    public class PasswordResetRequestService : IPasswordResetRequestService
    {
        private IPasswordResetRequestRepository _passwordResetRequestRepository;

        public PasswordResetRequestService() :
            this(new PasswordResetRequestRepository()) { }

        public PasswordResetRequestService(IPasswordResetRequestRepository passwordResetRequestRepository)
        {
            _passwordResetRequestRepository = passwordResetRequestRepository ?? new PasswordResetRequestRepository();
        }

        public PasswordResetRequestViewModel GetPasswordResetRequestById(Guid passwordresetRequestId)
        {
            ConvertViewModelObjects cvmo = new ConvertViewModelObjects();

            //var scheduledCallbacks = _callbackRepository.GetAll(cb => cb.CallbackId == callbackID).ToList();
            var queryable = _passwordResetRequestRepository.GetQueryable();

            var passwordResetRequest = _passwordResetRequestRepository.GetPasswordResetRequestById(passwordresetRequestId);

            return (cvmo.ConvertFromPasswordResetRequest(passwordResetRequest));
        }

        public PasswordResetRequestViewModel GetPasswordResetRequestByUser(string userName)
        {
            ConvertViewModelObjects cvmo = new ConvertViewModelObjects();

            //var scheduledCallbacks = _callbackRepository.GetAll(cb => cb.CallbackId == callbackID).ToList();
            var queryable = _passwordResetRequestRepository.GetQueryable();

            var passwordResetRequest = _passwordResetRequestRepository.GetPasswordResetRequestByName(userName);

            return (cvmo.ConvertFromPasswordResetRequest(passwordResetRequest));
        }

        public PasswordResetRequestViewModel AddPasswordResetRequest(Domain.Models.PasswordResetRequestViewModel passwordResetRequestVM)
        {
            ConvertViewModelObjects cvmo = new ConvertViewModelObjects();

            var passwordResetRequestEntity = cvmo.ConvertFromPasswordResetRequestVM(passwordResetRequestVM);

            _passwordResetRequestRepository.Add(passwordResetRequestEntity);
            _passwordResetRequestRepository.SaveChanges();

            // Get the saved details and returning back to the service and web levels for use to send user with link in an email
            var passwordResetRequest = _passwordResetRequestRepository.GetPasswordResetRequestByName(passwordResetRequestVM.UserName);

            return cvmo.ConvertFromPasswordResetRequest(passwordResetRequest);
        }
    }
}
