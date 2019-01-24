using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Web.UI;

namespace Website.Controllers
{
    public abstract class AKBaseController : Controller
    {
        public struct AlertMessage
        {
            public enum AlertType
            {
                Success,
                Info,
                Warning,
                Danger
            }

            public AlertType Type { get; set; }
            public string Title { get; set; }
            public string Message { get; set; }
        }

        protected void Alert(AlertMessage.AlertType type, string message, string title = "")
        {
            if (ViewBag.Alerts == null)
            {
                ViewBag.Alerts = new List<AlertMessage>();
            }

            ViewBag.Alerts.Add(new AlertMessage
            {
                Type = type,
                Title = title,
                Message = message
            });
        }

        protected Guid GetLoggedId()
        {
            var authCookie = Request.Cookies.Get("AiosKingdom_Auth");
            var auth = FormsAuthentication.Decrypt(authCookie.Value);
            var model = JsonConvert.DeserializeObject<Authentication.CustomSerializeModel>(auth.UserData);

            return model.Id;
        }

        protected string RenderPartialViewToString(Controller controller, string viewName, object model)
        {
            if (string.IsNullOrEmpty(viewName))
            {
                viewName = controller.ControllerContext.RouteData.GetRequiredString("action");
            }

            controller.ViewData.Model = model;

            using (var sw = new StringWriter())
            {
                // Find the partial view by its name and the current controller context.
                ViewEngineResult viewResult = ViewEngines.Engines.FindPartialView(controller.ControllerContext, viewName);

                // Create a view context.
                var viewContext = new ViewContext(controller.ControllerContext, viewResult.View, controller.ViewData, controller.TempData, sw);

                // Render the view using the StringWriter object.
                viewResult.View.Render(viewContext, sw);

                return sw.GetStringBuilder().ToString();
            }
        }
    }
}