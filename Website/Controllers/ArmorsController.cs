using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Website.Authentication;

namespace Website.Controllers
{
    public class ArmorsController : AKBaseController
    {
        public ActionResult Index(Models.Filters.ArmorFilter filter)
        {
            var armors = DataRepositories.ArmorRepository.GetAll();

            filter.Items = filter.FilterList(armors);

            return View(filter);
        }

        [CustomAuthorize(Roles = "ArmorSmith")]
        [HttpGet]
        public ActionResult Create()
        {
            var armor = new Models.ArmorModel();
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

        [CustomAuthorize(Roles = "ArmorSmith")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Models.ArmorModel armorModel)
        {
            if (ModelState.IsValid)
            {
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
                    Space = armorModel.Space,
                    SellingPrice = armorModel.SellingPrice,
                    ArmorValue = armorModel.ArmorValue,
                    Stats = armorModel.Stats
                }))
                {
                    return RedirectToAction("Index");
                }
            }

            return View(armorModel);
        }

        [CustomAuthorize(Roles = "ArmorSmith")]
        [HttpGet]
        public ActionResult Edit(Guid id)
        {
            var armor = DataRepositories.ArmorRepository.GetById(id);

            if (armor != null)
            {
                var model = new Models.ArmorModel
                {
                    Id = armor.Id,
                    SelectedVersion = armor.VersionId,
                    Name = armor.Name,
                    Description = armor.Description,
                    Image = armor.Image,
                    Quality = armor.Quality,
                    Part = armor.Part,
                    ItemLevel = armor.ItemLevel,
                    UseLevelRequired = armor.UseLevelRequired,
                    Space = armor.Space,
                    SellingPrice = armor.SellingPrice,
                    ArmorValue = armor.ArmorValue,
                    Stats = armor.Stats
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

        [CustomAuthorize(Roles = "ArmorSmith")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Models.ArmorModel armorModel)
        {
            if (ModelState.IsValid)
            {
                armorModel.Stats.RemoveAll(s => s.StatValue == 0);

                if (DataRepositories.ArmorRepository.Update(new DataModels.Items.Armor
                {
                    Id = armorModel.Id,
                    VersionId = armorModel.SelectedVersion,
                    Name = armorModel.Name,
                    Description = armorModel.Description,
                    Image = armorModel.Image,
                    ItemLevel = armorModel.ItemLevel,
                    Quality = armorModel.Quality,
                    UseLevelRequired = armorModel.UseLevelRequired,
                    Space = armorModel.Space,
                    SellingPrice = armorModel.SellingPrice,
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