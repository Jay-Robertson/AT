using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Mavo.Assets.Models;
using Mavo.Assets.Models.ViewModel;
using Mavo.Assets.Services;
using Postal;
using System.Data.Entity;
using System.Diagnostics;
namespace Mavo.Assets.Controllers
{
    [Authorize]
    public partial class JobPickerController : BaseController
    {

        private readonly IAssetActivityManager AssetActivity;
        private readonly AssetContext Context;
        /// <summary>
        /// Initializes a new instance of the JobPickerController class.
        /// </summary>
        public JobPickerController(AssetContext context, IAssetActivityManager assetActivity)
        {
            AssetActivity = assetActivity;
            Context = context;
        }

        [HttpPost]
        public virtual ActionResult Cancel(int id)
        {
            Job job = Context.Jobs.FirstOrDefault(x => x.Id == id);
            job.PickStarted = null;
            job.Status = JobStatus.ReadyToPick;
            job.PickedBy = null;
            Context.SaveChanges();
            return PartialView("~/Views/Shared/DisplayTemplates/JobStatus.cshtml", job.Status);
        }
        [HttpPost]
        public virtual JsonResult StartPicking(int id)
        {
            Job job = Context.Jobs.FirstOrDefault(x => x.Id == id);
            job.PickStarted = DateTime.Now;
            job.Status = JobStatus.BeingPicked;
            job.PickedBy = Context.Users.FirstOrDefault(x => x.Email == User.Identity.Name);
            Context.SaveChanges();
            return Json(job.PickStarted.ToString());
        }

        [HttpPost]
        public virtual ActionResult PickAssetForJob(int jobId, int assetId, int quantity = 1, string barcode = null)
        {
            Job job = Context.Jobs.Include(x => x.Assets).Include("Assets.Asset").Include(x => x.PickedAssets).FirstOrDefault(x => x.Id == jobId);
            if (job.Status == JobStatus.ReadyToPick)
                StartPicking(jobId);

            var pickedAsset = PickAsset(job, new JobAsset() { AssetId = assetId, QuantityTaken = quantity, Barcode = barcode, Kind = Context.Assets.First(x => x.Id == assetId).Kind });
            Context.SaveChanges();
            return PartialView(MVC.JobPicker.Views._PickedAssetRow, new PickedAssetRow { MavoNumber = pickedAsset.Asset.MavoItemNumber, AssetId = assetId, AssetName = pickedAsset.Asset.Name, CurrentPickedQty = pickedAsset.Quantity });
        }


        private PickedAsset PickAsset(Job job, JobAsset x)
        {

            var pickAsset = new PickedAsset()
                      {
                          Asset = Context.Assets.FirstOrDefault(a => a.Id == x.AssetId),
                          Item = !String.IsNullOrEmpty(x.Barcode) ? Context.AssetItems.FirstOrDefault(ai => x.Barcode == ai.Barcode) : null,
                          Job = job,
                          Picked = DateTime.Now,
                          Quantity = Math.Max(x.QuantityTaken ?? 1, 1),
                          Barcode = x.Barcode
                      };
            if (x.Kind != AssetKind.Serialized && job.PickedAssets.Any(a => a.Asset.Id == x.AssetId))
            {
                pickAsset = Context.PickedAssets.First(a => a.Asset.Id == x.AssetId);
                pickAsset.Quantity += x.QuantityTaken.Value;
            }
            else
                Context.PickedAssets.Add(pickAsset);

            foreach (var jobAsset in job.Assets)
            {
                if (jobAsset.Asset.Id == pickAsset.Asset.Id)
                {
                    jobAsset.QuantityPicked += pickAsset.Quantity;
                }
            }

            Asset asset = Context.Assets.FirstOrDefault(y => y.Id == pickAsset.Asset.Id);
            if (asset.Kind == AssetKind.Consumable || asset.Kind == AssetKind.NotSerialized && asset.Inventory.HasValue)
                asset.Inventory = Convert.ToInt32(Math.Max((decimal)((asset.Inventory ?? 0) - pickAsset.Quantity), 0m));
            else if (asset.Kind == AssetKind.Serialized)
                pickAsset.Item.Status = InventoryStatus.Out;

            AssetActivity.Add(AssetAction.Pick, pickAsset.Asset, pickAsset.Item, job);

            return pickAsset;
        }
        [HttpPost]
        public virtual ActionResult Index(int id, IList<JobAsset> assets)
        {
            Job job = Context.Jobs.Include(x => x.Assets).Include("Assets.Asset").FirstOrDefault(x => x.Id == id);

            IEnumerable<JobAsset> pickedAssets = null;
            if (assets != null)
            {
                pickedAssets = assets.Where(x => (x.Kind == AssetKind.Serialized && !String.IsNullOrEmpty(x.Barcode))
                                    || ((x.Kind == AssetKind.NotSerialized || x.Kind == AssetKind.Consumable) && (x.QuantityTaken.HasValue && x.QuantityTaken.Value > 0)));
                foreach (var pickedAsset in pickedAssets)
                {
                    PickAsset(job, pickedAsset);
                }
            }

            return CompletePicking(id);
        }
        public virtual ActionResult Success(int id)
        {


            return View();
        }
        public virtual ActionResult CompletePicking(int id)
        {
            JobAddon tempJob = Context.JobAddons
                .Include("Assets").Include("Assets.Asset").Include("PickedAssets").Include("PickedAssets.Asset")
                .Include("ParentJob").FirstOrDefault(x => x.Id == id);
            Job job = null;
            if (tempJob != null)
            {
                tempJob.IsPicked = true;
                tempJob.Status = JobStatus.Started;
                tempJob.PickCompleted = DateTime.Now;

                job = tempJob.ParentJob;
            }
            else
            {
                job = Context.Jobs
                    .Include("Assets").Include("Assets.Asset").Include("PickedAssets").Include("PickedAssets.Asset")
                    .FirstOrDefault(x => x.Id == id);
                job.Status = JobStatus.Started;
                job.PickCompleted = DateTime.Now;
            }

            job.PickedBy = Context.Users.FirstOrDefault(x => x.Email == User.Identity.Name);

            Context.SaveChanges();

            dynamic email = new Email("JobHasBeenPicked");
            email.Subject = String.Format("Job #{0} has been picked", job.JobNumber);
            email.To = Properties.Settings.Default.WarehouseManager;
            email.Job = job;
            email.Send();
            var shorts = new List<MissingItem>();
            if (job.Assets != null && job.Assets.Any())
            {
                foreach (var requestedAsset in job.Assets)
                {
                    int requestedAmount = requestedAsset.Quantity;
                    int pickedAmount = requestedAsset.QuantityPicked;

                    if (pickedAmount < requestedAmount)
                    {
                        shorts.Add(new MissingItem(requestedAsset.Asset.MavoItemNumber, requestedAmount - pickedAmount, requestedAsset.Asset.Name));
                    }
                }
            }
            if (shorts.Any())
            {
                dynamic shortEmail = new Email("Shorts");
                shortEmail.Subject = String.Format("Job #{0} is short some items", job.JobNumber);
                shortEmail.Shorts = shorts;
                shortEmail.To = Properties.Settings.Default.WarehouseManager;
                shortEmail.Job = job;
                shortEmail.Send();
            }

            return RedirectToAction(MVC.JobPicker.Success(id));
        }
        public virtual ActionResult Index(int id, bool isTablet = false)
        {
            var result = Context.Jobs.Include("Assets").Include("Assets.Asset").Include("PickedAssets").Include("PickedAssets.Asset")
                .Where(x => x.Id == id).Select(x =>
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
                }).First();
            if (result.PickCompleted.HasValue)
                return RedirectToAction(MVC.Reporting.Jobs());

            PickAJobModel viewModel = new PickAJobModel()
                        {
                            JobId = result.JobId,
                            JobName = result.JobName,
                            JobNumber = result.JobNumber,
                            Manager = String.Format("{0} {1}", result.ManagerFirstName, result.ManagerLastName),
                            Foreman = String.Format("{0} {1}", result.ForemanFirstName, result.ForemanLastName),
                            Customer = result.Customer,
                            PickStarted = result.PickStarted,
                            ReturnStarted = result.ReturnStarted,
                            Address = result.Address,
                            CompletionDate = result.CompletionDate,
                            Assets = result.Assets.Select(x => new JobAsset()
                            {
                                Name = x.Name,
                                Id = x.Id,
                                AssetId = x.AssetId,
                                QuantityNeeded = x.Quantity - x.QuantityPicked,
                                QuantityTaken = x.QuantityPicked,
                                Kind = x.Kind,
                                NotEnoughQuantity = x.NotEnoughQuantity,
                                QuantityAvailable = x.QuantityAvailable,
                                AssetCategory = x.AssetCategory,
                                MavoItemNumber = x.MavoItemNumber
                            }).ToList()
                        };
            return View("TabletPicker", viewModel);
        }
    }
    [DebuggerDisplay("\\{ MavoItemNumber = {MavoItemNumber}, NumberShort = {NumberShort}, AssetName = {AssetName} \\}")]
    public sealed class MissingItem : IEquatable<MissingItem>
    {
        private readonly string _MavoItemNumber;
        private readonly int _NumberShort;
        private readonly string _AssetName;

        public MissingItem(string mavoItemNumber, int numberShort, string assetName)
        {
            _MavoItemNumber = mavoItemNumber;
            _NumberShort = numberShort;
            _AssetName = assetName;
        }

        public override bool Equals(object obj)
        {
            if (obj is MissingItem)
                return Equals((MissingItem)obj);
            return false;
        }
        public bool Equals(MissingItem obj)
        {
            if (obj == null)
                return false;
            if (!EqualityComparer<string>.Default.Equals(_MavoItemNumber, obj._MavoItemNumber))
                return false;
            if (!EqualityComparer<int>.Default.Equals(_NumberShort, obj._NumberShort))
                return false;
            if (!EqualityComparer<string>.Default.Equals(_AssetName, obj._AssetName))
                return false;
            return true;
        }
        public override int GetHashCode()
        {
            int hash = 0;
            hash ^= EqualityComparer<string>.Default.GetHashCode(_MavoItemNumber);
            hash ^= EqualityComparer<int>.Default.GetHashCode(_NumberShort);
            hash ^= EqualityComparer<string>.Default.GetHashCode(_AssetName);
            return hash;
        }
        public override string ToString()
        {
            return String.Format("{{ MavoItemNumber = {0}, NumberShort = {1}, AssetName = {2} }}", _MavoItemNumber, _NumberShort, _AssetName);
        }

        public string MavoItemNumber
        {
            get
            {
                return _MavoItemNumber;
            }
        }
        public int NumberShort
        {
            get
            {
                return _NumberShort;
            }
        }
        public string AssetName
        {
            get
            {
                return _AssetName;
            }
        }
    }
}
