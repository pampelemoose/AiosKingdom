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
        public ActionResult Index(Models.Filters.UserFilter filter)
        {
            var users = DataRepositories.UserRepository.GetAll();

            filter.Users = filter.FilterList(users);

            return View(filter);
        }

        [HttpGet]
        public ActionResult Edit(Guid id)
        {
            var user = DataRepositories.UserRepository.GetById(id);

            var model = new Models.UserModel
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                Roles = user.Roles
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Models.UserModel userModel)
        {
            if (ModelState.IsValid)
            {
                var user = DataRepositories.UserRepository.GetById(userModel.Id);

                user.Username = userModel.Username;
                user.Email = userModel.Email;
                if (userModel.Roles != null && userModel.Roles.Count > 0)
                {
                    user.Roles = userModel.Roles;
                }

                if (DataRepositories.UserRepository.Update(user))
                {
                    return RedirectToAction("Index");
                }
            }

            return View(userModel);
        }
    }
}