using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Website.Authentication;

namespace Website.Controllers
{
    [CustomAuthorize(Roles = "JobCreator")]
    public class JobsController : Controller
    {
        public ActionResult Index(Models.Filters.JobFilter filter)
        {
            var jobs = DataRepositories.JobRepository.GetAll();

            filter.Jobs = filter.FilterList(jobs);

            return View(filter);
        }
    }
}