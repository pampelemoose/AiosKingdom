using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Website.Authentication;

namespace Website.Controllers
{
    public class WeaponsController : AKBaseController
    {
        public ActionResult Index(Models.Filters.WeaponFilter filter)
        {
            var weapons = DataRepositories.WeaponRepository.GetAll();

            filter.Items = filter.FilterList(weapons);

            return View(filter);
        }

        [CustomAuthorize(Roles = "WeaponSmith")]
        [HttpGet]
        public ActionResult Create()
        {
            var weapon = new Models.WeaponModel();

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

        [CustomAuthorize(Roles = "WeaponSmith")]
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
                    Space = weaponModel.Space,
                    SellingPrice = weaponModel.SellingPrice,
                    MinDamages = weaponModel.MinDamages,
                    MaxDamages = weaponModel.MaxDamages,
                    Stats = weaponModel.Stats
                }))
                {
                    return RedirectToAction("Index");
                }
            }

            return View(weaponModel);
        }

        [CustomAuthorize(Roles = "WeaponSmith")]
        [HttpGet]
        public ActionResult Edit(Guid id)
        {
            var weapon = DataRepositories.WeaponRepository.GetById(id);

            if (weapon != null)
            {
                var model = new Models.WeaponModel
                {
                    Id = weapon.Id,
                    SelectedVersion = weapon.VersionId,
                    Name = weapon.Name,
                    Description = weapon.Description,
                    Image = weapon.Image,
                    ItemLevel = weapon.ItemLevel,
                    Quality = weapon.Quality,
                    HandlingType = weapon.HandlingType,
                    Type = weapon.WeaponType,
                    UseLevelRequired = weapon.UseLevelRequired,
                    Space = weapon.Space,
                    SellingPrice = weapon.SellingPrice,
                    MinDamages = weapon.MinDamages,
                    MaxDamages = weapon.MaxDamages,
                    Stats = weapon.Stats
                };

                foreach (DataModels.Soul.Stats en in Enum.GetValues(typeof(DataModels.Soul.Stats)))
                {
                    if (model.Stats.FirstOrDefault(s => s.Type == en) == null)
                    {
                        model.Stats.Add(new DataModels.Items.ItemStat
                        {
                            Type = en
                        });
                    }
                }

                return View(model);
            }
            return RedirectToAction("Index");
        }

        [CustomAuthorize(Roles = "WeaponSmith")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Models.WeaponModel weaponModel)
        {
            if (ModelState.IsValid)
            {
                weaponModel.Stats.RemoveAll(s => s.StatValue == 0);

                if (DataRepositories.WeaponRepository.Update(new DataModels.Items.Weapon
                {
                    Id = weaponModel.Id,
                    VersionId = weaponModel.SelectedVersion,
                    Name = weaponModel.Name,
                    Description = weaponModel.Description,
                    Image = weaponModel.Image,
                    ItemLevel = weaponModel.ItemLevel,
                    Quality = weaponModel.Quality,
                    HandlingType = weaponModel.HandlingType,
                    WeaponType = weaponModel.Type,
                    UseLevelRequired = weaponModel.UseLevelRequired,
                    Space = weaponModel.Space,
                    SellingPrice = weaponModel.SellingPrice,
                    MinDamages = weaponModel.MinDamages,
                    MaxDamages = weaponModel.MaxDamages,
                    Stats = weaponModel.Stats
                }))
                {
                    return RedirectToAction("Index");
                }
            }

            return View(weaponModel);
        }
    }
}