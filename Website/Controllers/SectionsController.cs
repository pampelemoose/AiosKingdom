using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Website.Authentication;

namespace Website.Controllers
{
    public class SectionsController : Controller
    {
        [CustomAuthorize(Roles = "SuperAdmin")]
        public ActionResult Index()
        {
            var sections = DataRepositories.SectionRepository.GetAll();

            return View(sections);
        }

        [CustomAuthorize(Roles = "SuperAdmin")]
        [HttpGet]
        public ActionResult Create()
        {
            var section = new DataModels.Website.Section();

            section.Contents = new List<DataModels.Website.Banner>();

            return View(section);
        }

        [CustomAuthorize(Roles = "SuperAdmin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(DataModels.Website.Section section)
        {
            if (ModelState.IsValid)
            {
                if (DataRepositories.SectionRepository.Create(section))
                {
                    return RedirectToAction("Index");
                }
            }

            return View(section);
        }

        [CustomAuthorize(Roles = "SuperAdmin")]
        [HttpGet]
        public ActionResult Edit(int id)
        {
            var section = DataRepositories.SectionRepository.GetById(id);

            return View(section);
        }

        [CustomAuthorize(Roles = "SuperAdmin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(DataModels.Website.Section section)
        {
            if (ModelState.IsValid)
            {
                if (DataRepositories.SectionRepository.Update(section))
                {
                    return RedirectToAction("Index");
                }
            }

            return View(section);
        }
    }
}