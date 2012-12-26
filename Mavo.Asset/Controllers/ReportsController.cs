using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.Web.Mvc;
using Mavo.Assets.Models;
using Mavo.Assets.Models.ViewModel;

namespace Mavo.Assets.Controllers
{
    [Authorize]
    public partial class ReportsController : BaseController
    {
        private readonly AssetContext Context;
        public ReportsController(AssetContext context)
        {
            Context = context;
        }
        public virtual ActionResult LateJobs()
        {
            return View(Context.Jobs.Where(x => x.EstimatedCompletionDate < DateTime.Today && x.Status == JobStatus.Started).ToList());
        }
        private List<Job> GetReadyToPick()
        {
            return Context.Jobs.Include("ProjectManager").Include("PickedBy").Include("ReturnedBy").Where(x => x.Status == JobStatus.ReadyToPick).ToList().Where(x => x.PickupTime.Date == DateTime.Today).ToList();
        }
        private List<Job> GetReadyToReturn()
        {
            var tomorrow = DateTime.Today.AddDays(1).AddSeconds(-1);
            return Context.Jobs.Include("ProjectManager").Include("PickedBy").Include("ReturnedBy").Where(x => x.Status == JobStatus.Started && x.EstimatedCompletionDate >= DateTime.Today && x.EstimatedCompletionDate < tomorrow).ToList();
        }
        private List<Job> GetAlreadyPicked()
        {
            return Context.Jobs.Include("ProjectManager").Include("PickedBy").Include("ReturnedBy").Where(x => x.PickCompleted.HasValue && x.PickCompleted >= DateTime.Today).ToList();
        }
        private List<Job> GetAlreadyReturned()
        {
            return Context.Jobs.Include("ProjectManager").Include("PickedBy").Include("ReturnedBy").Where(x => x.ReturnCompleted.HasValue && x.ReturnCompleted.Value >= DateTime.Today).ToList();
        }
        private List<Job> GetBeingPicked()
        {
            return Context.Jobs.Include("ProjectManager").Include("PickedBy").Include("ReturnedBy").Where(x => x.Status == JobStatus.BeingPicked).OrderBy(x => x.PickStarted).ToList();
        }
        private List<Job> GetBeingReturned()
        {
            return Context.Jobs.Include("ProjectManager").Include("PickedBy").Include("ReturnedBy").Where(x => x.Status == JobStatus.BeingReturned).OrderBy(x => x.ReturnStarted).ToList();
        }
        public virtual ActionResult Jobs()
        {
            JobReportViewModel result = new JobReportViewModel()
                {
                    ReadyToPick = GetReadyToPick(),
                    ReadyToReturn = GetReadyToReturn(),
                    AlreadyPicked = GetAlreadyPicked(),
                    AlreadyReturned = GetAlreadyReturned(),
                    BeingPicked = GetBeingPicked(),
                    BeingReturned = GetBeingReturned()
                };
            return View(result);
        }

        public ActionResult AssetHistory(string id = null)
        {

            if (!String.IsNullOrEmpty(id))
            {
                var result = Context.AssetActivity
                                    .Include(x => x.Asset)
                                    .Include(x => x.Item)
                                    .Include(x => x.Job)
                                    .Include(x => x.User)
                                    .Where(x => x.Item.Barcode == id)
                                    .OrderBy(x => x.Date)
                                    .ToList();
                return View(result);
            }

            return View();
        }

        public ActionResult FellOffTruck(DateTime? startDate = null, DateTime? endDate = null)
        {
            if (!startDate.HasValue)
                startDate = DateTime.Today.AddDays(-7);
            if (!endDate.HasValue)
                endDate = DateTime.Today;

            endDate = endDate.Value.AddDays(1).AddSeconds(-1);

            var returnedJobsquery = Context.Jobs.Where(job => job.ReturnCompleted > startDate && job.ReturnCompleted <= endDate).ToList();
            var returnedJobIds = returnedJobsquery.Select(x => x.Id).ToArray();
            var assetsWithoutReturn = Context.AssetActivity.Where(ai => ai.Asset.Kind != AssetKind.Consumable && returnedJobIds.Contains(ai.Job.Id) && ai.Action == AssetAction.Return)
                .Select(x => new { Id = x.Asset.Id, JobId = x.Job.Id, Asset = x.Asset.Name, Barcode = x.Item.Barcode, Job = x.Job.Name, ReturnedOn = x.Job.ReturnCompleted, ReturnedBy = x.Job.ReturnedBy.FirstName + " " + x.Job.ReturnedBy.LastName })
                .Distinct()
                .ToList();

            return View(assetsWithoutReturn.Select(x=> new AssetsWithoutReturn(x.Id, x.Asset,x.Barcode,x.Job,x.ReturnedOn, x.ReturnedBy,x.JobId)).OrderBy(x=>x.ReturnedOn).ToList());
        }

        public ActionResult TomorrowsPicks()
        {
            var endDate = DateTime.Today.AddDays(2).AddSeconds(-1);
            var startDate = DateTime.Today.AddDays(1);
            var list = Context.Jobs.Include("Assets").Include("Asset")
                 .Where(x => x.PickupTime > startDate && x.PickupTime <= endDate)
                 .Select(x =>
                 new
                 {
                     JobId = x.Id,
                     JobName = x.Name,
                     JobNumber = x.JobNumber,
                     ManagerFirstName = x.ProjectManager.FirstName,
                     ManagerLastName = x.ProjectManager.LastName,
                     Customer = x.Customer.Name,
                     JobSite = x.JobSiteName,
                     ForemanFirstName = x.Foreman.FirstName,
                     ForemanLastName = x.Foreman.LastName,
                     PickStarted = x.PickStarted,
                     PickCompleted = x.PickCompleted,
                     ReturnStarted = x.ReturnStarted,
                     ReturnCompleted = x.ReturnCompleted,
                     PickupTime = x.PickupTime,
                     Assets = x.Assets.Select(a => new
                     {
                         Name = a.Asset.Name,
                         Id = a.Id,
                         Quantity = a.Quantity,
                         Kind = a.Asset.Kind,
                         AssetId = a.Asset.Id,
                         NotEnoughQuantity = a.Quantity > (a.Asset.Inventory ?? 0),
                         QuantityAvailable = a.Asset.Inventory
                     })
                 }).ToList();


            return View(list.Select(result =>
                new PickAJobModel()
                {
                    JobId = result.JobId,
                    JobName = result.JobName,
                    JobNumber = result.JobNumber,
                    Manager = String.Format("{0} {1}", result.ManagerFirstName, result.ManagerLastName),
                    JobSite = result.JobSite,
                    Foreman = String.Format("{0} {1}", result.ForemanFirstName, result.ForemanLastName),
                    Customer = result.Customer,
                    PickStarted = result.PickStarted,
                    ReturnStarted = result.ReturnStarted,
                    PickupTime = result.PickupTime,
                    Assets = result.Assets.Select(x => new JobAsset()
                    {
                        Name = x.Name,
                        Id = x.Id,
                        AssetId = x.AssetId,
                        QuantityNeeded = x.Quantity,
                        QuantityTaken = x.Quantity,
                        Kind = x.Kind,
                        NotEnoughQuantity = x.NotEnoughQuantity,
                        QuantityAvailable = x.QuantityAvailable
                    }).ToList()
                }));

        }
    }
}
