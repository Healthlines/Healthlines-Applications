using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NHSD.ElephantParade.Web.Areas.Advisor.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /Advisor/Home/

        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Help()
        {
            return View();
        }
        [Authorize(Roles = "Administrator,Advisor")]
        public ActionResult Resources()
        {
            return View();
        }
    }
}
