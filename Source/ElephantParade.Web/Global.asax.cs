using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using System.Security.Principal;
using Autofac;
using Autofac.Integration.Mvc;
using NHSD.ElephantParade.Core.Models;
using NHSD.ElephantParade.Web.Filters;
using NHSD.ElephantParade.Web.Authentication;
using NHSD.ElephantParade.Web.Helpers;
using System.Configuration;




namespace NHSD.ElephantParade.Web
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new LogExceptionFilterAttribute());
            //filters.Add(new HandleErrorAttribute());
            filters.Add(new HandleErrorAttribute { View = "~/Views/Shared/Error" });
            if(ConfigurationManager.AppSettings["EnsureSSL"]=="true")
                filters.Add(new RequireHttpsAttribute());
            filters.Add(new LogonAuthorize());
            filters.Add(new HandleErrorAttribute());
        }

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.IgnoreRoute("elmah.axd");

            routes.MapRoute("PatientView",
                "Patient/{studyid}/{patientid}/{controller}/{action}/{id}", // URL with parameters
                new { controller = "Patient", action = "Index", id = UrlParameter.Optional },
                new[] { "NHSD.ElephantParade.Web.Controllers" }
            );

            routes.MapRoute(
                "Default", // Route name
                "{controller}/{action}/{id}", // URL with parameters
                new { controller = "Home", action = "Index", id = UrlParameter.Optional }, // Parameter defaults
                new[] { "NHSD.ElephantParade.Web.Controllers" }
            );

            //This is route for Displaying the details of Single user based on link clicked in Patient details
            routes.MapRoute(
                  "PatientList", // Route name
                  "Advisor/Callback/PatientList/" // URL with parameters

            );

            //route on main advisor menu, to direct to patient search
            routes.MapRoute(
                  "PatientSearch", // Route name
                  "Advisor/Search/PatientSearch/" // URL with parameters
            );

            //route to redirect back to advisor menu, from administrator menu
            routes.MapRoute(
                  "AdvisorHome", // Route name
                  "Advisor/Home/Index/" // URL with parameters
            );
        }

        public override void Init()
        {
            this.PostAuthenticateRequest += this.PostAuthenticateRequestHandler;
            
            base.Init();
        }

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);

            //configure dependency with autofac
            var builder = new ContainerBuilder();
            builder.RegisterControllers(typeof(MvcApplication).Assembly);
            //add forms auth 
            builder.RegisterType<DefaultFormsAuthentication>().As<IFormsAuthentication>();
            //add callback service 
            builder.RegisterModule(new PatientCallbackModule());
            
            
            var container = builder.Build();
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
            
        }

        private void PostAuthenticateRequestHandler(object sender, EventArgs e)
        {
            var formsAuthentication = DependencyResolver.Current.GetService<IFormsAuthentication>();
            try
            {
                HttpCookie authCookie = this.Context.Request.Cookies[FormsAuthentication.FormsCookieName];
                if (IsValidAuthCookie(authCookie))
                {
                    var ticket = formsAuthentication.Decrypt(authCookie.Value);
                    var userIdentity = new HealthLinesParticipantIdentity(ticket);
                    this.Context.User = new GenericPrincipal(userIdentity, Roles.GetRolesForUser());

                    // Reset cookie for a sliding expiration.
                    formsAuthentication.SetAuthCookie(this.Context, ticket);

                    ////log user's page hit in db table
                    string extension = this.Context.Request.CurrentExecutionFilePathExtension;
                    string path = this.Context.Request.CurrentExecutionFilePath;

                    if (extension == string.Empty && path != "/")
                    {
                        PageVisitedLogViewModel pageVisitedLogViewModel = new PageVisitedLogViewModel
                        {
                            DateVisited = DateTime.Now,
                            IPAddress = this.Context.Request.UserHostAddress,
                            PageURL = this.Context.Request.RawUrl,
                            Username = this.Context.User.Identity.Name
                        };

                        DataHelper.UpdatePageVisitedLog(pageVisitedLogViewModel, null);
                    }
                }                
            }
            catch (Exception)
            {                
                //do nothing
                formsAuthentication.Signout();
            }
        }

        private static bool IsValidAuthCookie(HttpCookie authCookie)
        {
            return authCookie != null && !String.IsNullOrEmpty(authCookie.Value);
        }

        private void RegisterDependencys()
        {

        }
    }
}