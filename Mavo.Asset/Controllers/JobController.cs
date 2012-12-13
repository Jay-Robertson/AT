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
        private readonly AssetContext Context;

        public JobController(AssetContext context) { Context = context; }

        public virtual ActionResult Index()
        {
            ViewBag.JobsReadyToPick = new LeftNavViewModel() { Job = null, Jobs = Context.Jobs.GroupBy(x => x.Status).OrderBy(x=>x.Key).ToList() };
            return View();
        }

        public virtual ActionResult Create(int? id)
        {
            SetListsForCrud(null);
            ViewBag.Action = "Create a new";
            ViewBag.TemplateId = id;
            return View("Edit");
        }

        public virtual ActionResult Edit(int id)
        {
            Mavo.Assets.Models.Job job = Context.Jobs.FirstOrDefault(x=>x.Id == id);
            if (job != null)
            {
                SetListsForCrud(job);

                ViewBag.Action = "Edit a";

                return View("Edit", AutoMapper.Mapper.Map<Job,EditJobPostModel>(job));
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
                Job job = Context.Jobs.FirstOrDefault(x => x.Id == (jobPostModel.Id ?? 0));
                job = AutoMapper.Mapper.Map<EditJobPostModel, Job>(jobPostModel, job);
                if (ModelState.IsValid)
                {
                    if (jobPostModel.CustomerId.HasValue)
                        job.Customer = Context.Customers.FirstOrDefault(x=>x.Id == jobPostModel.CustomerId.Value);

                    if (jobPostModel.ForemanId.HasValue)
                        job.Foreman = Context.Users.FirstOrDefault(x=>x.Id == jobPostModel.ForemanId.Value);

                    if (jobPostModel.ProjectManagerId.HasValue)
                        job.ProjectManager = Context.Users.FirstOrDefault(x=>x.Id == jobPostModel.ProjectManagerId.Value);


                    if (!jobPostModel.Id.HasValue)
                    {
                        if (jobPostModel.TemplateId.HasValue)
                        {
                            var assets = Context.TemplateAssets.Include("Asset").Where(x => x.Template.Id == jobPostModel.TemplateId.Value).ToList();
                            job.Assets = assets.Select(x => new AssetWithQuantity() { Quantity = x.Quantity, Asset = x.Asset }).ToList();
                        }
                        Context.Jobs.Add(job);
                    }

                    Context.SaveChanges();

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

        [HttpPost]
        public virtual ActionResult MarkReadyToPick(int jobId)
        {
            Job job = Context.Jobs.FirstOrDefault(x => x.Id == jobId);
            job.Status = JobStatus.ReadyToPick;
            Context.SaveChanges();
            return RedirectToAction("Index");
        }

        private void SetListsForCrud(Job job)
        {
            ViewBag.Customers = Context.Customers.ToList();
            ViewBag.Foremen = Context.Users.Where(x => (x.Role & UserRole.Foreman) == UserRole.Foreman).ToList();
            ViewBag.ProjectManagers = Context.Users.Where(x => (x.Role & UserRole.ProjectManager) == UserRole.ProjectManager).ToList();
            ViewBag.JobsReadyToPick = new LeftNavViewModel() { Job = job, Jobs = Context.Jobs.ToList().GroupBy(x => x.Status).OrderBy(x => x.Key).ToList() };
        }
    }
}
