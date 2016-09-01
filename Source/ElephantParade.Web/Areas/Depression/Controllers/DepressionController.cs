using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NHSD.ElephantParade.Web.Controllers;

namespace NHSD.ElephantParade.Web.Areas.Depression.Controllers
{
    public class DepressionController : BaseController
    {
        //
        // GET: /Depression/Depression/

        public ActionResult Index()
        {
            return View();
        }

        
    }
}
