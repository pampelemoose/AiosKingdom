using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace Website.Controllers
{
    public class AccountController : AKBaseController
    {
        // GET: Account
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(Models.LoginView loginView)
        {
            if (ModelState.IsValid)
            {
                if (Membership.ValidateUser(loginView.LogUsername, loginView.LogPassword))
                {
                    var user = (Authentication.CustomMembershipUser)Membership.GetUser(loginView.LogUsername, false);
                    if (user != null)
                    {
                        Authentication.CustomSerializeModel userModel = new Authentication.CustomSerializeModel
                        {
                            Id = user.UserId,
                            Username = user.Username,
                            Email = user.Email,
                            Rolenames = user.Roles
                        };

                        string userData = JsonConvert.SerializeObject(userModel);
                        FormsAuthenticationTicket authTicket = new FormsAuthenticationTicket(
                            1, loginView.LogUsername, DateTime.Now, DateTime.Now.AddMinutes(15), false, userData);

                        string enTicket = FormsAuthentication.Encrypt(authTicket);
                        HttpCookie faCookie = new HttpCookie("AiosKingdom_Auth", enTicket);
                        Response.Cookies.Add(faCookie);
                    }

                    Response.StatusCode = (int)HttpStatusCode.OK;
                    return Json(Url.Action("Index", "Home"));
                }
            }

            Response.StatusCode = (int)HttpStatusCode.BadRequest;
            return PartialView("LoginPartial", loginView);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PreAlphaRegistration(Models.PreAlphaRegistrationView registrationView)
        {
            if (ModelState.IsValid)
            {
                string username = Membership.GetUserNameByEmail(registrationView.Email);
                if (!string.IsNullOrEmpty(username))
                {
                    ModelState.AddModelError("Email", "Sorry, this email is already registered.");
                    Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    return PartialView("PreAlphaRegistrationPartial", registrationView);
                }

                var user = new DataModels.User
                {
                    Id = Guid.NewGuid(),
                    Username = Guid.NewGuid().ToString(),
                    Password = "dummyPassword",
                    Email = registrationView.Email,
                    ActivationCode = Guid.NewGuid(),
                    IsActivated = false,
                    Roles = new List<string> { "User" }
                };

                if (DataRepositories.UserRepository.Create(user))
                {
                    PreAlphaEmail(registrationView.Email);

                    Alert(AlertMessage.AlertType.Success, $"Account created.", "Success !");
                    Response.StatusCode = (int)HttpStatusCode.OK;
                    return PartialView("PreAlphaRegistrationSuccessfulPartial");
                }
            }

            Response.StatusCode = (int)HttpStatusCode.BadRequest;
            return PartialView("PreAlphaRegistrationPartial", registrationView);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Registration(Models.RegistrationView registrationView)
        {
            if (ModelState.IsValid)
            {
                string username = Membership.GetUserNameByEmail(registrationView.Email);
                if (!string.IsNullOrEmpty(username))
                {
                    ModelState.AddModelError("Email", "Sorry, this email is already used.");
                    Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    return PartialView("RegistrationPartial", registrationView);
                }
                var usernameExists = DataRepositories.UserRepository.GetAll().FirstOrDefault(u => u.Username.Equals(registrationView.Username));
                if (usernameExists != null)
                {
                    ModelState.AddModelError("Username", "Sorry, this username is already used.");
                    Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    return PartialView("RegistrationPartial", registrationView);
                }

                var user = new DataModels.User
                {
                    Id = Guid.NewGuid(),
                    Username = registrationView.Username,
                    Password = DataModels.User.EncryptPassword(registrationView.Password),
                    Email = registrationView.Email,
                    ActivationCode = registrationView.ActivationCode,
                    IsActivated = false,
                    Roles = new List<string> { "User" }
                };

                if (DataRepositories.UserRepository.Create(user))
                {
                    VerificationEmail(registrationView.Email, registrationView.ActivationCode.ToString());

                    Alert(AlertMessage.AlertType.Success, $"Account created.", "Success !");
                    Response.StatusCode = (int)HttpStatusCode.OK;
                    return PartialView("RegistrationSuccessfulPartial");
                }
            }

            Response.StatusCode = (int)HttpStatusCode.BadRequest;
            return PartialView("RegistrationPartial", registrationView);
        }

        [HttpGet]
        public ActionResult ActivationAccount(string id)
        {
            bool statusAccount = false;

            using (var context = new DataRepositories.AiosKingdomContext())
            {
                Guid aId = Guid.Parse(id);
                var user = context.Users.FirstOrDefault(u => u.ActivationCode.Equals(aId));
                if (user != null)
                {
                    if (!user.IsActivated)
                    {
                        user.IsActivated = true;
                        context.SaveChanges();
                        statusAccount = true;
                    }
                    else
                    {
                        ViewBag.Message = "Already activated..";
                    }
                }
                else
                    ViewBag.Message = "Wrong activation code.";
            }
            ViewBag.Status = statusAccount;

            return View();
        }

        public ActionResult LogOut()
        {
            HttpCookie cookie = new HttpCookie("AiosKingdom_Auth", "");
            cookie.Expires = DateTime.Now.AddYears(-1);
            Response.Cookies.Add(cookie);

            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Home", null);
        }

        [NonAction]
        public void PreAlphaEmail(string email)
        {
            var fromEmail = new MailAddress("noreply@lemoosecorp.com", "[Aios Kingdom] Pre Alpha Registration !");
            var toEmail = new MailAddress(email);

            var fromEmailPassword = "Chouette002";
            string subject = "[Aios Kingdom] Pre Alpha Registration !";

            string body = RenderPartialViewToString(this, "PreAlphaRegistrationEmail", null);

            var smtp = new SmtpClient
            {
                Host = "ssl0.ovh.net",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromEmail.Address, fromEmailPassword)
            };

            using (var message = new MailMessage(fromEmail, toEmail)
            {
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            })
            {
                try
                {
                    smtp.Send(message);
                }
                catch
                {
                }
            }
        }

        [NonAction]
        public void VerificationEmail(string email, string activationCode)
        {
            var url = string.Format("/Account/ActivationAccount/{0}", activationCode);
            var link = Request.Url.AbsoluteUri.Replace(Request.Url.PathAndQuery, url);

            var fromEmail = new MailAddress("noreply@lemoosecorp.com", "Activation Account - AiosKindgom");
            var toEmail = new MailAddress(email);

            var fromEmailPassword = "Chouette002";
            string subject = "[Aios Kingdom] Activate your Account !";

            string body = RenderPartialViewToString(this, "RegistrationEmail", link);

            var smtp = new SmtpClient
            {
                Host = "ssl0.ovh.net",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromEmail.Address, fromEmailPassword)
            };

            using (var message = new MailMessage(fromEmail, toEmail)
            {
                Subject = subject,
                Body = body,
                IsBodyHtml = true

            })
            {
                try
                {
                    smtp.Send(message);
                }
                catch
                {
                }
            }
        }
    }
}