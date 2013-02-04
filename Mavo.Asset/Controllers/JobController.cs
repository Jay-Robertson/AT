using System;
using System.Linq;
using System.Web.Mvc;
using Mavo.Assets.Models;
using System.Data.Entity;
using Mavo.Assets.Services;
using System.Collections.Generic;
using Mavo.Assets.Models.ViewModel;
using Postal;

namespace Mavo.Assets.Controllers
{
    [Authorize]
    public partial class JobController : BaseController
    {
        private readonly IAssetActivityManager AssetActivity;
        private readonly AssetContext Context;
        private readonly IAssetPicker AssetPicker;
        private readonly ICurrentUserService CurrentUserService;

        public JobController(AssetContext context, IAssetPicker assetPicker, IAssetActivityManager assetActivity, ICurrentUserService currentUserService)
        {
            CurrentUserService = currentUserService;
            AssetActivity = assetActivity;
            AssetPicker = assetPicker;
            Context = context;
        }
        public virtual ActionResult AddOnModal(int id)
        {
            Job job = Context.Jobs.Include("Assets").Include("Assets.Asset").FirstOrDefault(x => x.Id == id);
            ViewBag.IsForJob = true;
            ViewBag.JobId = id;
            ViewBag.Assets = new List<AssetWithQuantity>();
            ViewBag.AssetCategories = Context.AssetCategories.ToList();
            ViewBag.Lock = false;
            return PartialView("Modals\\_AddOnModal", id);
        }
        public virtual ActionResult GetCustomerAddress(int id)
        {
            return Json(Context.Customers.FirstOrDefault(x=>x.Id == id).Address, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public virtual ActionResult CreateAddon(int id)
        {
            JobAddon newAddon = new JobAddon();
            Job job = Context.Jobs.Include(x => x.Customer).Include(x => x.Foreman).Include(x => x.ProjectManager).FirstOrDefault(x => x.Id == id);

            newAddon = AutoMapper.Mapper.Map<Job, JobAddon>(job);

            Context.JobAddons.Add(newAddon);
            Context.SaveChanges();
            return Json(newAddon.Id);
        }
        public virtual ActionResult TransferAssetsModal(int id)
        {
            List<Job> jobs = Context.Jobs.Include(x => x.Foreman).Include(x => x.Customer).Where(x => x.Status == JobStatus.Started && x.Id != id).OrderBy(x => x.JobNumber).ToList();
            return PartialView("Modals\\_TransferAssetModal", new TransferAssetsViewModel()
            {
                JobToTransferFrom = id,
                JobToTransferFromName = Context.Jobs.FirstOrDefault(x => x.Id == id).Name,
                Jobs = jobs,
                Assets = Context.PickedAssets.Where(x => x.Job.Id == id).Include(x => x.Asset).ToList()
            });
        }
        public virtual ActionResult TransferAssets(TransferAssetsViewModel model)
        {
            var assetsToTranfer = model.TransferredAssets.Where(x => x.IsSelected || (x.Quantity.HasValue && x.Quantity.Value > 0));
            Job jobToTransferTo = Context.Jobs.Include(x => x.PickedAssets).FirstOrDefault(x => x.Id == model.JobToTransferTo);
            Job jobToTransferFrom = Context.Jobs.Include(x => x.PickedAssets).FirstOrDefault(x => x.Id == model.JobToTransferFrom);
            foreach (var assetToTransfer in assetsToTranfer)
            {
                var pickedAsset = Context.PickedAssets.Include(x => x.Asset).Include(x => x.Item).Include(x => x.Job).FirstOrDefault(x => x.Id == assetToTransfer.PickedAssetId);

                if (pickedAsset.Asset.Kind == AssetKind.Serialized || assetToTransfer.Quantity == pickedAsset.Quantity)
                {
                    int index = jobToTransferFrom.PickedAssets.FindIndex(x => x.Id == assetToTransfer.PickedAssetId);
                    jobToTransferFrom.PickedAssets.RemoveAt(index);

                    pickedAsset.Job = jobToTransferTo;

                    jobToTransferTo.PickedAssets.Add(pickedAsset);
                }
                else if (assetToTransfer.Quantity < pickedAsset.Quantity)
                {
                    var newPickedAsset = new PickedAsset()
                    {
                        Asset = pickedAsset.Asset,
                        Picked = pickedAsset.Picked,
                        Quantity = assetToTransfer.Quantity.Value,
                        Item = pickedAsset.Item,
                        Job = jobToTransferTo
                    };
                    Context.PickedAssets.Add(newPickedAsset);

                    pickedAsset.Quantity -= newPickedAsset.Quantity;
                }

                AssetActivity.Add(AssetAction.TransferFrom, pickedAsset.Asset, pickedAsset.Item, jobToTransferFrom);
                AssetActivity.Add(AssetAction.TransferTo, pickedAsset.Asset, pickedAsset.Item, jobToTransferTo);

                Context.SaveChanges();
            }

            return RedirectToAction("Edit", new { id = model.JobToTransferFrom });
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
            var query = Context.Jobs.Where(x => (x is JobAddon && x.Status < JobStatus.Started) || !(x is JobAddon)).AsQueryable();
            if (!String.IsNullOrEmpty(search.SearchString))
                query = query.Where(x =>
                    x.Name.Contains(search.SearchString)
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
                Name = x.Name,
                Customer = x.Customer.Name,
                CustomerId = x.Customer.Id,
                JobNumber = x.JobNumber,
                ProjectManagerId = x.ProjectManager.Id,
                ProjectManager = x.ProjectManager.FirstName + " " + x.ProjectManager.LastName,
                Status = x.Status,
                ShipDate = x.PickupTime,
                ReturnDate = x.ReturnedDate,
                Id = x.Id,
                IsAddon = x is JobAddon
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
            Mavo.Assets.Models.Job job = Context.Jobs.Include(x => x.PickedAssets).Include("PickedAssets.Asset").Include("PickedAssets.Item").FirstOrDefault(x => x.Id == id);
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
        public virtual ActionResult SaveInvoice(int id, EditJobPostModel jobPostModel)
        {
            Job job = Context.Jobs.FirstOrDefault(x => x.Id == id);
            job.InvoiceDetail = jobPostModel.InvoiceDetail;
            var values = Request.Form["InvoiceDetail.SpecialForms"].Split(',');
            job.InvoiceDetail.SpecialForms = (SpecialForms)values.Aggregate(0, (acc, v) => acc |= Convert.ToInt32(v), acc => acc);
            Context.SaveChanges();
            return RedirectToAction("Edit", new { id = id });
        }

        [HttpPost]
        public virtual ActionResult SaveSummary(int id, EditJobPostModel jobPostModel)
        {
            Job job = Context.Jobs.FirstOrDefault(x => x.Id == id);
            job.Summary = jobPostModel.Summary;
            if (jobPostModel.ForemanId.HasValue)
                job.Foreman = Context.Users.FirstOrDefault(x => x.Id == jobPostModel.ForemanId.Value);

            Context.SaveChanges();
            return RedirectToAction("Edit", new { id = id });
        }

        [HttpPost]
        public virtual ActionResult Edit(EditJobPostModel jobPostModel)
        {
            Job job = Context.Jobs.FirstOrDefault(x => x.Id == (jobPostModel.Id ?? 0));
            job = AutoMapper.Mapper.Map<EditJobPostModel, Job>(jobPostModel, job);
            if (ModelState.IsValid)
            {
                if (job is JobAddon)
                {
                    job.PickupTime = jobPostModel.PickupTime;
                }
                else
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
                        job.CreatedDate = DateTime.Now;
                        job.SubmittedBy = CurrentUserService.GetCurrent();
                        if (jobPostModel.TemplateId.HasValue)
                        {
                            var assets = Context.TemplateAssets.Include("Asset").Where(x => x.Template.Id == jobPostModel.TemplateId.Value).ToList();
                            job.Assets = assets.Select(x => new AssetWithQuantity() { Quantity = x.Quantity, Asset = x.Asset }).ToList();
                        }
                        Context.Jobs.Add(job);
                    }
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

            dynamic email = new Email("ReadyToPick");
            email.Subject = String.Format("Job #{0} is ready to pick", job.JobNumber);
            email.To = Properties.Settings.Default.WarehouseManager;
            email.Job = job;
            email.Send();

            return RedirectToAction("Index");
        }

        private void SetListsForCrud(Job job)
        {
            ViewBag.Customers = Context.Customers.OrderBy(x=>x.Name).ToList();
            ViewBag.Foremen = Context.Users.Where(x => (x.Role & UserRole.Foreman) == UserRole.Foreman && !x.Disabled).ToList();
            ViewBag.ProjectManagers = Context.Users.Where(x => (x.Role & UserRole.ProjectManager) == UserRole.ProjectManager && !x.Disabled).ToList();
            ViewBag.JobsReadyToPick = new LeftNavViewModel() { Job = job, Jobs = Context.Jobs.ToList().GroupBy(x => x.Status).OrderBy(x => x.Key).ToList() };
        }
    }
}
