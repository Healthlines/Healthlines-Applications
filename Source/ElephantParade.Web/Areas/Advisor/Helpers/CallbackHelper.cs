using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using NHSD.ElephantParade.Core.Interfaces;
using NHSD.ElephantParade.Domain.Models;

namespace NHSD.ElephantParade.Web.Areas.Advisor.Helpers
{
    public static class CallbackHelper
    {
        /// <summary>
        /// Returns a list of dates to be disabled (prevented from being selected) for Patient Callback.
        /// </summary>
        /// <returns>String of comma separated dates to be disabled.</returns>
        public static string GetDisabledDates(string studyId, IContentSectionStatusService contentSectionStatusService)
        {
            //pass null to method, as we're not restricting it by studyId.
            var disabledDates = contentSectionStatusService.SectionDisabledDates(ContentSectionTypes.PatientCallback, null);

            //convert date list to string.
            StringBuilder dateList = new StringBuilder();

            foreach (DateTime date in disabledDates)
            {
                //ensure same date format as jquery datepicker uses.
                dateList.Append(date.ToString("dd/MM/yyyy"));
                //insert spaces between dates, so we can split in javascript.
                dateList.Append(",");
            }

            //remove space at the end of this string.
            return dateList.ToString().TrimEnd(',');
        }
    }
}