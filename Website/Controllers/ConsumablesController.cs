using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Website.Authentication;

namespace Website.Controllers
{
    public class ConsumablesController : AKBaseController
    {
        public ActionResult Index(Models.Filters.ConsumableFilter filter)
        {
            var consumables = DataRepositories.ConsumableRepository.GetAll();

            filter.Items = filter.FilterList(consumables);

            return View(filter);
        }

        [CustomAuthorize(Roles = "SuperAdmin")]
        [HttpGet]
        public ActionResult Create()
        {
            var consumable = new Models.ConsumableModel();

            consumable.Effects = new List<DataModels.Items.ConsumableEffect>();

            return View(consumable);
        }

        [CustomAuthorize(Roles = "SuperAdmin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Models.ConsumableModel consumableModel)
        {
            if (ModelState.IsValid)
            {
                consumableModel.Effects.RemoveAll(s => string.IsNullOrEmpty(s.Name));
                consumableModel.Effects.RemoveAll(s => string.IsNullOrEmpty(s.Description));
                consumableModel.Effects.RemoveAll(s => s.AffectValue <= 0);
                consumableModel.Effects.RemoveAll(s => s.AffectTime < 0);

                if (consumableModel.Effects.Count == 0)
                {
                    Alert(AlertMessage.AlertType.Danger, $"Need at least one effect.", "Effect missing !");
                    return View(consumableModel);
                }

                if (DataRepositories.ConsumableRepository.Create(new DataModels.Items.Consumable
                {
                    Id = Guid.NewGuid(),
                    VersionId = consumableModel.SelectedVersion,
                    ItemId = Guid.NewGuid(),
                    Name = consumableModel.Name,
                    Description = consumableModel.Description,
                    Image = consumableModel.Image,
                    ItemLevel = consumableModel.ItemLevel,
                    Quality = consumableModel.Quality,
                    UseLevelRequired = consumableModel.UseLevelRequired,
                    Space = consumableModel.Space,
                    SellingPrice = consumableModel.SellingPrice,
                    Effects = consumableModel.Effects
                }))
                {
                    return RedirectToAction("Index");
                }
            }

            if (consumableModel.Effects == null)
            {
                consumableModel.Effects = new List<DataModels.Items.ConsumableEffect>();
            }
            return View(consumableModel);
        }

        [CustomAuthorize(Roles = "SuperAdmin")]
        [HttpGet]
        public ActionResult AddEffectPartial()
        {
            var effect = new DataModels.Items.ConsumableEffect();
            effect.Id = Guid.NewGuid();

            return PartialView("EffectPartial", effect);
        }

        [CustomAuthorize(Roles = "SuperAdmin")]
        [HttpGet]
        public ActionResult Edit(Guid id)
        {
            var consumable = DataRepositories.ConsumableRepository.GetById(id);

            if (consumable != null)
            {
                var model = new Models.ConsumableModel
                {
                    Id = consumable.Id,
                    SelectedVersion = consumable.VersionId,
                    Name = consumable.Name,
                    Description = consumable.Description,
                    Image = consumable.Image,
                    Quality = consumable.Quality,
                    UseLevelRequired = consumable.UseLevelRequired,
                    ItemLevel = consumable.ItemLevel,
                    Space = consumable.Space,
                    SellingPrice = consumable.SellingPrice,
                    Effects = consumable.Effects
                };

                return View(model);
            }

            return RedirectToAction("Index");
        }

        [CustomAuthorize(Roles = "SuperAdmin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Models.ConsumableModel consumableModel)
        {
            if (ModelState.IsValid)
            {
                consumableModel.Effects.RemoveAll(s => string.IsNullOrEmpty(s.Name));
                consumableModel.Effects.RemoveAll(s => string.IsNullOrEmpty(s.Description));
                consumableModel.Effects.RemoveAll(s => s.AffectValue <= 0);
                consumableModel.Effects.RemoveAll(s => s.AffectTime < 0);

                if (consumableModel.Effects.Count == 0)
                {
                    Alert(AlertMessage.AlertType.Danger, $"Need at least one effect.", "Effect missing !");
                    return View(consumableModel);
                }

                if (DataRepositories.ConsumableRepository.Update(new DataModels.Items.Consumable
                {
                    Id = consumableModel.Id,
                    VersionId = consumableModel.SelectedVersion,
                    Name = consumableModel.Name,
                    Description = consumableModel.Description,
                    Image = consumableModel.Image,
                    ItemLevel = consumableModel.ItemLevel,
                    Quality = consumableModel.Quality,
                    UseLevelRequired = consumableModel.UseLevelRequired,
                    Space = consumableModel.Space,
                    SellingPrice = consumableModel.SellingPrice,
                    Effects = consumableModel.Effects
                }))
                {
                    return RedirectToAction("Index");
                }
            }

            if (consumableModel.Effects == null)
            {
                consumableModel.Effects = new List<DataModels.Items.ConsumableEffect>();
            }
            return View(consumableModel);
        }
    }
}