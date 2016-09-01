using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.ComponentModel;
using NHSD.ElephantParade.Core.Interfaces;
using System.Data.Entity;
using System.Web.Mvc;
using NHSD.ElephantParade.Domain.Models;

namespace NHSD.ElephantParade.Core.Models
{
    [Description("PatientComments  details.")]
    public class PatientCallCommentViewModel
        : CallEventViewModel
    {

        public PatientCallCommentViewModel()
        {
            base.EventCode = "Comment";
        }

        public int CommentId { get; set; }

        public CallbackViewModel Callback { get; set; }

        public Guid? CallbackId { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Comment Required")]
        [Display(Name="Comments")]
        public override string Text
        {
            get
            {
                return base.Text;
            }
            set
            {
                base.Text = value;
            }
        }

        
        [Display(Name="Comments Date")]
        public override DateTime Date
        {
            get
            {
                return base.Date;
            }
            set
            {
                base.Date = value;
            }
        }

        [Display(Name = "Entered By")]
        public override string UserID
        {
            get
            {
                return base.UserID;
            }
            set
            {
                base.UserID = value;
            }
        }
    }
}

