using System.Collections.Generic;
using System.Web.Security;
using NHSD.ElephantParade.Core.Models;
using NHSD.ElephantParade.Domain.Models;

namespace NHSD.ElephantParade.Core.Interfaces
{
    public interface IMembershipService
    {
        MembershipCreateStatus CreateMembershipUser(string email, string username, string password, string role);
        MembershipCreateStatus CreateMembershipUser(UserViewModel user, string provider = "AspNetSqlMembershipProvider");
        IList<UserViewModel> ExistingADUsers();
        void DeleteMembershipUserByUserName(string userName);
        void DeleteMembershipUserByEmail(string email);
        bool UserValidated(string userName, string password);
        string GeneratePassword(int length, int numberOfNonAlphanumericChars);
        string GetUserNameByEmail(string email);
        void CreateDepressionAccount(StudyPatient newPatient);
        void CreateCVDAccount(StudyPatient newPatient);
        bool UpdateMembershipUserEmailAddress(PatientEmailAddressViewModel patientEmailAddressVM, string userName, string loginUser);
    }
}
