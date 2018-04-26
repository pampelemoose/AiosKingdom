using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Website.Authentication;

namespace Website.Controllers
{
    [CustomAuthorize(Roles = "SuperAdmin")]
    public class UserController : Controller
    {
        // GET: User
        public ActionResult Index(Models.UserFilter filter)
        {
            var users = DataRepositories.UserRepository.GetAll();

            filter.Users = filter.FilterList(users);

            return View(filter);
        }
    }
}