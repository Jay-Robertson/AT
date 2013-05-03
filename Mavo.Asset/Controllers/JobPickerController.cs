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
using System.Net;
namespace Mavo.Assets.Controllers
{
    [Authorize]
    public partial class JobPickerController : BaseController
    {

        private readonly IAssetActivityManager AssetActivity;
        private readonly AssetContext Context;
        
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

        private Job __GetJobForPickingActions(int jobId, bool picking = false)
        {
            // find the job by id
            var job = Context.Jobs
                .Include("Assets").Include("Assets.Asset").Include("Assets.Asset.Category")
                .Include("PickedAssets").Include("PickedAssets.Item").Include("PickedAssets.Asset")
                .Include("ProjectManager")
                .FirstOrDefault(x => x.Id == jobId);

            // mark it as being picked if needed
            if (picking && job.Status != JobStatus.BeingPicked)
            {
                job.Status = JobStatus.BeingPicked;
                job.PickStarted = DateTime.Now;
                Context.SaveChanges();
            }

            return job;
        }

        [HttpPost]
        public virtual ActionResult PickSerialized(int jobId, string barcode)
        {
            var job = __GetJobForPickingActions(jobId, true);
            var assetItem = Context.Lookup(barcode);

            // validate barcode, stock, and repair
            if (null == assetItem)
            {
                Response.StatusCode = (int)HttpStatusCode.NotFound;
                return Content(String.Format("{0} does not exist in inventory.", barcode));
               
            }
            if (assetItem.Status != InventoryStatus.In)
            {
                Response.StatusCode = (int)HttpStatusCode.MethodNotAllowed;
                return Content(String.Format("{0} is already out on a job.", barcode));
            }
            if (assetItem.Condition != AssetCondition.Good)
            {
                Response.StatusCode = (int)HttpStatusCode.MethodNotAllowed;
                return Content(String.Format("{0} is damaged or retired.", barcode));
            }

            // validate that this asset is part of the job
            var ask = job.Assets.FirstOrDefault(x => x.Asset.Id == assetItem.Asset.Id);
            if (null == ask)
            {
                Response.StatusCode = (int)HttpStatusCode.MethodNotAllowed;
                return Content(String.Format("{0} is not needed for this job.", assetItem.Asset.Name));
            }

            // count the existing picked 
            var quantityPicked = job.GetQuantityPicked(assetItem.Asset);
            if (quantityPicked >= ask.Quantity)
            {
                Response.StatusCode = (int)HttpStatusCode.MethodNotAllowed;
                return Content(String.Format("You don't need to pick anymore {0} for this job.", ask.Asset.Name));
            }
            quantityPicked++;

            // this thing is now out on the job!
            assetItem.Status = InventoryStatus.Out;

            // create the new picked asset
            job.PickedAssets.Add(new PickedAsset
            {
                Asset = assetItem.Asset,
                Barcode = assetItem.Barcode,
                Item = assetItem,
                Quantity = 1,
                QuantityPicked = 1,
                Picked = DateTime.Now,
                Job = job,
            });
            AssetActivity.Add(AssetAction.Pick, assetItem.Asset, assetItem, job);
            Context.SaveChanges();

            return PartialView(
                MVC.JobPicker.Views._PickedAssetRow,
                new PickedAssetRow {
                    MavoNumber = assetItem.Asset.MavoItemNumber,
                    AssetId = assetItem.Asset.Id,
                    AssetName = assetItem.Asset.Name,
                    Barcodes = assetItem.Barcode,
                    AssetItemId = assetItem.Id
                });
        }

        [HttpPost]
        public virtual ActionResult PickNonSerialized(int jobId, int assetId, int quantity)
        {
            var job = __GetJobForPickingActions(jobId, true);
            var asset = Context.Assets.Single(x => x.Id == assetId);

            // validate that this asset is part of the job
            var ask = job.Assets.FirstOrDefault(x => x.Asset.Id == asset.Id);
            if (null == ask)
            {
                Response.StatusCode = (int)HttpStatusCode.MethodNotAllowed;
                return Content(String.Format("{0} is not needed for this job.", asset.Name));
            }

            // count the existing picked 
            var quantityPicked = job.GetQuantityPicked(asset);
            if (quantityPicked >= ask.Quantity)
            {
                Response.StatusCode = (int)HttpStatusCode.MethodNotAllowed;
                return Content(String.Format("You don't need to pick anymore {0} for this job.", ask.Asset.Name));
            }
            quantityPicked += quantity;
            
            // update the picked quantity
            var picked = job.PickedAssets.SingleOrDefault(x => x.Asset.Id == asset.Id);
            if (null == picked)
            {
                picked = new PickedAsset
                {
                    Job = job,
                    Asset = asset,
                };
                job.PickedAssets.Add(picked);
            }
            picked.Picked = DateTime.Now;
            picked.Quantity = quantityPicked;
            picked.QuantityPicked = quantityPicked;
            asset.Inventory -= quantity;

            Context.SaveChanges();

            return PartialView(
                MVC.JobPicker.Views._PickedAssetRow,
                new PickedAssetRow
                {
                    MavoNumber = asset.MavoItemNumber,
                    AssetId = asset.Id,
                    AssetName = asset.Name,
                    CurrentPickedQty = picked.QuantityPicked,
                });
        }

        [HttpPost]
        public virtual ActionResult PickAssetForJob(int jobId, int assetId, int quantity = 1, string barcode = null)
        {
            //Job job = Context.Jobs.Include(x => x.Assets).Include("Assets.Asset").Include(x => x.PickedAssets).FirstOrDefault(x => x.Id == jobId);
            //if (job.Status == JobStatus.ReadyToPick)
            //    StartPicking(jobId);

            //var pickedAsset = PickAsset(job, new JobAsset() { AssetId = assetId, QuantityTaken = quantity, Barcode = barcode, Kind = Context.Assets.First(x => x.Id == assetId).Kind });
            //Context.SaveChanges();
            //return PartialView(MVC.JobPicker.Views._PickedAssetRow, new PickedAssetRow { MavoNumber = pickedAsset.Asset.MavoItemNumber, AssetId = assetId, AssetName = pickedAsset.Asset.Name, CurrentPickedQty = pickedAsset.Quantity });
            throw new NotImplementedException();
        }

        //private PickedAsset PickAsset(Job job, JobAsset x)
        //{

        //    var pickAsset = new PickedAsset()
        //              {
        //                  Asset = Context.Assets.FirstOrDefault(a => a.Id == x.AssetId),
        //                  Item = !String.IsNullOrEmpty(x.Barcode) ? Context.AssetItems.FirstOrDefault(ai => x.Barcode == ai.Barcode) : null,
        //                  Job = job,
        //                  Picked = DateTime.Now,
        //                  Quantity = Math.Max(x.QuantityTaken ?? 1, 1),
        //                  Barcode = x.Barcode
        //              };
        //    if (x.Kind != AssetKind.Serialized && job.PickedAssets.Any(a => a.Asset.Id == x.AssetId))
        //    {
        //        pickAsset = Context.PickedAssets.First(a => a.Asset.Id == x.AssetId);
        //        pickAsset.Quantity += x.QuantityTaken.Value;
        //    }
        //    else
        //        Context.PickedAssets.Add(pickAsset);

        //    foreach (var jobAsset in job.Assets)
        //    {
        //        if (jobAsset.Asset.Id == pickAsset.Asset.Id)
        //        {
        //            jobAsset.QuantityPicked += pickAsset.Quantity;
        //        }
        //    }

        //    Asset asset = Context.Assets.FirstOrDefault(y => y.Id == pickAsset.Asset.Id);
        //    if (asset.Kind == AssetKind.Consumable || asset.Kind == AssetKind.NotSerialized && asset.Inventory.HasValue)
        //        asset.Inventory = Convert.ToInt32(Math.Max((decimal)((asset.Inventory ?? 0) - pickAsset.Quantity), 0m));
        //    else if (asset.Kind == AssetKind.Serialized)
        //        pickAsset.Item.Status = InventoryStatus.Out;

        //    AssetActivity.Add(AssetAction.Pick, pickAsset.Asset, pickAsset.Item, job);

        //    return pickAsset;
        //}

        [HttpPost]
        public virtual ActionResult Index(int id, IList<JobAsset> assets)
        {
            //Job job = Context.Jobs.Include(x => x.Assets).Include("Assets.Asset").FirstOrDefault(x => x.Id == id);

            //IEnumerable<JobAsset> pickedAssets = null;
            //if (assets != null)
            //{
            //    pickedAssets = assets.Where(x => (x.Kind == AssetKind.Serialized && !String.IsNullOrEmpty(x.Barcode))
            //                        || ((x.Kind == AssetKind.NotSerialized || x.Kind == AssetKind.Consumable) && (x.QuantityTaken.HasValue && x.QuantityTaken.Value > 0)));
            //    foreach (var pickedAsset in pickedAssets)
            //    {
            //        PickAsset(job, pickedAsset);
            //    }
            //}

            //return CompletePicking(id);

            throw new NotImplementedException();
        }

        [HttpGet]
        public virtual ActionResult Success(int id)
        {
            return View();
        }

        [HttpPost]
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

                // move all added on assets into the parent job
                foreach (var a in tempJob.PickedAssets)
                {
                    a.Job = job;
                }
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

        [HttpGet]
        public virtual ActionResult Index(int id, bool isTablet = false)
        {
            var job = __GetJobForPickingActions(id);

            //if (result.PickCompleted.HasValue)
            //    return RedirectToAction(MVC.Reporting.Jobs());

            var viewModel = new PickAJobModel()
            {
                JobId = job.Id,
                JobName = job.Name,
                JobNumber = job.JobNumber,
                Manager = job.ProjectManager == null ? "" : job.ProjectManager.FullName,
                Foreman = job.Foreman == null ? "" : job.Foreman.FullName,
                Customer = job.Customer == null ? "" : job.Customer.Name,
                PickStarted = job.PickStarted,
                ReturnStarted = job.ReturnStarted,
                Address = job.Address,
                PickupTime = job.PickupTime,
                CompletionDate = job.EstimatedCompletionDate
            };
            viewModel.Assets = job.Assets.Select(x => new JobAsset
            {
                Name = x.Asset.Name,
                Id = x.Id,
                AssetId = x.Asset.Id,
                QuantityNeeded = x.Quantity - job.GetQuantityPicked(x.Asset),
                QuantityTaken = job.GetQuantityPicked(x.Asset),
                Kind = x.Asset.Kind,
                NotEnoughQuantity = (x.Quantity - job.GetQuantityPicked(x.Asset)) > x.Asset.Inventory,
                QuantityAvailable = x.Asset.Inventory,
                AssetCategory = x.Asset.Category.Name,
                MavoItemNumber = x.Asset.MavoItemNumber
            }).ToList();
            viewModel.PickedAssets = job.PickedAssets.Select(x => new JobAsset
            {
                Name = x.Asset.Name,
                Id = x.Id,
                AssetId = x.Asset.Id,
                QuantityTaken = x.Quantity,
                Kind = x.Asset.Kind,
                AssetCategory = x.Asset.Category.Name,
                MavoItemNumber = x.Asset.MavoItemNumber,
                Barcode = x.Barcode,
                AssetItemId = x.Item == null ? (int?)null : x.Item.Id
            }).ToList();
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
