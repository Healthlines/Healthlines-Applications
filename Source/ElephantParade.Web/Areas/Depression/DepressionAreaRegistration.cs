using System.Web.Mvc;

namespace NHSD.ElephantParade.Web.Areas.Depression
{
    public class DepressionAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Depression";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "Depression_default",
                "Depression/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
            
        }
    }
}
