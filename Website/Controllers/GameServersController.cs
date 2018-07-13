using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Website.Authentication;

namespace Website.Controllers
{
    public class GameServersController : AKBaseController
    {
        public ActionResult Index()
        {
            var servers = DataRepositories.ConfigRepository.GetAll();

            return View(servers);
        }

        [CustomAuthorize(Roles = "SuperAdmin")]
        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }

        [CustomAuthorize(Roles = "SuperAdmin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(DataModels.Config server)
        {
            if (ModelState.IsValid)
            {
                server.Id = Guid.NewGuid();

                if (DataRepositories.ConfigRepository.Create(server))
                {
                    return RedirectToAction("Index");
                }
            }

            return View(server);
        }
    }
}