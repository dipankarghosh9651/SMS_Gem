using SMS_Gem;
using System;
using System.Web;
using System.Web.Http;
using System.Web.Routing;
using System.Web.SessionState;

namespace SMS_GEM
{
    public class Global : HttpApplication
    {
        void Application_Start(object sender, EventArgs e)
        {
            // 1. Register Web API
            GlobalConfiguration.Configure(WebApiConfig.Register);

            // 2. Register standard routes if RouteConfig exists
            RegisterAppRoutes(RouteTable.Routes);
        }

        private void RegisterAppRoutes(RouteCollection routes)
        {
            routes.Ignore("{resource}.axd/{*pathInfo}");
        }

        protected void Application_PostAuthorizeRequest()
        {
            if (IsWebApiRequest())
            {
                HttpContext.Current.SetSessionStateBehavior(SessionStateBehavior.Required);
            }
        }

        private bool IsWebApiRequest()
        {
            return HttpContext.Current.Request.AppRelativeCurrentExecutionFilePath.StartsWith("~/api");
        }
    }
}