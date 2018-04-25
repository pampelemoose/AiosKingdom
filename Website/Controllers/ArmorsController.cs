using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Website.Controllers
{
    public class ArmorsController : Controller
    {
        // GET: Armors
        public ActionResult Index()
        {
            var armors = new List<DataModels.Items.Armor>();

            using (var context = new DataRepositories.DispatchDbContext())
            {
                armors = context.Armors.ToList();
            }

            return View(armors);
        }

        [HttpGet]
        public ActionResult Create()
        {
            var armor = new DataModels.Items.Armor();
            armor.Stats = new List<DataModels.Items.ItemStat>();

            foreach (DataModels.Soul.Stats en in Enum.GetValues(typeof(DataModels.Soul.Stats)))
            {
                armor.Stats.Add(new DataModels.Items.ItemStat
                {
                    Type = en
                });
            }

            return View(armor);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(DataModels.Items.Armor armor)
        {
            if (ModelState.IsValid)
            {
                armor.Id = Guid.NewGuid();
                armor.Stats.RemoveAll(s => s.StatValue == 0);

                using (var context = new DataRepositories.DispatchDbContext())
                {
                    context.Armors.Add(armor);
                    context.SaveChanges();
                }

                return RedirectToAction("Index");
            }

            return View(armor);
        }
    }
}