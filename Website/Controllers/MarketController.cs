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
        public ActionResult Index()
        {
            return View();
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

            return View(slot);
        }

        [CustomAuthorize(Roles = "SuperAdmin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Models.MarketSlotModel slot)
        {
            if (ModelState.IsValid)
            {
                bool haveErrors = false;
                if (slot.BitPrice < 1)
                {
                    ModelState.AddModelError("BitPrice", "Must be > 0");
                    haveErrors = true;
                }

                if (haveErrors) return View(slot);

                if (DataRepositories.MarketRepository.Create(new DataModels.MarketSlot {
                    Id = Guid.NewGuid(),
                    ServerId = slot.SelectedServer,
                    Type = DataModels.Items.ItemType.Armor,
                    ItemId = slot.SelectedItem,
                    Quantity = slot.Quantity,
                    ShardPrice = slot.ShardPrice,
                    BitPrice = slot.BitPrice
                }))
                {
                    return RedirectToAction("Index");
                }
            }

            return View(slot);
        }
    }
}