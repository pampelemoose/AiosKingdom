using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Website.Authentication;

namespace Website.Controllers
{
    public class BagsController : Controller
    {
        public ActionResult Index(Models.BagFilter filter)
        {
            var bags = DataRepositories.BagRepository.GetAll();

            filter.Items = filter.FilterList(bags);

            return View(filter);
        }

        [CustomAuthorize(Roles = "SuperAdmin")]
        [HttpGet]
        public ActionResult Create()
        {
            var bag = new DataModels.Items.Bag();
            bag.Stats = new List<DataModels.Items.ItemStat>();

            foreach (DataModels.Soul.Stats en in Enum.GetValues(typeof(DataModels.Soul.Stats)))
            {
                bag.Stats.Add(new DataModels.Items.ItemStat
                {
                    Type = en
                });
            }
            return View(bag);
        }

        [CustomAuthorize(Roles = "SuperAdmin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(DataModels.Items.Bag bag)
        {
            if (ModelState.IsValid)
            {
                bool haveErrors = false;
                if (string.IsNullOrEmpty(bag.Name))
                {
                    ModelState.AddModelError("Name", "Must specify a name.");
                    haveErrors = true;
                }

                if (string.IsNullOrEmpty(bag.Description))
                {
                    ModelState.AddModelError("Description", "Must specify a description");
                    haveErrors = true;
                }

                if (bag.ItemLevel < 1)
                {
                    ModelState.AddModelError("ItemLevel", "Must be > 0");
                    haveErrors = true;
                }

                if (bag.UseLevelRequired < 1)
                {
                    ModelState.AddModelError("UseLevelRequired", "Must be > 0");
                    haveErrors = true;
                }

                if (bag.SlotCount < 1)
                {
                    ModelState.AddModelError("SlotCount", "Must be > 0");
                    haveErrors = true;
                }

                if (haveErrors) return View(bag);

                bag.Id = Guid.NewGuid();
                bag.Stats.RemoveAll(s => s.StatValue == 0);

                if (DataRepositories.BagRepository.Create(bag))
                {
                    return RedirectToAction("Index");
                }
            }

            return View(bag);
        }
    }
}