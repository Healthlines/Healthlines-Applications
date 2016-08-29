using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace NHSD.ElephantParade.Domain.Models
{
    public class PasswordResetRequestViewModel
    {
        public Guid PasswordResetRequestId { get; set; }

        [Required(ErrorMessage = "Email address required")]
        [Display(Name = "Email address")]
        [RegularExpression(@"[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,4}", ErrorMessage = "Please enter a valid email address")]
        public string Email { get; set; }

        public string UserName { get; set; }
        public DateTime Date { get; set; }
    }
}
