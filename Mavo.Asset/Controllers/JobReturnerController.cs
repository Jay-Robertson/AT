using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Mavo.Assets.Models;
using Mavo.Assets.Models.ViewModel;
using Mavo.Assets.Services;

namespace Mavo.Assets.Controllers
{
    [Authorize]
    public partial class JobReturnerController : BaseController
    {
        //
        private readonly IAssetActivityManager AssetActivity;
        private readonly AssetContext Context;
        public JobReturnerController(AssetContext context, IAssetActivityManager assetActivity)
        {
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
        public virtual ActionResult Index(int id, IList<JobAsset> assets)
        {
            Job job = Context.Jobs.Include("PickedAssets").Include("PickedAssets.Item").Include("PickedAssets.Asset").FirstOrDefault(x => x.Id == id);
            job.Status = JobStatus.Completed;
            job.ReturnCompleted = DateTime.Now;
            job.ReturnedAssets = new List<ReturnedAsset>();

            foreach (PickedAsset pickedAsset in job.PickedAssets)
            {
                bool hasReturned = false;
                Asset asset = Context.Assets.FirstOrDefault(x => x.Id == pickedAsset.Asset.Id);
                int? quantityUsed = assets.FirstOrDefault(x => x.AssetId == asset.Id).QuantityTaken;
                if (asset.Kind == AssetKind.Consumable || asset.Kind == AssetKind.NotSerialized && asset.Inventory.HasValue)
                {
                    hasReturned = true;
                    asset.Inventory += quantityUsed;
                }
                else if (asset.Kind == AssetKind.Serialized && !String.IsNullOrEmpty(asset.Barcode))
                {
                    hasReturned = true;
                    pickedAsset.Item.Status = InventoryStatus.In;
                    var incomingItem = assets.FirstOrDefault(x => x.AssetItemId == pickedAsset.Item.Id);
                    if (incomingItem.IsDamaged)
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


            return RedirectToAction(MVC.JobReturner.Success(id));
        }
        public virtual ActionResult Success(int id)
        {
            return View();
        }
        public virtual ActionResult Index(int id)
        {
            var result = Context.Jobs.Include("Assets").Include("Asset").Include("Item").Where(x => x.Id == id).Select(x =>
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
                    ReturnStarted = x.ReturnStarted,
                    Assets = x.PickedAssets.Select(a => new
                    {
                        Name = a.Asset.Name,
                        Id = a.Id,
                        Quantity = a.Quantity,
                        Kind = a.Asset.Kind,
                        AssetId = a.Asset.Id,
                        Serial = a.Barcode,
                        AssetItemId = (a.Item != null ? a.Item.Id : default(int))
                    })
                }).First();
            return View(new PickAJobModel()
            {
                JobId = result.JobId,
                JobName = result.JobName,
                JobNumber = result.JobNumber,
                Manager = String.Format("{0} {1}", result.ManagerFirstName, result.ManagerLastName),
                JobSite = result.JobSite,
                Foreman = String.Format("{0} {1}", result.ForemanFirstName, result.ForemanLastName),
                Customer = result.Customer,
                ReturnStarted = result.ReturnStarted,
                Assets = result.Assets.Select(x => new JobAsset()
                {
                    Name = x.Name,
                    Id = x.Id,
                    AssetId = x.AssetId,
                    AssetItemId = x.AssetItemId,
                    QuantityNeeded = x.Quantity,
                    QuantityTaken = x.Quantity,
                    Kind = x.Kind,
                    Barcode = x.Serial
                }).ToList()
            });
        }
    }
}
