using System.ComponentModel;

namespace NHSD.ElephantParade.Core.Models
{
    public class UserViewModel
    {
        [DisplayName("User name")]
        public string UserName { get; set; }
        [DisplayName("Email address")]
        public string EmailAddress { get; set; }
        [DisplayName("Is the user an administrator (can access administration screens)")]
        public bool Administrator { get; set; }
        [DisplayName("or a supervisor (can unlock locked records)")]
        public bool Supervisor { get; set; }
        public bool Advisor { get; set; }
        /// <summary>
        /// Any other roles not covered by booleans
        /// </summary>
        public string[] Roles { get; set; }

        public string Password { get; set; }
    }
}
