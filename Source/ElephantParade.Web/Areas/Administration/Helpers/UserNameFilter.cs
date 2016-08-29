using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NHSD.ElephantParade.Web.Areas.Administration.Helpers
{
    /// <summary>
    /// used to seperate httpcontext from controller to allow for controller unit testing
    /// </summary>
    public class UserNameFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            const string Key = "userName";

            if (filterContext.ActionParameters.ContainsKey(Key))
            {
                if (filterContext.HttpContext.User.Identity.IsAuthenticated)
                {
                    filterContext.ActionParameters[Key] = filterContext.HttpContext.User.Identity.Name;
                }
            }

            base.OnActionExecuting(filterContext);
        }
    }
}