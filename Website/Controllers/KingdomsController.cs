using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Website.Authentication;

namespace Website.Controllers
{
    public class KingdomsController : Controller
    {
        [CustomAuthorize(Roles = "SuperAdmin")]
        public ActionResult Index()
        {
            var kingdoms = DataRepositories.KingdomRepository.GetAll();

            return View(kingdoms);
        }

        [CustomAuthorize(Roles = "SuperAdmin")]
        public ActionResult Raise(Guid id)
        {
            var kingdom = DataRepositories.KingdomRepository.GetById(id);

            if (kingdom != null)
            {
                kingdom.CurrentMaxLevel += kingdom.LevelGap;
                kingdom.CurrentMaxLevelCount = 0;
                kingdom.MaxLevelCountForGap *= 10;
                DataRepositories.KingdomRepository.Update(kingdom);
            }

            return RedirectToAction("Index");
        }
    }
}