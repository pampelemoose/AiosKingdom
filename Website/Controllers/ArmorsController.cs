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

            filter.VersionList = DataRepositories.VersionRepository.GetAll();
            filter.Items = filter.FilterList(armors);

            return View(filter);
        }

        [CustomAuthorize(Roles = "SuperAdmin")]
        [HttpGet]
        public ActionResult Create()
        {
            var armor = new Models.ArmorModel();
            armor.VersionList = DataRepositories.VersionRepository.GetAll();
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
        public ActionResult Create(Models.ArmorModel armorModel)
        {
            if (ModelState.IsValid)
            {
                bool haveErrors = false;
                if (string.IsNullOrEmpty(armorModel.Name))
                {
                    ModelState.AddModelError("Name", "Must specify a name.");
                    haveErrors = true;
                }

                if (string.IsNullOrEmpty(armorModel.Description))
                {
                    ModelState.AddModelError("Description", "Must specify a description");
                    haveErrors = true;
                }

                if (armorModel.ItemLevel < 1)
                {
                    ModelState.AddModelError("ItemLevel", "Must be > 0");
                    haveErrors = true;
                }

                if (armorModel.UseLevelRequired < 1)
                {
                    ModelState.AddModelError("UseLevelRequired", "Must be > 0");
                    haveErrors = true;
                }

                if (armorModel.ArmorValue < 1)
                {
                    ModelState.AddModelError("ArmorValue", "Must be > 0");
                    haveErrors = true;
                }

                if (haveErrors) return View(armorModel);

                armorModel.Stats.RemoveAll(s => s.StatValue == 0);

                if (DataRepositories.ArmorRepository.Create(new DataModels.Items.Armor
                {
                    Id = Guid.NewGuid(),
                    VersionId = armorModel.SelectedVersion,
                    ItemId = Guid.NewGuid(),
                    Name = armorModel.Name,
                    Description = armorModel.Description,
                    Image = armorModel.Image,
                    ItemLevel = armorModel.ItemLevel,
                    Quality = armorModel.Quality,
                    Part = armorModel.Part,
                    UseLevelRequired = armorModel.UseLevelRequired,
                    ArmorValue = armorModel.ArmorValue,
                    Stats = armorModel.Stats
                }))
                {
                    return RedirectToAction("Index");
                }
            }

            return View(armorModel);
        }
    }
}