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
    [Description("CallOutcomeReason details")]
    public class CallOutcomeReasonViewModel//: ICallOutcomeReasonModel 
    {
        public Guid OutcomeId { get; set; }

        [Display(Name="CallOutcome Reason")]
        public string Reason { get; set; }
    }
}

