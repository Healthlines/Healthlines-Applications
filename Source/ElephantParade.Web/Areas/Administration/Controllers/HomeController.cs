using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NHSD.ElephantParade.Web.Areas.Administration.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /Administration/Home/
        [Authorize(Roles = "Administrator")]
        public ActionResult Index()
        {
            return View();
        }

        [Authorize(Roles = "Administrator")]
        public ActionResult AdvisorHome()
        {
            return RedirectToRoute("AdvisorHome");
        }
    }
}
