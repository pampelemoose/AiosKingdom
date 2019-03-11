using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Website.Authentication;

namespace Website.Controllers
{
    public class BooksController : AKBaseController
    {
        public ActionResult Index(Models.Filters.BookFilter filter)
        {
            var books = DataRepositories.BookRepository.GetAll();

            filter.Books = filter.FilterList(books);

            return View(filter);
        }

        public ActionResult Details(Guid id)
        {
            var book = DataRepositories.BookRepository.GetById(id);

            book.Talents = book.Talents.OrderBy(t => t.Branch).ThenBy(t => t.Leaf).ToList();

            return View(book);
        }

        [CustomAuthorize(Roles = "BookWriter")]
        [HttpGet]
        public ActionResult Create()
        {
            var book = new Models.BookModel();

            book.Inscriptions = new List<Models.InscriptionModel>();
            book.Talents = new List<Models.TalentModel>();

            return View(book);
        }

        [CustomAuthorize(Roles = "BookWriter")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Models.BookModel bookModel)
        {
            if (bookModel.Inscriptions == null)
            {
                bookModel.Inscriptions = new List<Models.InscriptionModel>();
            }

            if (ModelState.IsValid)
            {
                if (bookModel.Inscriptions.Count == 0)
                {
                    Alert(AlertMessage.AlertType.Danger, $"You need at least one Inscription and one Talent.");
                    return View(bookModel);
                }

                bool hasErrors = false;
                foreach (var insc in bookModel.Inscriptions)
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
                    Alert(AlertMessage.AlertType.Danger, $"Please check the data inputed and retry.", "Error !");
                    return View(bookModel);
                }

                var bookId = Guid.NewGuid();
                var book = new DataModels.Skills.Book
                {
                    Id = Guid.NewGuid(),
                    VersionId = bookModel.SelectedVersion,
                    Vid = bookId,
                    Quality = bookModel.Quality,
                    Name = bookModel.Name,
                    Description = bookModel.Description,
                    EmberCost = bookModel.StatCost,
                    ManaCost = bookModel.ManaCost,
                    Cooldown = bookModel.Cooldown,
                    Inscriptions = new List<DataModels.Skills.Inscription>(),
                    Talents = new List<DataModels.Skills.Talent>()
                };
                foreach (var inscModel in bookModel.Inscriptions)
                {
                    var insc = new DataModels.Skills.Inscription
                    {
                        Id = Guid.NewGuid(),
                        BookId = bookId,
                        Type = inscModel.Type,
                        BaseValue = inscModel.BaseValue,
                        StatType = inscModel.StatType,
                        Ratio = inscModel.Ratio,
                        Duration = inscModel.Duration,
                        IncludeWeaponDamages = inscModel.IncludeWeaponDamages,
                        WeaponTypes = inscModel.WeaponTypes?.Select(wt => (DataModels.Items.ItemType)wt.Type).ToList(),
                        WeaponDamagesRatio = inscModel.WeaponDamagesRatio,
                        PreferredWeaponTypes = inscModel.PreferredWeaponTypes?.Select(wt => (DataModels.Items.ItemType)wt.Type).ToList(),
                        PreferredWeaponDamagesRatio = inscModel.PreferredWeaponDamagesRatio
                    };
                    book.Inscriptions.Add(insc);
                }
                if (DataRepositories.BookRepository.Create(book))
                {
                    return RedirectToAction("Index");
                }
            }

            return View(bookModel);
        }

        [CustomAuthorize(Roles = "BookWriter")]
        [HttpGet]
        public ActionResult AddInscPartial()
        {
            var insc = new Models.InscriptionModel();

            return PartialView("InscPartial", insc);
        }

        [CustomAuthorize(Roles = "BookWriter")]
        [HttpGet]
        public ActionResult AddWeaponTypePartial(string inscId, string typeExtension)
        {
            var type = new Models.InscWeaponTypeModel
            {
                InscId = inscId,
                TypeExtension = typeExtension
            };

            return PartialView("WeaponTypePartial", type);
        }

        [CustomAuthorize(Roles = "BookWriter")]
        [HttpGet]
        public ActionResult Edit(Guid id)
        {
            var book = DataRepositories.BookRepository.GetById(id);

            if (book != null)
            {
                var model = new Models.BookModel
                {
                    Id = book.Id,
                    SelectedVersion = book.VersionId,
                    Name = book.Name,
                    Quality = book.Quality,
                    Description = book.Description,
                    StatCost = book.EmberCost,
                    ManaCost = book.ManaCost,
                    Cooldown = book.Cooldown,
                    Inscriptions = new List<Models.InscriptionModel>(),
                    Talents = new List<Models.TalentModel>()
                };

                foreach (var insc in book.Inscriptions)
                {
                    var inscModel = new Models.InscriptionModel
                    {
                        Id = insc.Id,
                        Type = insc.Type,
                        BaseValue = insc.BaseValue,
                        StatType = insc.StatType,
                        Ratio = insc.Ratio,
                        Duration = insc.Duration,
                        IncludeWeaponDamages = insc.IncludeWeaponDamages,
                        WeaponTypes = new List<Models.InscWeaponTypeModel>(),
                        WeaponDamagesRatio = insc.WeaponDamagesRatio,
                        PreferredWeaponTypes = new List<Models.InscWeaponTypeModel>(),
                        PreferredWeaponDamagesRatio = insc.PreferredWeaponDamagesRatio,
                    };

                    foreach (var type in insc.WeaponTypes)
                    {
                        inscModel.WeaponTypes.Add(new Models.InscWeaponTypeModel
                        {
                            Type = type
                        });
                    }

                    foreach (var type in insc.PreferredWeaponTypes)
                    {
                        inscModel.PreferredWeaponTypes.Add(new Models.InscWeaponTypeModel
                        {
                            Type = type
                        });
                    }

                    model.Inscriptions.Add(inscModel);
                }

                foreach (var talent in book.Talents)
                {
                    var tal = new Models.TalentModel
                    {
                        Id = talent.Id,
                        Branch = talent.Branch,
                        Leaf = talent.Leaf,
                        TalentPointsRequired = talent.TalentPointsRequired,
                        TargetInscription = talent.TargetInscription,
                        Inscriptions = book.Inscriptions,
                        Unlocks = new List<Models.TalentUnlockTypeModel>(),
                        Type = talent.Type,
                        Value = talent.Value
                    };

                    foreach (var unlock in talent.Unlocks)
                    {
                        tal.Unlocks.Add(new Models.TalentUnlockTypeModel
                        {
                            Type = unlock
                        });
                    }

                    model.Talents.Add(tal);
                }

                model.Talents = model.Talents.OrderBy(t => t.Branch).ThenBy(t => t.Leaf).ToList();

                return View(model);
            }

            return RedirectToAction("Index");
        }

        [CustomAuthorize(Roles = "BookWriter")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Models.BookModel bookModel)
        {
            bool newItems = false;
            if (bookModel.NewInsc > 0)
            {
                while (bookModel.NewInsc > 0)
                {
                    if (bookModel.Inscriptions == null)
                        bookModel.Inscriptions = new List<Models.InscriptionModel>();

                    bookModel.Inscriptions.Add(new Models.InscriptionModel());
                    bookModel.NewInsc--;
                }

                newItems = true;
            }

            if (newItems)
                return View(bookModel);

            if (bookModel.Inscriptions == null)
            {
                bookModel.Inscriptions = new List<Models.InscriptionModel>();
            }

            if (ModelState.IsValid)
            {
                if (bookModel.Inscriptions.Count == 0)
                {
                    Alert(AlertMessage.AlertType.Danger, $"You need at least one inscription.");
                    return View(bookModel);
                }

                bool hasErrors = false;
                foreach (var insc in bookModel.Inscriptions)
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
                    Alert(AlertMessage.AlertType.Danger, $"Please check the data inputed and retry.", "Error !");
                    return View(bookModel);
                }

                var book = new DataModels.Skills.Book
                {
                    Id = bookModel.Id,
                    VersionId = bookModel.SelectedVersion,
                    Quality = bookModel.Quality,
                    Name = bookModel.Name,
                    Description = bookModel.Description,
                    EmberCost = bookModel.StatCost,
                    ManaCost = bookModel.ManaCost,
                    Cooldown = bookModel.Cooldown,
                    Inscriptions = new List<DataModels.Skills.Inscription>(),
                    Talents = new List<DataModels.Skills.Talent>()
                };

                foreach (var inscModel in bookModel.Inscriptions)
                {
                    var insc = new DataModels.Skills.Inscription
                    {
                        Id = inscModel.Id,
                        Type = inscModel.Type,
                        BaseValue = inscModel.BaseValue,
                        StatType = inscModel.StatType,
                        Ratio = inscModel.Ratio,
                        Duration = inscModel.Duration,
                        IncludeWeaponDamages = inscModel.IncludeWeaponDamages,
                        WeaponTypes = inscModel.WeaponTypes?.Select(wt => (DataModels.Items.ItemType)wt.Type).ToList(),
                        WeaponDamagesRatio = inscModel.WeaponDamagesRatio,
                        PreferredWeaponTypes = inscModel.PreferredWeaponTypes?.Select(wt => (DataModels.Items.ItemType)wt.Type).ToList(),
                        PreferredWeaponDamagesRatio = inscModel.PreferredWeaponDamagesRatio
                    };
                    book.Inscriptions.Add(insc);
                }

                foreach (var talentModel in bookModel.Talents)
                {
                    var tal = new DataModels.Skills.Talent
                    {
                        Id = talentModel.Id,
                        Branch = talentModel.Branch,
                        Leaf = talentModel.Leaf,
                        Unlocks = talentModel.Unlocks?.Select(wt => (DataModels.Skills.TalentUnlock)wt.Type).ToList(),
                        TalentPointsRequired = talentModel.TalentPointsRequired,
                        TargetInscription = talentModel.TargetInscription,
                        Type = talentModel.Type,
                        Value = talentModel.Value
                    };
                    book.Talents.Add(tal);
                }

                if (DataRepositories.BookRepository.Update(book))
                {
                    return RedirectToAction("Index");
                }
            }

            return View(bookModel);
        }
    }
}