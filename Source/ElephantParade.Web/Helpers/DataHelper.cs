using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NHSD.ElephantParade.Core;
using NHSD.ElephantParade.Core.Interfaces;
using NHSD.ElephantParade.Core.Models;

namespace NHSD.ElephantParade.Web.Helpers
{
    public class DataHelper
    {
        public static void UpdatePageVisitedLog(PageVisitedLogViewModel pageVisitedLogViewModel, IPageVisitedLogService pageVisitedLogService)
        {
            pageVisitedLogService = pageVisitedLogService ?? new PageVisitedLogService();
            pageVisitedLogService.Add(pageVisitedLogViewModel);
        }
    }
}