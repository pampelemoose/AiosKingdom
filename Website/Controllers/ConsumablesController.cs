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

            filter.Items = filter.FilterList(consumables);

            return View(filter);
        }

        [CustomAuthorize(Roles = "SuperAdmin")]
        [HttpGet]
        public ActionResult Create()
        {
            var consumable = new DataModels.Items.Consumable();
            consumable.Effects = new List<DataModels.Items.ConsumableEffect>();

            foreach (DataModels.Items.EffectType en in Enum.GetValues(typeof(DataModels.Items.EffectType)))
            {
                consumable.Effects.Add(new DataModels.Items.ConsumableEffect
                {
                    Type = en
                });
            }

            return View(consumable);
        }

        [CustomAuthorize(Roles = "SuperAdmin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(DataModels.Items.Consumable consumable)
        {
            if (ModelState.IsValid)
            {
                bool haveErrors = false;
                if (string.IsNullOrEmpty(consumable.Name))
                {
                    ModelState.AddModelError("Name", "Must specify a name.");
                    haveErrors = true;
                }

                if (string.IsNullOrEmpty(consumable.Description))
                {
                    ModelState.AddModelError("Description", "Must specify a description");
                    haveErrors = true;
                }

                if (consumable.ItemLevel < 1)
                {
                    ModelState.AddModelError("ItemLevel", "Must be > 0");
                    haveErrors = true;
                }

                if (consumable.UseLevelRequired < 1)
                {
                    ModelState.AddModelError("UseLevelRequired", "Must be > 0");
                    haveErrors = true;
                }

                if (haveErrors) return View(consumable);

                consumable.Id = Guid.NewGuid();
                consumable.Effects.RemoveAll(s => string.IsNullOrEmpty(s.Name));
                consumable.Effects.RemoveAll(s => string.IsNullOrEmpty(s.Description));
                consumable.Effects.RemoveAll(s => s.AffectValue <= 0);
                consumable.Effects.RemoveAll(s => s.AffectTime < 0);

                if (DataRepositories.ConsumableRepository.Create(consumable))
                {
                    return RedirectToAction("Index");
                }
            }

            return View(consumable);
        }
    }
}