using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Web.Mvc;
using System.Text;

using CsvHelper;
using CsvHelper.Configuration;

using Mavo.Assets.Models;
using Mavo.Assets.Models.ViewModel;

namespace Mavo.Assets.Controllers
{
    class CsvResult<TRow> : ActionResult where TRow : class
    {
        private String _filename;
        private IEnumerable<TRow> _rows;

        public CsvResult(string filename, IEnumerable<TRow> rows)
        {
            _filename = filename;
            _rows = rows;
        }

        public override void ExecuteResult(ControllerContext context)
        {
            context.HttpContext.Response.ContentType = "application/csv";
            context.HttpContext.Response.AddHeader(
                "Content-disposition",
                String.Format("attachment; filename=\"{0}\"", _filename)
            );
            using (var csv = new CsvWriter(context.HttpContext.Response.Output))
            {
                csv.Configuration.HasHeaderRecord = true;
                csv.WriteRecords(_rows);
            }
        }
    }

    class __AssetExportRow
    {
        [CsvField(Index=1)] public string MavoItemNumber { get; set; }
        [CsvField(Index=0)] public string Category { get; set; }
        [CsvField(Index=2)] public string Name { get; set; }
        [CsvField(Index=3)] public AssetKind Kind { get; set; }
        [CsvField(Index=4)] public string Barcode { get; set; }
        [CsvField(Index=5)] public AssetCondition? Condition { get; set; }
        [CsvField(Index=6)] public int? Quantity { get; set; }
        [CsvField(Index=7)] public string UnitOfMeasure { get; set; }
    }

    [System.Web.Mvc.Authorize]
    public partial class ReportingController : BaseController
    {
        private readonly AssetContext Context;
        public ReportingController(AssetContext context)
        {
            Context = context;
        }

        public virtual ActionResult AssetExport()
        {
            var rows = new List<__AssetExportRow>();
            rows.AddRange(
                Context.Assets.Where(x => x.Kind != AssetKind.Serialized).Select(x => 
                    new __AssetExportRow {
                        MavoItemNumber = x.MavoItemNumber,
                        Category = x.Category.Name,
                        Name = x.Name,
                        Kind = x.Kind,
                        Barcode = null,
                        Condition = null,
                        Quantity = x.Inventory,
                        UnitOfMeasure = x.UnitOfMeasure
                    }
                )
            );
            rows.AddRange(
                Context.AssetItems.Include("Asset").Select(x => 
                    new __AssetExportRow {
                        MavoItemNumber = x.Asset.MavoItemNumber,
                        Category = x.Asset.Category.Name,
                        Name = x.Asset.Name,
                        Kind = x.Asset.Kind,
                        Barcode = x.Barcode,
                        Condition = x.Condition,
                        Quantity = null,
                        UnitOfMeasure = x.Asset.UnitOfMeasure
                    }
                )
            );
            return new CsvResult<__AssetExportRow>(
                String.Format("mavo-assets-{0:yyyy-MM-dd}.csv", DateTime.Now),
                rows
            );
        }

        public virtual ActionResult LateJobs()
        {
            return View(Context.Jobs.Where(x => !(x is JobAddon) && x.EstimatedCompletionDate < DateTime.Today && x.Status == JobStatus.Started).ToList());
        }
        private List<Job> GetReadyToPick()
        {
            return Context.Jobs
                .Include("ProjectManager").Include("PickedBy").Include("ReturnedBy")
                .Where(x => x.Status == JobStatus.ReadyToPick)
                .OrderBy(x => x.PickupTime)
                .ToList()
                .Where(x => x.PickupTime.Date <= DateTime.Now.Date.AddDays(7))
                .ToList();
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

        private List<Job> GetFutureJobs()
        {
            return Context.Jobs
                .Include("ProjectManager").Include("PickedBy").Include("ReturnedBy")
                .Where(x => x.Status == JobStatus.ReadyToPick)
                .ToList()
                .Where(x => x.PickupTime.Date > DateTime.Now.Date.AddDays(7))
                .ToList();
        }

        public virtual ActionResult Jobs()
        {
            JobReportViewModel result = new JobReportViewModel()
                {
                    ReadyToPick = GetReadyToPick(),
                    AlreadyPicked = GetAlreadyPicked(),
                    AlreadyReturned = GetAlreadyReturned(),
                    BeingPicked = GetBeingPicked(),
                    BeingReturned = GetBeingReturned(),
                    FutureJobs = GetFutureJobs()
                };
            return View(result);
        }

        public virtual ActionResult AssetHistory()
        {
            ViewBag.Users = Context.Users.OrderBy(x=>x.LastName).ThenBy(x=>x.FirstName).ToList();
            return View();
        }

        public virtual ActionResult AssetHistoryFilter(string id = null, int? userId = null, DateTime? date = null, DateTime? startDate = null, DateTime? endDate = null, int? job = null)
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
            if (job.HasValue)
                result = result.Where(x => x.Job.Id == job.Value);
            if (!String.IsNullOrEmpty(id))
                result = result.Where(x => x.Item.Barcode == id);
            if (userId.HasValue)
                result = result.Where(x => x.User.Id == userId.Value);
            if (startDate.HasValue)
                result = result.Where(x => x.Date >= startDate);
            if (endDate.HasValue)
                result = result.Where(x => x.Date < endDate);
            ViewBag.Users = Context.Users.OrderBy(x => x.LastName).ThenBy(x => x.FirstName).ToList();

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
                .Where(x => x.Item.Status == InventoryStatus.Out &&
                            x.Job.Status == JobStatus.Completed &&
                            x.Job.ReturnCompleted >= startDate &&
                            x.Job.ReturnCompleted < endDate)
                .ToList()
                .Where(x => x.Job != null)
                .GroupBy(x => x.Job);

            var jobIds = pickedAssets.Select(x => x.Key.Id);

            var returnedAssets = Context.ReturnedAssets
                   .Include(x => x.Job).Include(x => x.Job.ReturnedBy).Include(x => x.Item).Include(x => x.Asset)
                   .Where(x => jobIds.Contains(x.Job.Id))
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
                        fellOfTruck = !returnedAssetsForJob.Any(x => x.Item != null && x.Item.Barcode == pickedAsset.Item.Barcode);
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
                        result.Add(new AssetsWithoutReturn(pickedAsset.Asset.Id, pickedAsset.Asset.MavoItemNumber, pickedAsset.Asset.Name, pickedAsset.Item, pickedAssetsForJob.Key, quantityLost));
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
                     ForemanFirstName = x.Foreman.FirstName,
                     ForemanLastName = x.Foreman.LastName,
                     PickStarted = x.PickStarted,
                     PickCompleted = x.PickCompleted,
                     ReturnStarted = x.ReturnStarted,
                     ReturnCompleted = x.ReturnCompleted,
                     PickupTime = x.PickupTime,
                     IsAddon = x is JobAddon,
                     Address = x.Address,
                     CompletionDate = x.EstimatedCompletionDate,
                     Assets = x.Assets.Select(a => new
                     {
                         Name = a.Asset.Name,
                         Id = a.Id,
                         Quantity = a.Quantity,
                         Kind = a.Asset.Kind,
                         AssetId = a.Asset.Id,
                         NotEnoughQuantity = a.Quantity > (a.Asset.Inventory ?? 0),
                         QuantityAvailable = a.Asset.Inventory,
                         AssetCategory = a.Asset.Category.Name,
                         MavoItemNumber = a.Asset.MavoItemNumber,
                         QuantityPicked = a.QuantityPicked
                     })
                 }).ToList();


            return View(list.Select(result =>
                new PickAJobModel()
                {
                    JobId = result.JobId,
                    JobName = result.JobName,
                    JobNumber = result.JobNumber,
                    Manager = String.Format("{0} {1}", result.ManagerFirstName, result.ManagerLastName),
                    Foreman = String.Format("{0} {1}", result.ForemanFirstName, result.ForemanLastName),
                    Customer = result.Customer,
                    PickStarted = result.PickStarted,
                    ReturnStarted = result.ReturnStarted,
                    PickupTime = result.PickupTime,
                    IsAddon = result.IsAddon,
                    Address = result.Address,
                    CompletionDate = result.CompletionDate,
                    Assets = result.Assets.Select(x => new JobAsset()
                    {
                        Name = x.Name,
                        Id = x.Id,
                        AssetId = x.AssetId,
                        QuantityNeeded = x.Quantity - x.QuantityPicked,
                        QuantityTaken = x.Quantity,
                        Kind = x.Kind,
                        NotEnoughQuantity = x.NotEnoughQuantity,
                        QuantityAvailable = x.QuantityAvailable,
                        AssetCategory = x.AssetCategory,
                        MavoItemNumber = x.MavoItemNumber
                    }).ToList()
                }));

        }

        public virtual ActionResult ItemsNotPicked(DateTime? startDate = null, DateTime? endDate = null)
        {
            if (!startDate.HasValue) startDate = DateTime.Today.AddDays(-7);
            if (!endDate.HasValue) endDate = DateTime.Today;

            var list = Context.Jobs
                .Include(x => x.PickedAssets)
                .Include("PickedAssets.Asset")
                .Include("Assets")
                .Include("Assets.Asset")
                .Where(x => x.PickupTime > startDate &&
                            x.PickupTime <= endDate &&
                            x.PickedAssets.Count != x.Assets.Count);

            List<ItemNotPicked> result = new List<ItemNotPicked>();

            foreach (var item in list)
            {
                ItemNotPicked newItemNotPicked = new ItemNotPicked() { Job = item, NotPicked = new List<NotPickedItem>() };
                foreach (var requestedAsset in item.Assets)
                {
                    int picked = 0;
                    int requested = requestedAsset.Quantity;
                    if (requestedAsset.Asset.Kind == AssetKind.Serialized)
                    {
                        picked = item.PickedAssets.Count(x => x.Asset.Id == requestedAsset.Asset.Id);
                    }
                    else
                    {
                        PickedAsset pickedAsset = item.PickedAssets.FirstOrDefault(x => x.Asset.Id == requestedAsset.Asset.Id);
                        if (pickedAsset != null)
                            picked = pickedAsset.Quantity;
                    }
                    if (requested != picked)
                    {
                        newItemNotPicked.NotPicked.Add(new NotPickedItem()
                        {
                            AssetId = requestedAsset.Asset.Id,
                            AssetName = requestedAsset.Asset.Name,
                            Picked = picked,
                            Requested = requested
                        });
                    }
                }
                result.Add(newItemNotPicked);
            }

            return View(result);
        }
    }
}
