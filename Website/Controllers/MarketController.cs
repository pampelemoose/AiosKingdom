using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Website.Authentication;

namespace Website.Controllers
{
    public class MarketController : Controller
    {
        // GET: Market
        public ActionResult Index(Models.MarketSlotFilter filter)
        {
            var slots = DataRepositories.MarketRepository.GetAll();

            filter.Servers = DataRepositories.GameServerRepository.GetAll();
            filter.Slots = filter.FilterList(slots); 

            return View(filter);
        }

        [CustomAuthorize(Roles = "SuperAdmin")]
        [HttpGet]
        public ActionResult Create()
        {
            var slot = new Models.MarketSlotModel();
            slot.Servers = DataRepositories.GameServerRepository.GetAll();
            slot.Items = new List<DataModels.Items.AItem>();

            foreach (var armor in DataRepositories.ArmorRepository.GetAll())
            {
                slot.Items.Add(armor);
            }

            foreach (var bag in DataRepositories.BagRepository.GetAll())
            {
                slot.Items.Add(bag);
            }

            foreach (var consumable in DataRepositories.ConsumableRepository.GetAll())
            {
                slot.Items.Add(consumable);
            }

            return View(slot);
        }

        [CustomAuthorize(Roles = "SuperAdmin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Models.MarketSlotModel slot)
        {
            if (ModelState.IsValid)
            {
                if (DataRepositories.MarketRepository.Create(new DataModels.MarketSlot {
                    Id = Guid.NewGuid(),
                    ServerId = slot.SelectedServer,
                    Type = slot.Type,
                    ItemId = slot.SelectedItem,
                    Quantity = slot.Quantity,
                    ShardPrice = slot.ShardPrice,
                    BitPrice = slot.BitPrice
                }))
                {
                    return RedirectToAction("Index");
                }
            }

            slot.Servers = DataRepositories.GameServerRepository.GetAll();
            return View(slot);
        }
    }
}