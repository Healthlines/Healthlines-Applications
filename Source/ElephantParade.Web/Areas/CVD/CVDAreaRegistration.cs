using System.Web.Mvc;

namespace NHSD.ElephantParade.Web.Areas.CVD
{
    public class CVDAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "CVD";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute("CVD_PatientView",
                "Patient/{studyid}/{patientid}/CVD/{controller}/{action}/{id}", // URL with parameters
                new { action = "Index", id = UrlParameter.Optional },
                new[] { "NHSD.ElephantParade.Web.Areas.CVD.Controllers" }
            );
            context.MapRoute(
                "CVD_default",
                "CVD/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
