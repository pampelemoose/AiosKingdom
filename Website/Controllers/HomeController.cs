﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Website.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            List<DataModels.User> users = new List<DataModels.User>();

            using (var context = new DataRepositories.DispatchDbContext())
            {
                users = context.Users.ToList();
            }

            return View(users);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}