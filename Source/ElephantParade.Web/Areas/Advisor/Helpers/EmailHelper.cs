using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NHSD.ElephantParade.Domain.Models;

namespace NHSD.ElephantParade.Web.Areas.Advisor.Helpers
{
    public class EmailHelper
    {

        public static string ParseText(string text, StudyPatient patient)
        {
            text = text.Replace("{GP.name}", patient.GPPractice.Name);
            string pname = (patient.Title + " " + patient.Forename + " " + patient.Surname).Trim();
            text = text.Replace("{patient.name}", pname);
            text = text.Replace("{patient.DOB}", patient.DOB.HasValue?patient.DOB.Value.ToShortDateString():"unknown");
            return text;
        }

    }
}