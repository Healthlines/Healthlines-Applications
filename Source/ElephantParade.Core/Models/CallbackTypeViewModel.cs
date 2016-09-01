using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.ComponentModel;
using NHSD.ElephantParade.Core.Interfaces;
using System.Data.Entity;
using System.Web.Mvc;

namespace NHSD.ElephantParade.Core.Models
{
    [Description("CallbackType details.")]
    public class CallbackTypeViewModel//:ICallbackTypeModel
    {
        public Guid CallbackTypeId { get; set; }

        [Display(Name = "Callback Type")]
        public string CallbackConfirmation { get; set; }



    }
}
