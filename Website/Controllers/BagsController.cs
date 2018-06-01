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
        public ActionResult Index(Models.Filters.BagFilter filter)
        {
            var bags = DataRepositories.BagRepository.GetAll();

            filter.VersionList = DataRepositories.VersionRepository.GetAll();
            filter.Items = filter.FilterList(bags);

            return View(filter);
        }

        [CustomAuthorize(Roles = "SuperAdmin")]
        [HttpGet]
        public ActionResult Create()
        {
            var bag = new Models.BagModel();

            bag.VersionList = DataRepositories.VersionRepository.GetAll();
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
        public ActionResult Create(Models.BagModel bag)
        {
            if (ModelState.IsValid)
            {
                bag.Stats.RemoveAll(s => s.StatValue == 0);

                if (bag.Stats.Count == 0)
                {
                    bag.VersionList = DataRepositories.VersionRepository.GetAll();
                    return View(bag);
                }

                if (DataRepositories.BagRepository.Create(new DataModels.Items.Bag
                {
                    Id = Guid.NewGuid(),
                    VersionId = bag.SelectedVersion,
                    ItemId = Guid.NewGuid(),
                    Name = bag.Name,
                    Description = bag.Description,
                    Image = bag.Image,
                    ItemLevel = bag.ItemLevel,
                    Quality = bag.Quality,
                    UseLevelRequired = bag.UseLevelRequired,
                    SlotCount = bag.SlotCount,
                    Stats = bag.Stats
                }))
                {
                    return RedirectToAction("Index");
                }
            }

            bag.VersionList = DataRepositories.VersionRepository.GetAll();
            return View(bag);
        }
    }
}