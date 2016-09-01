using System.Web.Mvc;

namespace NHSD.ElephantParade.Web.Areas.Advisor
{
    public class AdvisorAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Advisor";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            //explicit route for download, to use when we don't have a callback ID
            //callback ID is not used in this method anyway.
            context.MapRoute("QuestionnaireNoCallback",
                "Advisor/QuestionnaireSession/Download/{id}",
                new { controller = "QuestionnaireSession", action = "Download", id = UrlParameter.Optional },
                new[] { "NHSD.ElephantParade.Web.Areas.Advisor.Controllers" }
            );

            context.MapRoute("CallbackQuestionnaire",
                "Advisor/QuestionnaireSession/{callbackID}/{action}/{id}",
                new { controller = "QuestionnaireSession", action = "Index", id = UrlParameter.Optional },
                new[] { "NHSD.ElephantParade.Web.Areas.Advisor.Controllers" }
            ); 
            
            context.MapRoute(
                "Advisor_default",
                "Advisor/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional },
                new[] { "NHSD.ElephantParade.Web.Areas.Advisor.Controllers" }
            );

        }
    }
}
