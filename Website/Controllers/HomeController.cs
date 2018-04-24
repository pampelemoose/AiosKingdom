using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Website.Controllers
{
    public class HomeController : Controller
    {
        [Authorize]
        public ActionResult Index()
        {
            ViewBag.Message = "Your application description page.";

            List<DataModels.User> users = new List<DataModels.User>();

            using (var context = new DataRepositories.DispatchDbContext())
            {
                users = context.Users.ToList();
            }

            return View(users);
        }
    }
}