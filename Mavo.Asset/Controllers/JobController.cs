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
    public partial class JobController : BaseController
    {
        private readonly IRepository Repo;
        public JobController(IRepository repo)
        {
            Repo = repo;
        }

        public virtual ActionResult Index()
        {
            ViewBag.JobsReadyToPick = new LeftNavViewModel() { Job = null, Jobs = Repo.GetJobs().GroupBy(x => x.Status) };
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
                Job job = Repo.GetJobById(jobPostModel.Id ?? 0);
                job = AutoMapper.Mapper.Map<EditJobPostModel, Job>(jobPostModel, job);
                if (ModelState.IsValid)
                {
                    if (jobPostModel.CustomerId.HasValue)
                        job.Customer = Repo.GetCustomer(jobPostModel.CustomerId.Value);

                    if (jobPostModel.ForemanId.HasValue)
                        job.Foreman = Repo.GetUser(jobPostModel.ForemanId.Value);

                    if (jobPostModel.ProjectManagerId.HasValue)
                        job.ProjectManager = Repo.GetUser(jobPostModel.ProjectManagerId.Value);


                    if (!jobPostModel.Id.HasValue)
                        Repo.Context.Jobs.Add(job);

                    Repo.Context.SaveChanges();

                    return RedirectToAction("Edit", new { id = job.Id });
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

        private void SetListsForCrud(Job job)
        {
            ViewBag.Customers = Repo.GetCustomers();
            ViewBag.Foremen = Repo.GetForemen();
            ViewBag.ProjectManagers = Repo.GetProjectManagers();
            ViewBag.JobsReadyToPick = new LeftNavViewModel() { Job = job, Jobs = Repo.GetJobs().GroupBy(x => x.Status) };

        }
    }
}
