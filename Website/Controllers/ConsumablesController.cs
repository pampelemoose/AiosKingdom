using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Website.Authentication;

namespace Website.Controllers
{
    public class ConsumablesController : Controller
    {
        public ActionResult Index(Models.ConsumableFilter filter)
        {
            var consumables = DataRepositories.ConsumableRepository.GetAll();

            filter.VersionList = DataRepositories.VersionRepository.GetAll();
            filter.Items = filter.FilterList(consumables);

            return View(filter);
        }

        [CustomAuthorize(Roles = "SuperAdmin")]
        [HttpGet]
        public ActionResult Create()
        {
            var consumable = new Models.ConsumableModel();

            consumable.VersionList = DataRepositories.VersionRepository.GetAll();
            consumable.Effects = new List<DataModels.Items.ConsumableEffect>();

            return View(consumable);
        }

        [CustomAuthorize(Roles = "SuperAdmin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Models.ConsumableModel consumable)
        {
            if (ModelState.IsValid)
            {
                consumable.Effects.RemoveAll(s => string.IsNullOrEmpty(s.Name));
                consumable.Effects.RemoveAll(s => string.IsNullOrEmpty(s.Description));
                consumable.Effects.RemoveAll(s => s.AffectValue <= 0);
                consumable.Effects.RemoveAll(s => s.AffectTime < 0);

                if (consumable.Effects.Count == 0)
                {
                    consumable.VersionList = DataRepositories.VersionRepository.GetAll();
                    return View(consumable);
                }

                if (DataRepositories.ConsumableRepository.Create(new DataModels.Items.Consumable
                {
                    Id = Guid.NewGuid(),
                    VersionId = consumable.SelectedVersion,
                    ItemId = Guid.NewGuid(),
                    Name = consumable.Name,
                    Description = consumable.Description,
                    Image = consumable.Image,
                    ItemLevel = consumable.ItemLevel,
                    Quality = consumable.Quality,
                    UseLevelRequired = consumable.UseLevelRequired,
                    Effects = consumable.Effects
                }))
                {
                    return RedirectToAction("Index");
                }
            }

            consumable.VersionList = DataRepositories.VersionRepository.GetAll();
            if (consumable.Effects == null)
            {
                consumable.Effects = new List<DataModels.Items.ConsumableEffect>();
            }
            return View(consumable);
        }

        [CustomAuthorize(Roles = "SuperAdmin")]
        [HttpGet]
        public ActionResult AddEffectPartial()
        {
            var effect = new DataModels.Items.ConsumableEffect();

            return PartialView("EffectPartial", effect);
        }
    }
}