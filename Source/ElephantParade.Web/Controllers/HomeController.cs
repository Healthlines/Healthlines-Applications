using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace NHSD.ElephantParade.Web.Controllers
{
    public class HomeController : BaseController
    {
        //public ActionResult Index()
        //{
        //    ViewBag.Message = "";            
        //    return View();
        //}

        public ActionResult Index()
        {
            return RedirectToAction("LogOn", "Account");
        }

        public ActionResult About()
        {
            return View();
//            return Redirect("http://www.bristol.ac.uk/healthlines/");
        }

        public ActionResult AboutHealthlinesStudy()
        {
            return PartialView("_AboutHealthlinesStudy");
        }

        public ActionResult AboutNHSDirect()
        {
            return PartialView("_AboutNHSDirect");
        }

        [AllowAnonymous]
        [HttpGet]
        public ActionResult UrgentHelpCVD()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpGet]
        public ActionResult UrgentHelpDepression()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpGet]
        public ActionResult TechnicalHelp()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpGet]        
        public ActionResult Privacy()
        {          
            return View();
        }

        [AllowAnonymous]
        [HttpGet]
        public ActionResult Terms()
        {
            
            return View();
        }
    }
}
