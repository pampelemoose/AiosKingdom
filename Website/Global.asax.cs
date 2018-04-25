using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Security;

namespace Website
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        protected void Application_PostAuthenticateRequest(object sender, EventArgs e)
        {
            HttpCookie authCookie = Request.Cookies["AiosKingdom_Auth"];
            if (authCookie != null)
            {
                FormsAuthenticationTicket authTicket = FormsAuthentication.Decrypt(authCookie.Value);

                var serializeModel = JsonConvert.DeserializeObject<Authentication.CustomSerializeModel>(authTicket.UserData);

                Authentication.CustomPrincipal principal = new Authentication.CustomPrincipal(authTicket.Name);

                principal.Id = serializeModel.Id;
                principal.Username = serializeModel.Username;
                principal.Email = serializeModel.Email;
                principal.Roles = serializeModel.Rolenames?.ToArray();

                HttpContext.Current.User = principal;
            }
        }
    }
}
