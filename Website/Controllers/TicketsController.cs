using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Website.Authentication;

namespace Website.Controllers
{
    [CustomAuthorize(Roles = "Ticketer")]
    public class TicketsController : AKBaseController
    {
        // GET: Tickets
        public ActionResult Index()
        {
            var tickets = DataRepositories.ForumRepository.GetAllTickets();

            return View(tickets);
        }

        public ActionResult Details(int id)
        {
            var ticket = DataRepositories.ForumRepository.GetTicketById(id);

            return View(ticket);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Details(int id, Models.CommentModel model)
        {
            if (ModelState.IsValid)
            {
                if (DataRepositories.ForumRepository.CreateComment(new DataModels.Website.Comment
                {
                    Content = model.Content,
                    CreatedAt = DateTime.Now,
                    CreatedBy = GetLoggedId(),
                    CreatedByUsername = User.Identity.Name
                }, id, true))
                {
                    return RedirectToAction("Details", new { id = id });
                }
            }

            var ticket = DataRepositories.ForumRepository.GetTicketById(id);

            return View(ticket);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AdministrateTicket(int id, DataModels.Website.Ticket model)
        {
            if (ModelState.IsValid)
            {
                model.AssignedAt = DateTime.Now;
                model.AssignedBy = GetLoggedId();
                model.AssignedByUsername = DataRepositories.UserRepository.GetById(GetLoggedId()).Username;
                model.AssignedToUsername = DataRepositories.UserRepository.GetById(model.AssignedTo).Username;

                DataRepositories.ForumRepository.UpdateTicket(model);
            }

            return RedirectToAction("Details", new { id = id });
        }

        [CustomAuthorize(Roles = "Ticketer")]
        public ActionResult DeleteComment(int id)
        {
            if (DataRepositories.ForumRepository.DeleteComment(id))
            {
                return RedirectToAction("Index");
            }

            return RedirectToAction("Details", new { id = id });
        }
    }
}