﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Mavo.Assets.Models;
using Mavo.Assets.Models.EmailViewModel;
using Mavo.Assets.Models.ViewModel;
using Mavo.Assets.Services;
using Postal;

namespace Mavo.Assets.Controllers
{
    [Authorize]
    public partial class JobReturnerController : BaseController
    {
        private readonly IAssetActivityManager AssetActivity;
        private readonly AssetContext Context;
        private readonly ICurrentUserService CurrentUser;

        public JobReturnerController(AssetContext context, IAssetActivityManager assetActivity, ICurrentUserService currentUser)
        {
            CurrentUser = currentUser;
            AssetActivity = assetActivity;
            Context = context;
        }

        [HttpPost]
        public virtual ActionResult Cancel(int id)
        {
            Job job = Context.Jobs.FirstOrDefault(x => x.Id == id);
            job.ReturnStarted = null;
            job.Status = JobStatus.Started;
            job.ReturnedBy = null;
            Context.SaveChanges();
            return PartialView("~/Views/Shared/DisplayTemplates/JobStatus.cshtml", job.Status);
        }

        [HttpPost]
        public virtual JsonResult StartReturning(int id)
        {
            Job job = Context.Jobs.FirstOrDefault(x => x.Id == id);
            job.ReturnStarted = DateTime.Now;
            job.Status = JobStatus.BeingReturned;
            job.ReturnedBy = Context.Users.FirstOrDefault(x => x.Email == User.Identity.Name);
            Context.SaveChanges();
            return Json(job.ReturnStarted.ToString());
        }

        [HttpPost]
        public ActionResult ReturnAssetForJob(int jobId, int assetId, int? assetItemId, int quantity = 1, bool isDamaged = false)
        {
            var job = Context.Jobs
                .Include("ReturnedAssets").Include("ReturnedAssets.Item").Include("ReturnedAssets.Asset")
                .Include("PickedAssets").Include("PickedAssets.Item").Include("PickedAssets.Asset")
                .Include("ProjectManager")
                .FirstOrDefault(x => x.Id == jobId);

            if (job.Status == JobStatus.Started)
            {
                job.Status = JobStatus.BeingReturned;
                job.ReturnStarted = DateTime.Now;
            }

            var pickedAsset = assetItemId.HasValue
                ? job.PickedAssets.FirstOrDefault(x => x.Item.Id == assetItemId.Value)
                : job.PickedAssets.FirstOrDefault(x => x.Asset.Id == assetId);
             
            bool hasReturned = false;
            var asset = Context.Assets.FirstOrDefault(x => x.Id == pickedAsset.Asset.Id);
            int? quantityUsed = quantity;
            if (asset.Kind == AssetKind.Consumable || asset.Kind == AssetKind.NotSerialized)
            {
                hasReturned = true;

                if (!asset.Inventory.HasValue)
                    asset.Inventory = quantityUsed;
                else
                    asset.Inventory += quantityUsed;
            }
            else if (asset.Kind == AssetKind.Serialized)
            {
                hasReturned = true;
                pickedAsset.Item.Status = InventoryStatus.In;
                if (isDamaged)
                {
                    AssetActivity.Add(AssetAction.Damaged, pickedAsset.Asset, pickedAsset.Item, job);
                    pickedAsset.Item.Condition = AssetCondition.Damaged;
                }
            }
            ReturnedAsset newReturnedAsset = new ReturnedAsset() { Asset = asset, Item = pickedAsset.Item, Quantity = pickedAsset.QuantityPicked, QuantityPicked = quantity, Job = job, Returned = DateTime.Now };
            if (hasReturned)
            {
                if (job.ReturnedAssets != null && asset.Kind != AssetKind.Serialized && job.ReturnedAssets.Any(x => x.Asset.Id == asset.Id))
                {
                    newReturnedAsset = job.ReturnedAssets.FirstOrDefault(x => x.Asset.Id == asset.Id);
                    newReturnedAsset.QuantityPicked += quantity;
                }
                else
                {
                    if (job.ReturnedAssets == null)
                        job.ReturnedAssets = new List<ReturnedAsset>();

                    job.ReturnedAssets.Add(newReturnedAsset);
                }

                AssetActivity.Add(AssetAction.Return, pickedAsset.Asset, pickedAsset.Item, job);
            }
            Context.SaveChanges();


            return PartialView(MVC.JobPicker.Views._PickedAssetRow,
                 new PickedAssetRow()
                 {
                     AssetId = assetId,
                     AssetName = pickedAsset.Asset.Name,
                     CurrentPickedQty = newReturnedAsset.QuantityPicked,
                     MavoNumber = pickedAsset.Asset.MavoItemNumber
                 }
                );
        }

        [HttpPost]
        public virtual ActionResult Index(int id, IList<JobAsset> assets)
        {
            Job job = Context.Jobs
                .Include("PickedAssets").Include("PickedAssets.Item").Include("PickedAssets.Asset")
                .Include("ProjectManager").FirstOrDefault(x => x.Id == id);
            job.Status = JobStatus.Completed;
            job.ReturnCompleted = DateTime.Now;
            job.ReturnedAssets = new List<ReturnedAsset>();

            foreach (PickedAsset pickedAsset in job.PickedAssets)
            {
                bool hasReturned = false;
                Asset asset = Context.Assets.FirstOrDefault(x => x.Id == pickedAsset.Asset.Id);
                JobAsset incomingAsset = assets.FirstOrDefault(x => x.AssetId == asset.Id);
                int? quantityUsed = incomingAsset.QuantityTaken;
                if (asset.Kind == AssetKind.Consumable || asset.Kind == AssetKind.NotSerialized && asset.Inventory.HasValue)
                {
                    hasReturned = true;
                    asset.Inventory += quantityUsed;
                }
                else if (asset.Kind == AssetKind.Serialized && !String.IsNullOrEmpty(incomingAsset.Barcode))
                {
                    hasReturned = true;
                    pickedAsset.Item.Status = InventoryStatus.In;
                    if (incomingAsset.IsDamaged)
                    {
                        AssetActivity.Add(AssetAction.Damaged, pickedAsset.Asset, pickedAsset.Item, job);
                        pickedAsset.Item.Condition = AssetCondition.Damaged;
                    }
                }
                if (hasReturned)
                {
                    job.ReturnedAssets.Add(new ReturnedAsset()
                    {
                        Asset = asset,
                        Item = pickedAsset.Item,
                        Quantity = quantityUsed ?? 0,
                        Job = job,
                        Returned = job.ReturnCompleted.Value
                    });

                    AssetActivity.Add(AssetAction.Return, pickedAsset.Asset, pickedAsset.Item, job);
                }
            }
            Context.SaveChanges();

            if (job.ProjectManager != null)
            {
                dynamic email = new Email("JobComplete");
                email.Subject = String.Format("Job #{0} is complete!", job.JobNumber);
                email.To = job.ProjectManager.Email;
                email.Job = job;
                email.Send();
            }

            return RedirectToAction(MVC.JobReturner.Success(id));
        }

        public virtual ActionResult Success(int id)
        {
            return View("Success");
        }

        public virtual ActionResult CompleteReturning(int jobId)
        {
            Job job = Context.Jobs.FirstOrDefault(x => x.Id == jobId);
            job.ReturnedBy = CurrentUser.GetCurrent();
            job.ReturnCompleted = DateTime.Now;
            job.Status = JobStatus.Completed;
            Context.SaveChanges();
            return Success(jobId);
        }

        [HttpGet]
        public virtual ActionResult Index(int id)
        {
            var result = Context.Jobs
                .Include("PickedAssets").Include("PickedAssets.Item").Include("PickedAssets.Asset")
                .Include("ReturnedAssets").Include("ReturnedAssets.Item").Include("ReturnedAssetsAssets.Asset")
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
                    ReturnStarted = x.ReturnStarted,
                    Address = x.Address,
                    CompletionDate = x.EstimatedCompletionDate,
                    ReturnedAssets = x.ReturnedAssets.Select(a => new
                    {
                        Name = a.Asset.Name,
                        Id = a.Id,
                        Quantity = a.Quantity,
                        Kind = a.Asset.Kind,
                        AssetId = a.Asset.Id,
                        Serial = a.Item.Barcode,
                        AssetCategory = a.Asset.Category.Name,
                        AssetItemId = (a.Item != null ? a.Item.Id : default(int)),
                        MavoItemNumber = a.Asset.MavoItemNumber,
                        QuantityReturned = a.QuantityPicked,
                    }),
                    Assets = x.PickedAssets.Select(a => new
                    {
                        Name = a.Asset.Name,
                        Id = a.Id,
                        Quantity = a.Quantity,
                        Kind = a.Asset.Kind,
                        AssetId = a.Asset.Id,
                        Serial = a.Item.Barcode,
                        AssetCategory = a.Asset.Category.Name,
                        AssetItemId = (a.Item != null ? a.Item.Id : default(int)),
                        MavoItemNumber = a.Asset.MavoItemNumber,
                        QuantityReturned = a.QuantityPicked
                    })
                }).First();

            return View("TabletReturner", new PickAJobModel()
            {
                JobId = result.JobId,
                JobName = result.JobName,
                JobNumber = result.JobNumber,
                Manager = String.Format("{0} {1}", result.ManagerFirstName, result.ManagerLastName),
                Foreman = String.Format("{0} {1}", result.ForemanFirstName, result.ForemanLastName),
                Customer = result.Customer,
                ReturnStarted = result.ReturnStarted,
                Address = result.Address,
                CompletionDate = result.CompletionDate,
                ReturnedAssets = result.ReturnedAssets.Select(x => new JobAsset()
                {
                    Name = x.Name,
                    Id = x.Id,
                    AssetId = x.AssetId,
                    AssetItemId = x.AssetItemId,
                    QuantityNeeded = x.Quantity - x.QuantityReturned,
                    QuantityReturned = x.QuantityReturned,
                    QuantityTaken = x.QuantityReturned,
                    Kind = x.Kind,
                    Barcode = x.Serial,
                    AssetCategory = x.AssetCategory,
                    MavoItemNumber = x.MavoItemNumber
                }).ToList(),
                Assets = result.Assets.Select(x => new JobAsset()
                {
                    Name = x.Name,
                    Id = x.Id,
                    AssetId = x.AssetId,
                    AssetItemId = x.AssetItemId,
                    QuantityNeeded = x.Quantity - 
                        (x.Kind == AssetKind.Serialized 
                            ? (result.ReturnedAssets.Any(y => x.AssetItemId == y.AssetItemId) ? 1 : 0) 
                            : (result.ReturnedAssets.Any(y => x.AssetId == y.AssetId) ? result.ReturnedAssets.FirstOrDefault(y => y.AssetId == x.AssetId).QuantityReturned : 0)),
                    QuantityTaken = x.Quantity,
                    Kind = x.Kind,
                    Barcode = x.Serial,
                    AssetCategory = x.AssetCategory,
                    MavoItemNumber = x.MavoItemNumber
                }).ToList()
            });
        }
    }
}
