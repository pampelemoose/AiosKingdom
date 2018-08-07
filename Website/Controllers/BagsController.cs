using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Website.Authentication;

namespace Website.Controllers
{
    public class BagsController : AKBaseController
    {
        public ActionResult Index(Models.Filters.BagFilter filter)
        {
            var bags = DataRepositories.BagRepository.GetAll();

            filter.Items = filter.FilterList(bags);

            return View(filter);
        }

        [CustomAuthorize(Roles = "SuperAdmin")]
        [HttpGet]
        public ActionResult Create()
        {
            var bag = new Models.BagModel();

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

            return View(bag);
        }

        [CustomAuthorize(Roles = "SuperAdmin")]
        [HttpGet]
        public ActionResult Edit(Guid id)
        {
            var bag = DataRepositories.BagRepository.GetById(id);

            if (bag != null)
            {
                var model = new Models.BagModel
                {
                    Id = bag.Id,
                    SelectedVersion = bag.VersionId,
                    Name = bag.Name,
                    Description = bag.Description,
                    Image = bag.Image,
                    Quality = bag.Quality,
                    ItemLevel = bag.ItemLevel,
                    UseLevelRequired = bag.UseLevelRequired,
                    SlotCount = bag.SlotCount,
                    Stats = bag.Stats
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

        [CustomAuthorize(Roles = "SuperAdmin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Models.BagModel bag)
        {
            if (ModelState.IsValid)
            {
                bag.Stats.RemoveAll(s => s.StatValue == 0);

                if (DataRepositories.BagRepository.Update(new DataModels.Items.Bag
                {
                    Id = bag.Id,
                    VersionId = bag.SelectedVersion,
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

            return View(bag);
        }
    }
}