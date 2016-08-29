using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NHSD.ElephantParade.Web.Controllers;

namespace NHSD.ElephantParade.Web.Filters
{
    public sealed class LogonAuthorize : AuthorizeAttribute {
        public override void OnAuthorization(AuthorizationContext filterContext) 
        {
            bool skipAuthorization = filterContext.ActionDescriptor.IsDefined(typeof(AllowAnonymousAttribute), true)
            || filterContext.ActionDescriptor.ControllerDescriptor.IsDefined(typeof(AllowAnonymousAttribute), true);
            
            if (!skipAuthorization) 
            {
                base.OnAuthorization(filterContext);
            }
        }
    }
}