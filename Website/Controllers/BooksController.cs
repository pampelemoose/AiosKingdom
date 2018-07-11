using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Website.Authentication;

namespace Website.Controllers
{
    public class BooksController : Controller
    {
        // GET: Books
        public ActionResult Index(Models.Filters.BookFilter filter)
        {
            var books = DataRepositories.BookRepository.GetAll();

            filter.VersionList = DataRepositories.VersionRepository.GetAll();
            filter.Books = filter.FilterList(books);

            return View(filter);
        }

        public ActionResult Details(Guid id)
        {
            var book = DataRepositories.BookRepository.GetById(id);

            return View(book);
        }

        [CustomAuthorize(Roles = "SuperAdmin")]
        [HttpGet]
        public ActionResult Create()
        {
            var book = new Models.BookModel();
            book.VersionList = DataRepositories.VersionRepository.GetAll();
            book.Pages = new List<Models.PageModel>();

            return View(book);
        }

        [CustomAuthorize(Roles = "SuperAdmin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Models.BookModel bookModel)
        {
            ViewBag.Errors = new List<string>();

            if (bookModel.Pages == null)
            {
                bookModel.Pages = new List<Models.PageModel>();
            }
            foreach (var page in bookModel.Pages)
            {
                if (page.Inscriptions == null)
                {
                    page.Inscriptions = new List<Models.InscriptionModel>();
                }
            }

            if (ModelState.IsValid)
            {
                bookModel.Pages.RemoveAll(p => p.Inscriptions.Count == 0);

                if (bookModel.Pages.Count == 0 || bookModel.Pages?.Select(p => p.Inscriptions.Count).Sum() == 0)
                {
                    ViewBag.Errors.Add("You need at least one page and one inscription per page.");
                    bookModel.VersionList = DataRepositories.VersionRepository.GetAll();
                    return View(bookModel);
                }

                bool hasErrors = false;
                foreach (var insc in bookModel.Pages.SelectMany(p => p.Inscriptions))
                {
                    if (insc.WeaponTypes != null)
                        insc.WeaponTypes = insc.WeaponTypes.Where(wt => wt.Type != null).Distinct().ToList();

                    if (insc.PreferredWeaponTypes != null)
                        insc.PreferredWeaponTypes = insc.PreferredWeaponTypes.Where(wt => wt.Type != null).Distinct().ToList();

                    if (insc.IncludeWeaponDamages && 
                        ((insc.PreferredWeaponTypes?.Count > 0 && insc.PreferredWeaponDamagesRatio <= 0)
                        && (insc.WeaponTypes?.Count > 0 || insc.WeaponDamagesRatio <= 0)))
                    {
                        ModelState.AddModelError("IncludeWeaponDamages", "It is active so you need at least one WeaponType or PrefferedWeaponType specified. Ratios should be > 0");
                        hasErrors = true;
                    }
                    if (!insc.IncludeWeaponDamages && (insc.PreferredWeaponTypes?.Count > 0 || insc.WeaponTypes?.Count > 0))
                    {
                        ModelState.AddModelError("IncludeWeaponDamages", "It is inactive but one WeaponType or PrefferedWeaponType is specified.");
                        hasErrors = true;
                    }
                }
                if (hasErrors)
                {
                    bookModel.VersionList = DataRepositories.VersionRepository.GetAll();
                    return View(bookModel);
                }

                var bookId = Guid.NewGuid();
                var book = new DataModels.Skills.Book
                {
                    Id = Guid.NewGuid(),
                    VersionId = bookModel.SelectedVersion,
                    BookId = bookId,
                    Quality = bookModel.Quality,
                    Name = bookModel.Name,
                    Pages = new List<DataModels.Skills.Page>()
                };
                foreach (var pageModel in bookModel.Pages)
                {
                    var pageId = Guid.NewGuid();
                    var page = new DataModels.Skills.Page
                    {
                        Id = pageId,
                        BookId = bookId,
                        Description = pageModel.Description,
                        Image = pageModel.Image,
                        Rank = pageModel.Rank,
                        EmberCost = pageModel.StatCost,
                        ManaCost = pageModel.ManaCost,
                        Cooldown = pageModel.Cooldown,
                        Inscriptions = new List<DataModels.Skills.Inscription>()
                    };
                    foreach (var inscModel in pageModel.Inscriptions)
                    {
                        var insc = new DataModels.Skills.Inscription
                        {
                            Id = Guid.NewGuid(),
                            PageId = pageId,
                            Description = inscModel.Description,
                            Type = inscModel.Type,
                            BaseValue = inscModel.BaseValue,
                            StatType = inscModel.StatType,
                            Ratio = inscModel.Ratio,
                            Duration = inscModel.Duration,
                            IncludeWeaponDamages = inscModel.IncludeWeaponDamages,
                            WeaponTypes = inscModel.WeaponTypes?.Select(wt => (DataModels.Items.WeaponType)wt.Type).ToList(),
                            WeaponDamagesRatio = inscModel.WeaponDamagesRatio,
                            PreferredWeaponTypes = inscModel.PreferredWeaponTypes?.Select(wt => (DataModels.Items.WeaponType)wt.Type).ToList(),
                            PreferredWeaponDamagesRatio = inscModel.PreferredWeaponDamagesRatio
                        };
                        page.Inscriptions.Add(insc);
                    }
                    book.Pages.Add(page);
                }
                if (DataRepositories.BookRepository.Create(book))
                {
                    return RedirectToAction("Index");
                }
            }

            bookModel.VersionList = DataRepositories.VersionRepository.GetAll();
            return View(bookModel);
        }

        [CustomAuthorize(Roles = "SuperAdmin")]
        [HttpGet]
        public ActionResult AddPagePartial()
        {
            var page = new Models.PageModel();
            page.Inscriptions = new List<Models.InscriptionModel>();

            return PartialView("PagePartial", page);
        }

        [CustomAuthorize(Roles = "SuperAdmin")]
        [HttpGet]
        public ActionResult AddInscPartial(string id)
        {
            var insc = new Models.InscriptionModel
            {
                PageId = id
            };

            return PartialView("InscPartial", insc);
        }

        [CustomAuthorize(Roles = "SuperAdmin")]
        [HttpGet]
        public ActionResult AddWeaponTypePartial(string id, string inscId, string typeExtension)
        {
            var type = new Models.InscWeaponTypeModel
            {
                PageId = id,
                InscId = inscId,
                TypeExtension = typeExtension
            };

            return PartialView("WeaponTypePartial", type);
        }
    }
}