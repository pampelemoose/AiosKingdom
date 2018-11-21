using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Website.Authentication;

namespace Website.Controllers
{
    public class ItemsController : AKBaseController
    {
        public ActionResult Index(Models.Filters.ItemFilter filter)
        {
            var items = DataRepositories.ItemRepository.GetAll();

            filter.Items = filter.FilterList(items);

            return View(filter);
        }

        [CustomAuthorize(Roles = "ItemSmith")]
        [HttpGet]
        public ActionResult Create()
        {
            var item = new Models.ItemModel();

            return View(item);
        }

        [CustomAuthorize(Roles = "ItemSmith")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Models.ItemModel itemModel)
        {
            if (ModelState.IsValid)
            {
                if (itemModel.Stats != null)
                {
                    itemModel.Stats.RemoveAll(s => s.StatValue == 0);
                }

                if (itemModel.Effects != null)
                {
                    itemModel.Effects.RemoveAll(s => string.IsNullOrEmpty(s.Name));
                    itemModel.Effects.RemoveAll(s => string.IsNullOrEmpty(s.Description));
                    itemModel.Effects.RemoveAll(s => s.AffectValue <= 0);
                    itemModel.Effects.RemoveAll(s => s.AffectTime < 0);
                }

                if (itemModel.Type == DataModels.Items.ItemType.Armor 
                    && (itemModel.ArmorValue == null || itemModel.Slot == null))
                {
                    Alert(AlertMessage.AlertType.Danger, $"Need armorValue and Slot for armors.", "Value missing !");
                    itemModel.SetupLists();
                    return View(itemModel);
                }

                if (itemModel.Type == DataModels.Items.ItemType.Consumable 
                    && (itemModel.Effects == null || (itemModel.Effects != null && itemModel.Effects.Count == 0)))
                {
                    Alert(AlertMessage.AlertType.Danger, $"Need at least one effect.", "Effect missing !");
                    itemModel.SetupLists();
                    return View(itemModel);
                }

                if (itemModel.Type == DataModels.Items.ItemType.Bag && itemModel.SlotCount == null)
                {
                    Alert(AlertMessage.AlertType.Danger, $"Need slotCount for bags.", "Value missing !");
                    itemModel.SetupLists();
                    return View(itemModel);
                }

                if (itemModel.Type != DataModels.Items.ItemType.Armor
                    && itemModel.Type != DataModels.Items.ItemType.Bag
                    && itemModel.Type != DataModels.Items.ItemType.Consumable
                    && itemModel.Type != DataModels.Items.ItemType.Jewelry
                    && itemModel.Type != DataModels.Items.ItemType.Junk
                    && (itemModel.Slot == null || itemModel.MinDamages == null || itemModel.MaxDamages == null))
                {
                    Alert(AlertMessage.AlertType.Danger, $"Need minDamages, maxDamages and Slot for weapons.", "Value missing !");
                    itemModel.SetupLists();
                    return View(itemModel);
                }

                if (DataRepositories.ItemRepository.Create(new DataModels.Items.Item(itemModel.Type, itemModel.Slot)
                {
                    Id = Guid.NewGuid(),
                    VersionId = itemModel.SelectedVersion,
                    ItemId = Guid.NewGuid(),
                    Name = itemModel.Name,
                    Description = itemModel.Description,
                    ItemLevel = itemModel.ItemLevel,
                    Quality = itemModel.Quality,
                    UseLevelRequired = itemModel.UseLevelRequired,
                    Space = itemModel.Space,
                    SellingPrice = itemModel.SellingPrice,
                    ArmorValue = itemModel.ArmorValue,
                    SlotCount = itemModel.SlotCount,
                    MinDamages = itemModel.MinDamages,
                    MaxDamages = itemModel.MaxDamages,
                    Stats = itemModel.Stats,
                    Effects = itemModel.Effects
                }))
                {
                    return RedirectToAction("Index");
                }
            }

            itemModel.SetupLists();
            return View(itemModel);
        }

        [CustomAuthorize(Roles = "ItemSmith")]
        [HttpGet]
        public ActionResult Edit(Guid id)
        {
            var item = DataRepositories.ItemRepository.GetById(id);

            if (item != null)
            {
                var model = new Models.ItemModel
                {
                    Id = item.Id,
                    SelectedVersion = item.VersionId,
                    Name = item.Name,
                    Description = item.Description,
                    Quality = item.Quality,
                    Slot = item.Slot,
                    ItemLevel = item.ItemLevel,
                    UseLevelRequired = item.UseLevelRequired,
                    Space = item.Space,
                    SellingPrice = item.SellingPrice,
                    ArmorValue = item.ArmorValue,
                    SlotCount = item.SlotCount,
                    MinDamages = item.MinDamages,
                    MaxDamages = item.MaxDamages,
                    Stats = item.Stats,
                    Effects = item.Effects
                };

                model.SetupLists();
                return View(model);
            }

            return RedirectToAction("Index");
        }

        [CustomAuthorize(Roles = "ItemSmith")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Models.ItemModel itemModel)
        {
            if (ModelState.IsValid)
            {
                itemModel.Stats.RemoveAll(s => s.StatValue == 0);

                if (DataRepositories.ItemRepository.Update(new DataModels.Items.Item
                {
                    Id = itemModel.Id,
                    VersionId = itemModel.SelectedVersion,
                    Name = itemModel.Name,
                    Description = itemModel.Description,
                    ItemLevel = itemModel.ItemLevel,
                    Quality = itemModel.Quality,
                    UseLevelRequired = itemModel.UseLevelRequired,
                    Space = itemModel.Space,
                    SellingPrice = itemModel.SellingPrice,
                    ArmorValue = itemModel.ArmorValue,
                    SlotCount = itemModel.SlotCount,
                    MinDamages = itemModel.MinDamages,
                    MaxDamages = itemModel.MaxDamages,
                    Stats = itemModel.Stats,
                    Effects = itemModel.Effects
                }))
                {
                    return RedirectToAction("Index");
                }
            }

            itemModel.SetupLists();
            return View(itemModel);
        }

        [CustomAuthorize(Roles = "ItemSmith")]
        [HttpGet]
        public ActionResult AddEffectPartial()
        {
            var effect = new DataModels.Items.ItemEffect();
            effect.Id = Guid.NewGuid();

            return PartialView("EffectPartial", effect);
        }
    }
}