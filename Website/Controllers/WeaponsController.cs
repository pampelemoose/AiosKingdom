using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Website.Authentication;

namespace Website.Controllers
{
    public class WeaponsController : Controller
    {
        public ActionResult Index(Models.Filters.WeaponFilter filter)
        {
            var weapons = DataRepositories.WeaponRepository.GetAll();

            filter.VersionList = DataRepositories.VersionRepository.GetAll();
            filter.Items = filter.FilterList(weapons);

            return View(filter);
        }

        [CustomAuthorize(Roles = "SuperAdmin")]
        [HttpGet]
        public ActionResult Create()
        {
            var weapon = new Models.WeaponModel();
            weapon.VersionList = DataRepositories.VersionRepository.GetAll();
            weapon.Stats = new List<DataModels.Items.ItemStat>();

            foreach (DataModels.Soul.Stats en in Enum.GetValues(typeof(DataModels.Soul.Stats)))
            {
                weapon.Stats.Add(new DataModels.Items.ItemStat
                {
                    Type = en
                });
            }

            return View(weapon);
        }

        [CustomAuthorize(Roles = "SuperAdmin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Models.WeaponModel weaponModel)
        {
            if (ModelState.IsValid)
            {
                weaponModel.Stats.RemoveAll(s => s.StatValue == 0);

                if (DataRepositories.WeaponRepository.Create(new DataModels.Items.Weapon
                {
                    Id = Guid.NewGuid(),
                    VersionId = weaponModel.SelectedVersion,
                    ItemId = Guid.NewGuid(),
                    Name = weaponModel.Name,
                    Description = weaponModel.Description,
                    Image = weaponModel.Image,
                    ItemLevel = weaponModel.ItemLevel,
                    Quality = weaponModel.Quality,
                    HandlingType = weaponModel.HandlingType,
                    WeaponType = weaponModel.Type,
                    UseLevelRequired = weaponModel.UseLevelRequired,
                    MinDamages = weaponModel.MinDamages,
                    MaxDamages = weaponModel.MaxDamages,
                    Stats = weaponModel.Stats
                }))
                {
                    return RedirectToAction("Index");
                }
            }

            weaponModel.VersionList = DataRepositories.VersionRepository.GetAll();
            return View(weaponModel);
        }
    }
}