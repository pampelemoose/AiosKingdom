using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

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
    }
}