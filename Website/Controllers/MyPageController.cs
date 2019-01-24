using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Website.Authentication;

namespace Website.Controllers
{
    [CustomAuthorize(Roles = "User")]
    public class MyPageController : AKBaseController
    {
        public ActionResult Index()
        {
            var user = DataRepositories.UserRepository.GetById(GetLoggedId());
            var tickets = DataRepositories.ForumRepository.GetTicketsForUserId(GetLoggedId());

            return View(new Models.MyPageModel { User = user, Tickets = tickets });
        }

        [HttpGet]
        public ActionResult ChangeUsername()
        {
            return View(new Models.ChangeUsernameView());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangeUsername(Models.ChangeUsernameView model)
        {
            if (ModelState.IsValid)
            {
                var user = DataRepositories.UserRepository.GetById(GetLoggedId());
                if (user.Password.Equals(DataModels.User.EncryptPassword(model.Password)))
                {
                    var usernameExists = DataRepositories.UserRepository.GetAll().FirstOrDefault(u => u.Username.Equals(model.Username));
                    if (usernameExists == null)
                    {
                        user.Username = model.Username;
                        if (DataRepositories.UserRepository.Update(user))
                        {
                            HttpCookie cookie = new HttpCookie("AiosKingdom_Auth", "");
                            cookie.Expires = DateTime.Now.AddYears(-1);
                            Response.Cookies.Add(cookie);

                            FormsAuthentication.SignOut();
                            return RedirectToAction("Index");
                        }
                        Alert(AlertMessage.AlertType.Danger, "Database error, please try again later.");
                        return View(model);
                    }

                    ModelState.AddModelError("Username", "Username already used.");
                    return View(model);
                }

                ModelState.AddModelError("Password", "Wrong password.");
            }

            return View(model);
        }

        [HttpGet]
        public ActionResult ResetPassword()
        {
            return View(new Models.ResetPasswordView());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ResetPassword(Models.ResetPasswordView model)
        {
            if (ModelState.IsValid)
            {
                var user = DataRepositories.UserRepository.GetById(GetLoggedId());
                if (user.Password.Equals(DataModels.User.EncryptPassword(model.OldPassword)))
                {
                    user.Password = model.Password;
                    if (DataRepositories.UserRepository.Update(user))
                    {
                        return RedirectToAction("Index");
                    }
                    Alert(AlertMessage.AlertType.Danger, "Database error, please try again later.");
                    return View(model);
                }

                ModelState.AddModelError("Password", "Wrong password.");
            }

            return View(model);
        }

        [HttpGet]
        public ActionResult NewTicket()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult NewTicket(Models.NewTicketView model)
        {
            if (ModelState.IsValid)
            {
                var user = DataRepositories.UserRepository.GetById(GetLoggedId());

                if (DataRepositories.ForumRepository.CreateTicket(new DataModels.Website.Ticket
                {
                    Category = DataModels.Website.Ticket.Topic.EmailChange,
                    CreatedAt = DateTime.Now,
                    CreatedBy = user.Id,
                    CreatedByUsername = user.Username,
                    IsActive = true,
                    IsOpen = true,
                    Comments = new List<DataModels.Website.Comment> { new DataModels.Website.Comment {
                        Content = model.Content,
                        CreatedAt = DateTime.Now,
                        CreatedBy = user.Id,
                        CreatedByUsername = user.Username,
                    } }
                }))
                    return RedirectToAction("Index");
            }

            return View(model);
        }

        [HttpGet]
        public ActionResult Ticket(int id)
        {
            var ticket = DataRepositories.ForumRepository.GetTicketsForUserId(GetLoggedId()).FirstOrDefault(t => t.Id == id);
            if (ticket != null)
            {
                return View(ticket);
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Ticket(int id, Models.CommentModel model)
        {
            var ticket = DataRepositories.ForumRepository.GetTicketsForUserId(GetLoggedId()).FirstOrDefault(t => t.Id == id);
            if (ticket != null)
            {
                if (DataRepositories.ForumRepository.CreateComment(new DataModels.Website.Comment
                {
                    Content = model.Content,
                    CreatedAt = DateTime.Now,
                    CreatedBy = GetLoggedId(),
                    CreatedByUsername = User.Identity.Name
                }, id, true))
                {
                    return RedirectToAction("Ticket", new { id = id });
                }

            }

            return RedirectToAction("Index");
        }
    }
}