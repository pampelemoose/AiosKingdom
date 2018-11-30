using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Website.Authentication;

namespace Website.Controllers
{
    public class MarketController : AKBaseController
    {
        [CustomAuthorize(Roles = "MarketRegulator")]
        public ActionResult Index(Models.Filters.MarketSlotFilter filter)
        {
            var slots = DataRepositories.MarketRepository.GetAll();

            filter.Slots = filter.FilterList(slots);

            return View(filter);
        }

        [CustomAuthorize(Roles = "MarketRegulator")]
        [HttpGet]
        public ActionResult Create()
        {
            var slot = new Models.MarketSlotModel();

            return View(slot);
        }

        [CustomAuthorize(Roles = "MarketRegulator")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Models.MarketSlotModel slot)
        {
            if (ModelState.IsValid)
            {
                if (DataRepositories.MarketRepository.Create(new DataModels.MarketSlot
                {
                    Id = Guid.NewGuid(),
                    TownId = slot.SelectedServer,
                    ItemId = slot.SelectedItem,
                    IsSpecial = true,
                    Quantity = slot.Quantity,
                    Price = slot.Price
                }))
                {
                    return RedirectToAction("Index");
                }
            }

            return View(slot);
        }

        [CustomAuthorize(Roles = "MarketRegulator")]
        [HttpGet]
        public ActionResult Edit(Guid id)
        {
            var slot = DataRepositories.MarketRepository.GetById(id);

            if (slot != null)
            {
                return View(new Models.MarketSlotModel
                {
                    Id = slot.Id,
                    Quantity = slot.Quantity,
                    Price = slot.Price
                });
            }

            return RedirectToAction("Index");
        }

        [CustomAuthorize(Roles = "MarketRegulator")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Models.MarketSlotModel slot)
        {
            if (ModelState.IsValid)
            {
                if (DataRepositories.MarketRepository.Update(new DataModels.MarketSlot
                {
                    Id = slot.Id,
                    Quantity = slot.Quantity,
                    Price = slot.Price
                }))
                {
                    return RedirectToAction("Index");
                }
            }

            return View(slot);
        }
    }
}