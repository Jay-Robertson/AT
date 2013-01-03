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
            return View(Context.Jobs.Where(x => !(x is JobAddon) && x.EstimatedCompletionDate < DateTime.Today && x.Status == JobStatus.Started).ToList());
        }
        private List<Job> GetReadyToPick()
        {
            return Context.Jobs.Include("ProjectManager").Include("PickedBy").Include("ReturnedBy").Where(x => x.Status == JobStatus.ReadyToPick).ToList().Where(x => x.PickupTime.Date == DateTime.Today).ToList();
        }
        private List<Job> GetReadyToReturn()
        {
            var tomorrow = DateTime.Today.AddDays(1).AddSeconds(-1);
            return Context.Jobs.Include("ProjectManager").Include("PickedBy").Include("ReturnedBy").Where(x => !(x is JobAddon) && x.Status == JobStatus.Started && x.EstimatedCompletionDate >= DateTime.Today && x.EstimatedCompletionDate < tomorrow).ToList();
        }
        private List<Job> GetAlreadyPicked()
        {
            return Context.Jobs.Include("ProjectManager").Include("PickedBy").Include("ReturnedBy").Where(x => x.PickCompleted.HasValue && x.PickCompleted >= DateTime.Today).ToList();
        }
        private List<Job> GetAlreadyReturned()
        {
            return Context.Jobs.Include("ProjectManager").Include("PickedBy").Include("ReturnedBy").Where(x => !(x is JobAddon) && x.ReturnCompleted.HasValue && x.ReturnCompleted.Value >= DateTime.Today).ToList();
        }
        private List<Job> GetBeingPicked()
        {
            return Context.Jobs.Include("ProjectManager").Include("PickedBy").Include("ReturnedBy").Where(x => x.Status == JobStatus.BeingPicked).OrderBy(x => x.PickStarted).ToList();
        }
        private List<Job> GetBeingReturned()
        {
            return Context.Jobs.Include("ProjectManager").Include("PickedBy").Include("ReturnedBy").Where(x => !(x is JobAddon) && x.Status == JobStatus.BeingReturned).OrderBy(x => x.ReturnStarted).ToList();
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

        public virtual ActionResult AssetHistory()
        {
            ViewBag.Users = Context.Users.ToList();
            return View();
        }

        public virtual ActionResult AssetHistoryFilter(string id = null, int? userId = null, DateTime? date = null, DateTime? startDate = null, DateTime? endDate = null)
        {
            if (date.HasValue)
            {
                startDate = date.Value;
                endDate = date.Value.AddDays(1);
            }


            var result = Context.AssetActivity
                                .Include(x => x.Asset)
                                .Include(x => x.Item)
                                .Include(x => x.Job)
                                .Include(x => x.User);

            if (!String.IsNullOrEmpty(id))
                result = result.Where(x => x.Asset.Barcode == id);
            if (userId.HasValue)
                result = result.Where(x => x.User.Id == userId.Value);
            if (startDate.HasValue)
                result = result.Where(x => x.Date >= startDate);
            if (endDate.HasValue)
                result = result.Where(x => x.Date < endDate);
            ViewBag.Users = Context.Users.ToList();

            return View("AssetHistory", result.OrderBy(x => x.Date).ToList());
        }

        public virtual ActionResult FellOffTruck(DateTime? startDate = null, DateTime? endDate = null)
        {
            if (!startDate.HasValue)
                startDate = DateTime.Today.AddDays(-7);
            if (!endDate.HasValue)
                endDate = DateTime.Today;

            endDate = endDate.Value.AddDays(1).AddSeconds(-1);

            var pickedAssets = Context.PickedAssets
                .Include(x => x.Job).Include(x => x.Job.ReturnedBy).Include(x => x.Item).Include(x => x.Asset)
                .Where(x => x.Job.ReturnCompleted >= startDate && x.Job.ReturnCompleted < endDate)
                .ToList()
                .Where(x=>x.Job != null)
                .GroupBy(x => x.Job);

            var jobIds = pickedAssets.Select(x=>x.Key.Id);

            var returnedAssets = Context.ReturnedAssets
                   .Include(x => x.Job).Include(x => x.Job.ReturnedBy).Include(x => x.Item).Include(x => x.Asset)
                   .Where(x=> jobIds.Contains(x.Job.Id))
                   .ToList()
                   .GroupBy(x => x.Job);

            IList<AssetsWithoutReturn> result = new List<AssetsWithoutReturn>();
            foreach (var pickedAssetsForJob in pickedAssets)
            {
                if (pickedAssetsForJob.Key == null)
                    continue;
                var returnedAssetsForJob = returnedAssets.FirstOrDefault(x => x.Key == pickedAssetsForJob.Key);
                foreach (var pickedAsset in pickedAssetsForJob)
                {
                    bool fellOfTruck = false;
                    int? quantityLost = null;

                    if (returnedAssetsForJob == null || !returnedAssetsForJob.Any())
                    {
                        fellOfTruck = true;
                        quantityLost = pickedAsset.Quantity;
                    }
                    else if (pickedAsset.Asset.Kind == AssetKind.Serialized)
                    {
                        fellOfTruck = returnedAssetsForJob.Any(x => x.Item.Barcode == pickedAsset.Item.Barcode);
                    }
                    else
                    {
                        foreach (var returnedAsset in returnedAssetsForJob.Where(x => x.Asset.Kind == AssetKind.NotSerialized && x.Id == pickedAsset.Id))
                        {
                            if (returnedAsset.Quantity < pickedAsset.Quantity)
                            {
                                fellOfTruck = true;
                                quantityLost = pickedAsset.Quantity - returnedAsset.Quantity;
                            }
                        }
                    }

                    if (fellOfTruck)
                    {
                        result.Add(new AssetsWithoutReturn(pickedAsset.Id, pickedAsset.Asset.Name, pickedAsset.Item, pickedAssetsForJob.Key, quantityLost));
                    }
                }
            }

            return View(result);
        }

        public virtual ActionResult TomorrowsPicks()
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
                     IsAddon = x is JobAddon,
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
                    IsAddon = result.IsAddon,
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
