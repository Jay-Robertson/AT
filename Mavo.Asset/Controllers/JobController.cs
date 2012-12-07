using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Mavo.Assets.Data;
using Mavo.Assets.Models;
using Mavo.Assets.Models.ViewModel;
using AutoMapper;
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
        public virtual ActionResult Edit(EditJobPostModel jobPostModel)
        {
            try
            {
                AssetContext context = new AssetContext();
                Job job = context.Jobs.FirstOrDefault(x=> x.Id == jobPostModel.Id);
                job = AutoMapper.Mapper.Map<EditJobPostModel, Job>(jobPostModel, job);
                if (ModelState.IsValid)
                {
                    if (jobPostModel.CustomerId.HasValue)
                        job.Customer = Repo.GetCustomer(jobPostModel.CustomerId.Value);

                    if (!jobPostModel.Id.HasValue)
                        context.Jobs.Add(job);

                    context.SaveChanges();

                    return RedirectToAction(MVC.Job.Index());
                }
                else
                {
                    SetListsForCrud(job);
                    return View(job);
                }
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
            ViewBag.Foremen = Repo.GetForemen();
            ViewBag.ProjectManagers = Repo.GetProjectManagers();
            ViewBag.JobsReadyToPick = new LeftNavViewModel() { Job = job, Jobs = Repo.GetReadyJobs() };

        }
    }
}
