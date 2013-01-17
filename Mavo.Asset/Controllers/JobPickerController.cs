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
        public virtual ActionResult Index(int id, IList<JobAsset> assets)
        {
            JobAddon tempJob = Context.JobAddons.Include("ParentJob").FirstOrDefault(x => x.Id == id);
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
                job = Context.Jobs.FirstOrDefault(x => x.Id == id);
                job.Status = JobStatus.Started;
                job.PickedAssets = new List<PickedAsset>();
                job.PickCompleted = DateTime.Now;
            }
            job.PickedBy = Context.Users.FirstOrDefault(x => x.Email == User.Identity.Name);
            IEnumerable<PickedAsset> pickedAssets = null;
            if (assets != null)
            {
                pickedAssets = assets.Where(x=>(x.Kind == AssetKind.Serialized && !String.IsNullOrEmpty(x.Barcode))
                    || ((x.Kind == AssetKind.NotSerialized || x.Kind == AssetKind.Consumable) && (x.QuantityTaken.HasValue && x.QuantityTaken.Value > 0)))
                    .Select(x => new PickedAsset()
                    {
                        Asset = Context.Assets.FirstOrDefault(a => a.Id == x.AssetId),
                        Item = !String.IsNullOrEmpty(x.Barcode) ? Context.AssetItems.FirstOrDefault(ai => x.Barcode == ai.Barcode) : null,
                        Job = job,
                        Picked = DateTime.Now,
                        Quantity = Math.Max(x.QuantityTaken ?? 1, 1),
                        Barcode = x.Barcode
                    });
                foreach (var pickedAsset in pickedAssets)
                {
                    Context.PickedAssets.Add(pickedAsset);
                    Asset asset = Context.Assets.FirstOrDefault(x => x.Id == pickedAsset.Asset.Id);
                    if (asset.Kind == AssetKind.Consumable || asset.Kind == AssetKind.NotSerialized && asset.Inventory.HasValue)
                        asset.Inventory = Convert.ToInt32(Math.Max((decimal)((asset.Inventory ?? 0) - pickedAsset.Quantity), 0m));
                    else if (asset.Kind == AssetKind.Serialized)
                    {
                        pickedAsset.Item.Status = InventoryStatus.Out;
                    }

                    job.PickedAssets.Add(pickedAsset);
                    AssetActivity.Add(AssetAction.Pick, pickedAsset.Asset, pickedAsset.Item, job);
                }
            }
            Context.SaveChanges();


            return RedirectToAction(MVC.JobPicker.Success(id));
        }
        public virtual ActionResult Success(int id)
        {
            return View();
        }
        public virtual ActionResult Index(int id)
        {
            var result = Context.Jobs.Include("Assets").Include("Asset").Where(x => x.Id == id).Select(x =>
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
                }).First();
            if (result.PickCompleted.HasValue)
                return RedirectToAction(MVC.Reporting.Jobs());

            return View(new PickAJobModel()
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
            });
        }
    }
}
