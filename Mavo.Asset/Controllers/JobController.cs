using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Mavo.Assets.Models;
using Mavo.Assets.Models.ViewModel;
using AutoMapper;
using Mavo.Assets.Services;
using System.Data.Entity;
namespace Mavo.Assets.Controllers
{
    [Authorize]
    public partial class JobController : BaseController
    {
        private readonly AssetContext Context;
        private readonly IAssetPicker AssetPicker;

        public JobController(AssetContext context, IAssetPicker assetPicker) 
        {
            AssetPicker = assetPicker;
            Context = context;
        }

        public virtual ActionResult Index(JobStatus? status = null, int? customerId = null, int? projectManagerId = null)
        {
            SetListsForCrud(null);
            if (status.HasValue || customerId.HasValue || projectManagerId.HasValue)
                return Index(new SearchResult() { Status = status, CustomerId = customerId, ProjectManagerId = projectManagerId });
            else
                return View();
        }

        [HttpPost]
        public virtual ActionResult AddAssetsFromTemplate(int id, int templateId)
        {
            AssetPicker.AddFromTemplate(id, templateId);
            ViewBag.Assets = Context.Jobs.Include(x => x.Assets).Include("Assets.Asset").First(x => x.Id == id).Assets;
            return PartialView(MVC.Asset.Views._AssetTable);
        }
      

        [HttpPost]
        public virtual ActionResult Index(SearchResult search)
        {
            SetListsForCrud(null);
            var query = Context.Jobs.AsQueryable();
            if (!String.IsNullOrEmpty(search.SearchString))
                query = query.Where(x =>
                    x.JobSiteName.Contains(search.SearchString)
                    || x.JobNumber.Contains(search.SearchString)
                    || x.ProjectManager.LastName.Contains(search.SearchString)
                    || x.ProjectManager.LastName.Contains(search.SearchString)
                    || x.Customer.Name.Contains(search.SearchString)
                    || x.Foreman.FirstName.Contains(search.SearchString)
                    || x.Foreman.LastName.Contains(search.SearchString)
                    || x.ContractNumber.Contains(search.SearchString)
                    || x.Description.Contains(search.SearchString)
                    );

            if (search.CustomerId.HasValue)
                query = query.Where(x => x.Customer.Id == search.CustomerId);
            if (search.StartDate.HasValue)
                query = query.Where(x => x.PickupTime >= search.StartDate.Value);
            if (search.EndDate.HasValue)
                query = query.Where(x => x.PickupTime <= search.EndDate.Value);

            if (search.ProjectManagerId.HasValue)
                query = query.Where(x => x.ProjectManager.Id == search.ProjectManagerId);
            if (search.Status.HasValue)
                query = query.Where(x => x.Status == search.Status.Value);
            search.Results = query.Select(x => new SearchResult()
            {
                JobName = x.JobSiteName,
                Customer = x.Customer.Name,
                CustomerId = x.Customer.Id,
                JobNumber = x.JobNumber,
                ProjectManagerId = x.ProjectManager.Id,
                ProjectManager = x.ProjectManager.FirstName + " " + x.ProjectManager.LastName,
                Status = x.Status,
                ShipDate = x.PickupTime,
                ReturnDate = x.ReturnedDate,
                Id = x.Id
            }).ToList();

            return View(search);
        }

        public virtual ActionResult Create(int? templateId)
        {
            SetListsForCrud(null);
            ViewBag.Action = "Create a new";
            ViewBag.TemplateId = templateId;
            return View("Edit");
        }

        public virtual ActionResult Edit(int id)
        {
            Mavo.Assets.Models.Job job = Context.Jobs.FirstOrDefault(x => x.Id == id);
            if (job != null)
            {
                SetListsForCrud(job);

                ViewBag.Action = "Edit a";

                return View("Edit", AutoMapper.Mapper.Map<Job, EditJobPostModel>(job));
            }
            else
                return RedirectToAction(MVC.Job.Create());
        }

        //
        // POST: /Jobs/Edit/5

        [HttpPost]
        public virtual ActionResult Edit(EditJobPostModel jobPostModel)
        {
            Job job = Context.Jobs.FirstOrDefault(x => x.Id == (jobPostModel.Id ?? 0));
            job = AutoMapper.Mapper.Map<EditJobPostModel, Job>(jobPostModel, job);
            if (ModelState.IsValid)
            {
                if (jobPostModel.CustomerId.HasValue)
                    job.Customer = Context.Customers.FirstOrDefault(x => x.Id == jobPostModel.CustomerId.Value);

                if (jobPostModel.ForemanId.HasValue)
                    job.Foreman = Context.Users.FirstOrDefault(x => x.Id == jobPostModel.ForemanId.Value);

                if (jobPostModel.ProjectManagerId.HasValue)
                    job.ProjectManager = Context.Users.FirstOrDefault(x => x.Id == jobPostModel.ProjectManagerId.Value);

                if (!jobPostModel.Id.HasValue)
                {
                    job.Status = JobStatus.New;
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
                return View(jobPostModel);
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
