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
        public ActionResult Index(Models.BookFilter filter)
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
            if (ModelState.IsValid)
            {
                bool reloadPage = false;
                if (bookModel.Pages == null)
                {
                    bookModel.Pages = new List<Models.PageModel>();
                }
                else
                {
                    foreach (var page in bookModel.Pages)
                    {
                        if (page.Inscriptions == null)
                        {
                            page.Inscriptions = new List<DataModels.Skills.Inscription>();
                        }

                        if (page.InscriptionCount != page.Inscriptions.Count)
                        {
                            if (page.InscriptionCount > page.Inscriptions.Count)
                            {
                                for (int i = page.InscriptionCount - page.Inscriptions.Count; i > 0; --i)
                                {
                                    page.Inscriptions.Add(new DataModels.Skills.Inscription());
                                }
                            }
                            else
                            {
                                int count = page.Inscriptions.Count;
                                for (int i = count - page.InscriptionCount; i > 0; --i)
                                {
                                    page.Inscriptions.RemoveAt(page.Inscriptions.Count - 1);
                                }
                            }

                            reloadPage = true;
                        }
                    }
                }

                if (reloadPage || bookModel.PageCount != bookModel.Pages.Count)
                {
                    if (bookModel.PageCount >= bookModel.Pages.Count)
                    {
                        for (int i = bookModel.PageCount - bookModel.Pages.Count; i > 0; --i)
                        {
                            bookModel.Pages.Add(new Models.PageModel());
                        }
                    }
                    else
                    {
                        int count = bookModel.Pages.Count;
                        for (int i = count - bookModel.PageCount; i > 0; --i)
                        {
                            bookModel.Pages.RemoveAt(bookModel.Pages.Count - 1);
                        }
                    }

                    bookModel.VersionList = DataRepositories.VersionRepository.GetAll();
                    return View(bookModel);
                }

                var bookId = Guid.NewGuid();
                var book = new DataModels.Skills.Book
                {
                    Id = bookId,
                    VersionId = bookModel.SelectedVersion,
                    BookId = Guid.NewGuid(),
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
                        Inscriptions = new List<DataModels.Skills.Inscription>()
                    };
                    foreach (var insc in pageModel.Inscriptions)
                    {
                        insc.Id = Guid.NewGuid();
                        insc.PageId = pageId;
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
    }
}