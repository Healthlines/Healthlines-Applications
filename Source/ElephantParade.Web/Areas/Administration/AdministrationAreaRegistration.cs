using System.Web.Mvc;

namespace NHSD.ElephantParade.Web.Areas.Administration
{
    public class AdministrationAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Administration";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "Administration_default",
                "Administration/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );

            context.MapRoute(
                "StudyPatientDataUpload", // Route name
                "Administration/Administration/StudyPatientDataUpload/" // URL with parameters
            );
        }
    }
}
