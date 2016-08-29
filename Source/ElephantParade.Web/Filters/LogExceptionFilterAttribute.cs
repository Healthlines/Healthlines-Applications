using System.Web.Mvc;
using Elmah;

namespace NHSD.ElephantParade.Web.Filters
{
    public class LogExceptionFilterAttribute : FilterAttribute, IExceptionFilter
    {
        public void OnException(ExceptionContext filterContext)
        {
            // ToDo: Log the exception here
            ErrorSignal.FromCurrentContext().Raise(filterContext.Exception);
        }
    }
}