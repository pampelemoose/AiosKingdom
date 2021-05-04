using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Website.Authentication;

namespace Website.Controllers
{
    public class TalentsController : AKBaseController
    {
        [CustomAuthorize(Roles = "BookWriter")]
        [HttpGet]
        public ActionResult Create(Guid id)
        {
            var book = DataRepositories.BookRepository.GetByVid(id);

            if (book != null && book.Talents.Count == 0)
            {
                var model = new Models.BookModel
                {
                    Id = book.Vid,
                    Talents = new List<Models.TalentModel>(),
                };

                return View(model);
            }

            return RedirectToAction("Index", "Books");
        }

        [CustomAuthorize(Roles = "BookWriter")]
        [HttpPost]
        public ActionResult Create(Models.BookModel bookModel)
        {
            if (bookModel.Talents != null && bookModel.Talents.Count > 0)
            {
                var book = DataRepositories.BookRepository.GetByVid(bookModel.Id);

                foreach (var talentModel in bookModel.Talents)
                {
                    var tal = new DataModels.Skills.Talent
                    {
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
                    return RedirectToAction("Index", "Books");
                }
            }

            Alert(AlertMessage.AlertType.Danger, $"You need at least one Talent.");
            return View(bookModel);
        }

        [CustomAuthorize(Roles = "BookWriter")]
        [HttpGet]
        public ActionResult Edit(Guid id)
        {
            var book = DataRepositories.BookRepository.GetByVid(id);

            if (book != null && book.Talents.Count > 0)
            {
                var model = new Models.BookModel
                {
                    Id = book.Vid,
                    Talents = new List<Models.TalentModel>()
                };

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

            return RedirectToAction("Index", "Books");
        }

        [CustomAuthorize(Roles = "BookWriter")]
        [HttpPost]
        public ActionResult Edit(Models.BookModel bookModel)
        {
            if (bookModel.Talents != null && bookModel.Talents.Count > 0)
            {
                var book = DataRepositories.BookRepository.GetByVid(bookModel.Id);

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
                    return RedirectToAction("Index", "Books");
                }
            }

            Alert(AlertMessage.AlertType.Danger, $"You need at least one Talent.");
            return View(bookModel);
        }

        [CustomAuthorize(Roles = "BookWriter")]
        [HttpGet]
        public ActionResult AddTalentPartial(string bookId)
        {
            var book = DataRepositories.BookRepository.GetByVid(Guid.Parse(bookId));

            var talent = new Models.TalentModel
            {
                Inscriptions = book.Inscriptions
            };

            return PartialView("TalentPartial", talent);
        }

        [CustomAuthorize(Roles = "BookWriter")]
        [HttpGet]
        public ActionResult AddTalentUnlockPartial(string talentId)
        {
            var type = new Models.TalentUnlockTypeModel
            {
                TalentId = talentId
            };

            return PartialView("UnlockTypePartial", type);
        }
    }
}