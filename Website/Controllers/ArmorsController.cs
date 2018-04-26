using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Website.Authentication;

namespace Website.Controllers
{
    public class ArmorsController : Controller
    {
        public ActionResult Index(Models.ArmorFilter filter)
        {
            var armors = DataRepositories.ArmorRepository.GetAll();

            filter.Armors = filter.FilterList(armors);

            return View(filter);
        }

        [CustomAuthorize(Roles = "SuperAdmin")]
        [HttpGet]
        public ActionResult Create()
        {
            var armor = new DataModels.Items.Armor();
            armor.Stats = new List<DataModels.Items.ItemStat>();

            foreach (DataModels.Soul.Stats en in Enum.GetValues(typeof(DataModels.Soul.Stats)))
            {
                armor.Stats.Add(new DataModels.Items.ItemStat
                {
                    Type = en
                });
            }

            return View(armor);
        }

        [CustomAuthorize(Roles = "SuperAdmin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(DataModels.Items.Armor armor)
        {
            if (ModelState.IsValid)
            {
                bool haveErrors = false;
                if (string.IsNullOrEmpty(armor.Name))
                {
                    ModelState.AddModelError("Name", "Must specify a name.");
                    haveErrors = true;
                }

                if (string.IsNullOrEmpty(armor.Description))
                {
                    ModelState.AddModelError("Description", "Must specify a description");
                    haveErrors = true;
                }

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

                if (haveErrors) return View(armor);

                armor.Id = Guid.NewGuid();
                armor.Stats.RemoveAll(s => s.StatValue == 0);

                if (DataRepositories.ArmorRepository.Create(armor))
                {
                    return RedirectToAction("Index");
                }
            }

            return View(armor);
        }
    }
}