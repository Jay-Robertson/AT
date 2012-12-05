using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Mavo.Assets.Data;
using Mavo.Assets.Models;
using Mavo.Assets.Models.ViewModel;

namespace Mavo.Assets.Controllers
{
    public partial class JobController : Controller
    {
        private readonly IRepository Repo;
        public JobController(IRepository repo)
        {
            Repo = repo;
        }
        //
        // GET: /Jobs/

        public virtual ActionResult Index()
        {
            ViewBag.JobsReadyToPick = new LeftNavViewModel() { Job = null, Jobs = Repo.GetReadyJobs() };
            return View();
        }

        //
        // GET: /Jobs/Details/5

        public virtual ActionResult Details(int id)
        {
            return View();
        }

        //
        // GET: /Jobs/Create

        public virtual ActionResult Create()
        {
            SetListsForCrud(null);
            ViewBag.Action = "Create a new";
            return View("Edit");
        }

        //
        // GET: /Jobs/Edit/5

        public virtual ActionResult Edit(int id)
        {
            Mavo.Assets.Models.Job job = Repo.GetJobById(id);
            if (job != null)
            {
                SetListsForCrud(job);

                ViewBag.Action = "Edit a";

                return View("Edit", job);
            }
            else
                return RedirectToAction(MVC.Job.Create());

        }

        //
        // POST: /Jobs/Edit/5

        [HttpPost]
        public virtual ActionResult Edit(Job job)
        {
            try
            {
                if (ModelState.IsValid)
                    return RedirectToAction(MVC.Job.Edit(job));
                else
                    return View();
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /Jobs/Delete/5

        public virtual ActionResult Delete(int id)
        {
            return View();
        }

        //
        // POST: /Jobs/Delete/5

        [HttpPost]
        public virtual ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        private void SetListsForCrud(Job job)
        {
            ViewBag.Customers = Repo.GetCustomers();
            ViewBag.JobsReadyToPick = new LeftNavViewModel() { Job = job, Jobs = Repo.GetReadyJobs() };

        }
    }
}
