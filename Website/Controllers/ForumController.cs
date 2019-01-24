using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Website.Authentication;

namespace Website.Controllers
{
    public class ForumController : AKBaseController
    {
        // GET: Forum
        public ActionResult Index()
        {
            var categories = DataRepositories.ForumRepository.GetAllCategories();

            return View(categories);
        }

        [CustomAuthorize(Roles = "SuperAdmin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(Models.CategoryModel model)
        {
            if (ModelState.IsValid)
            {
                if (DataRepositories.ForumRepository.CreateCategory(new DataModels.Website.Category
                {
                    Name = model.Name,
                    IsActive = true
                }))
                {
                    return RedirectToAction("Index");
                }
            }

            var categories = DataRepositories.ForumRepository.GetAllCategories();

            return View(categories);
        }

        public ActionResult Category(int id)
        {
            var category = DataRepositories.ForumRepository.GetCategoryById(id);

            return View(category);
        }

        [CustomAuthorize(Roles = "User")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Category(int id, Models.ThreadModel model)
        {
            if (ModelState.IsValid)
            {
                if (DataRepositories.ForumRepository.CreateThread(new DataModels.Website.Thread
                {
                    Name = model.Name,
                    CreatedAt = DateTime.Now,
                    CreatedBy = GetLoggedId(),
                    CreatedByUsername = User.Identity.Name,
                    IsOpen = true,
                    IsActive = true
                }, id))
                {
                    return RedirectToAction("Category", new { id = id });
                }
            }

            var category = DataRepositories.ForumRepository.GetCategoryById(id);

            return View(category);
        }

        [HttpGet]
        public ActionResult Thread(int id)
        {
            var thread = DataRepositories.ForumRepository.GetThreadById(id);

            return View(thread);
        }

        [CustomAuthorize(Roles = "User")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Thread(int id, Models.CommentModel model)
        {
            if (ModelState.IsValid)
            {
                if (DataRepositories.ForumRepository.CreateComment(new DataModels.Website.Comment {
                    Content = model.Content,
                    CreatedAt = DateTime.Now,
                    CreatedBy = GetLoggedId(),
                    CreatedByUsername = User.Identity.Name
                }, id))
                {
                    return RedirectToAction("Thread", new { id = id });
                }
            }

            var thread = DataRepositories.ForumRepository.GetThreadById(id);

            return View(thread);
        }

        [CustomAuthorize(Roles = "ForumAdmin")]
        public ActionResult DeleteThread(int id)
        {
            if (DataRepositories.ForumRepository.DeleteThread(id))
            {
                return RedirectToAction("Index");
            }

            return RedirectToAction("Thread", new { id = id });
        }

        [CustomAuthorize(Roles = "ForumAdmin")]
        public ActionResult DeleteComment(int id)
        {
            if (DataRepositories.ForumRepository.DeleteComment(id))
            {
                return RedirectToAction("Index");
            }

            return RedirectToAction("Thread", new { id = id });
        }

    }
}