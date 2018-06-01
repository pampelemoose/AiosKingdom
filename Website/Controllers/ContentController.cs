using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Website.Controllers
{
    public class ContentController : Controller
    {
        [ChildActionOnly]
        public ActionResult GetSection(string action, string controller, bool before = false)
        {
            var section = DataRepositories.SectionRepository.GetForPage(action, controller, before);
            return PartialView("SectionPartial", section);
        }
    }
}