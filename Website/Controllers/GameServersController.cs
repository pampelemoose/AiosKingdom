using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Website.Authentication;

namespace Website.Controllers
{
    public class GameServersController : Controller
    {
        public ActionResult Index()
        {
            var servers = DataRepositories.GameServerRepository.GetAll();

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
        public ActionResult Create(DataModels.GameServer server)
        {
            if (ModelState.IsValid)
            {
                bool haveErrors = false;
                if (string.IsNullOrEmpty(server.Name))
                {
                    ModelState.AddModelError("Name", "Must specify a name.");
                    haveErrors = true;
                }
                /*
                if (armor.ItemLevel < 1)
                {
                    ModelState.AddModelError("ItemLevel", "Must be > 0");
                    haveErrors = true;
                }

                if (armor.UseLevelRequired < 1)
                {
                    ModelState.AddModelError("UseLevelRequired", "Must be > 0");
                    haveErrors = true;
                }

                if (armor.ArmorValue < 1)
                {
                    ModelState.AddModelError("ArmorValue", "Must be > 0");
                    haveErrors = true;
                }
                */
                if (haveErrors) return View(server);

                server.Id = Guid.NewGuid();

                if (DataRepositories.GameServerRepository.Create(server))
                {
                    return RedirectToAction("Index");
                }
            }

            return View(server);
        }
    }
}