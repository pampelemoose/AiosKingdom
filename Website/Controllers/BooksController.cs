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
                            Description = inscModel.Inscription.Description,
                            Type = inscModel.Inscription.Type,
                            BaseValue = inscModel.Inscription.BaseValue,
                            StatType = inscModel.Inscription.StatType,
                            Ratio = inscModel.Inscription.Ratio,
                            Duration = inscModel.Inscription.Duration
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
                PageId = id,
                Inscription = new DataModels.Skills.Inscription()
            };

            return PartialView("InscPartial", insc);
        }
    }
}