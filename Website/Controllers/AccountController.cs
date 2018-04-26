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
    public class AccountController : Controller
    {
        // GET: Account
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Login(string returnUrl = "")
        {
            if (User.Identity.IsAuthenticated)
            {
                return LogOut();
            }

            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(Models.LoginView loginView, string returnUrl = "")
        {
            if (ModelState.IsValid)
            {
                if (Membership.ValidateUser(loginView.Username, loginView.Password))
                {
                    var user = (Authentication.CustomMembershipUser)Membership.GetUser(loginView.Username, false);
                    if (user != null)
                    {
                        Authentication.CustomSerializeModel userModel = new Authentication.CustomSerializeModel
                        {
                            Id = user.UserId,
                            Username = user.Username,
                            Email = user.Email,
                            Rolenames = user.Roles?.Select(r => r.Name).ToList()
                        };

                        string userData = JsonConvert.SerializeObject(userModel);
                        FormsAuthenticationTicket authTicket = new FormsAuthenticationTicket(
                            1, loginView.Username, DateTime.Now, DateTime.Now.AddMinutes(15), false, userData);

                        string enTicket = FormsAuthentication.Encrypt(authTicket);
                        HttpCookie faCookie = new HttpCookie("AiosKingdom_Auth", enTicket);
                        Response.Cookies.Add(faCookie);
                    }

                    if (Url.IsLocalUrl(returnUrl)) return Redirect(returnUrl);

                    return RedirectToAction("Index", "Home");
                }
            }

            return View(loginView);
        }

        [HttpGet]
        public ActionResult Registration()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Registration(Models.RegistrationView registrationView)
        {
            bool statusRegistration = false;
            string messageRegistration = string.Empty;

            if (ModelState.IsValid)
            {
                string username = Membership.GetUserNameByEmail(registrationView.Email);
                if (!string.IsNullOrEmpty(username))
                {
                    ModelState.AddModelError("Warning Email", "Sorry, this email is already used.");
                    return View(registrationView);
                }

                var role = DataRepositories.UserRepository.Roles.FirstOrDefault(r => r.Name.Equals("User"));
                var user = new DataModels.User
                {
                    Id = Guid.NewGuid(),
                    Username = registrationView.Username,
                    Password = DataModels.User.EncryptPassword(registrationView.Password),
                    Email = registrationView.Email,
                    ActivationCode = registrationView.ActivationCode,
                    IsActivated = false,
                    Roles = new List<DataModels.Role> { role }
                };

                if (DataRepositories.UserRepository.Create(user))
                {
                    VerificationEmail(registrationView.Email, registrationView.ActivationCode.ToString());
                    messageRegistration = "Your account has been created.";
                    statusRegistration = true;
                }
            }

            ViewBag.Message = messageRegistration;
            ViewBag.Status = statusRegistration;

            return View(registrationView);
        }

        [HttpGet]
        public ActionResult ActivationAccount(string id)
        {
            bool statusAccount = false;

            using (var context = new DataRepositories.DispatchDbContext())
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
            return RedirectToAction("Login", "Account", null);
        }

        [NonAction]
        public void VerificationEmail(string email, string activationCode)
        {
            var url = string.Format("/Account/ActivationAccount/{0}", activationCode);
            var link = Request.Url.AbsoluteUri.Replace(Request.Url.PathAndQuery, url);

            var fromEmail = new MailAddress("noreply@lemoosecorp.com", "Activation Account - AiosKindgom");
            var toEmail = new MailAddress(email);

            var fromEmailPassword = "Chouette002";
            string subject = "Activation Account !";

            string body = "<br/> Please click on the following link in order to activate your account" + "<br/><a href='" + link + "'> Activation Account ! </a>";

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

            smtp.Send(message);
        }
    }
}