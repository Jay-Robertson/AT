using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Mavo.Assets.Models;
using Mavo.Assets.Models.EmailViewModel;
using Mavo.Assets.Models.ViewModel;
using Mavo.Assets.Services;
using Postal;
using System.Net;

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
        public JsonResult UpdateQuantity(int id, int assetId, int quantity)
        {
            var job = __GetJobForReturningActions(id);
            var returned = job.ReturnedAssets.First(x => x.Asset.Id == assetId);
            returned.QuantityPicked = quantity;
            Context.SaveChanges();
            return Json("Ok");
        }

        private Job __GetJobForReturningActions(int jobId)
        {
            // find the job by id
            var job = Context.Jobs
                .Include("ReturnedAssets").Include("ReturnedAssets.Item").Include("ReturnedAssets.Asset")
                .Include("PickedAssets").Include("PickedAssets.Item").Include("PickedAssets.Asset")
                .Include("ProjectManager")
                .FirstOrDefault(x => x.Id == jobId);

            // mark it as being returned if needed
            if (job.Status == JobStatus.Started)
            {
                job.Status = JobStatus.BeingReturned;
                job.ReturnStarted = DateTime.Now;
                Context.SaveChanges();
            }

            return job;
        }

        [HttpPost]
        public ActionResult ReturnSerialized(int jobId, string barcode, bool isDamaged = false)
        {
            // find job and asset item
            var job = __GetJobForReturningActions(jobId);
            var assetItem = Context.AssetItems.FirstOrDefault(x => x.Barcode == barcode.Trim());

            // validate barcode
            if (null == assetItem)
            {
                Response.StatusCode = (int)HttpStatusCode.NotFound;
                return Json(String.Format("{0} does not exist in inventory.", barcode));
            }

            // validate item was picked
            if (!job.PickedAssets.Any(x => x.Item == assetItem))
            {
                Response.StatusCode = (int)HttpStatusCode.MethodNotAllowed;
                return Json(String.Format("{1} {0} is not part of the job.", barcode, assetItem.Asset.Name));
            }

            // validate item isnt already returned
            if (job.ReturnedAssets.Any(x => x.Item == assetItem))
            {
                Response.StatusCode = (int)HttpStatusCode.MethodNotAllowed;
                return Json(String.Format("{1} {0} has already been returned.", barcode, assetItem.Asset.Name));
            }

            // item is back in inventory
            assetItem.Status = InventoryStatus.In;
            if (isDamaged)
            {
                assetItem.Condition = AssetCondition.Damaged;
            }

            // record the returned asset
            job.ReturnedAssets.Add(new ReturnedAsset
            {
                Asset = assetItem.Asset,
                Item = assetItem,
                Quantity = 1,
                QuantityPicked = 1,
                Job = job,
                Returned = DateTime.Now
            });
            AssetActivity.Add(AssetAction.Return, assetItem.Asset, assetItem, job);

            Context.SaveChanges();

            return PartialView(
                MVC.JobPicker.Views._PickedAssetRow,
                new PickedAssetRow()
                {
                    AssetId = assetItem.Asset.Id,
                    AssetItemId = assetItem.Id,
                    AssetName = assetItem.Asset.Name,
                    Barcodes = assetItem.Barcode,
                    CurrentPickedQty = 1,
                    MavoNumber = assetItem.Asset.MavoItemNumber,
                    Damaged = (assetItem.Condition == AssetCondition.Damaged)
                }
            );
        }

        [HttpPost]
        public ActionResult ReturnNonSerialized(int jobId, int assetId, int quantity = 1)
        {
            var job = __GetJobForReturningActions(jobId);
            var asset = Context.Assets.FirstOrDefault(x => x.Id == assetId);
            
            // validate asset exists
            if (null == asset)
            {
                Response.StatusCode = (int)HttpStatusCode.NotFound;
                return Json(String.Format("No asset found with Id of {0}", assetId));
            }

            // validate asset is consumable/non-serialized
            if (asset.Kind == AssetKind.Serialized)
            {
                Response.StatusCode = (int)HttpStatusCode.MethodNotAllowed;
                return Json(String.Format("{0} is a serialized asset.", asset.Name));
            }

            // validate asset was picked
            var pickedAsset = job.PickedAssets.FirstOrDefault(x => x.Asset == asset);
            if (null == pickedAsset)
            {
                Response.StatusCode = (int)HttpStatusCode.MethodNotAllowed;
                return Json(String.Format("{0} is not part of the job.", asset.Name));
            }

            // create returned record (if needed)
            var returnedAsset = job.ReturnedAssets.FirstOrDefault(x => x.Asset == asset);
            if (null == returnedAsset)
            {
                returnedAsset = new ReturnedAsset {
                    Asset = asset,
                    Item = null,
                    Quantity = 0,
                    QuantityPicked = 0,
                    Job = job,
                    Returned = DateTime.Now
                };
                job.ReturnedAssets.Add(returnedAsset);
            }

            // increment returned quantity
            returnedAsset.Quantity += quantity;
            returnedAsset.QuantityPicked += quantity;
            
            AssetActivity.Add(AssetAction.Return, pickedAsset.Asset, pickedAsset.Item, job);
            Context.SaveChanges();

            return PartialView(
                MVC.JobPicker.Views._PickedAssetRow,
                new PickedAssetRow()
                {
                    AssetId = assetId,
                    AssetName = pickedAsset.Asset.Name,
                    CurrentPickedQty = returnedAsset.QuantityPicked,
                    MavoNumber = asset.MavoItemNumber
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
            Job job = Context.Jobs.Include("ProjectManager").FirstOrDefault(x => x.Id == jobId);
            job.ReturnedBy = CurrentUser.GetCurrent();
            job.ReturnCompleted = DateTime.Now;
            job.Status = JobStatus.Completed;
            Context.SaveChanges();

            if (job.ProjectManager != null)
            {
                dynamic email = new Email("JobComplete");
                email.Subject = String.Format("Job #{0} is complete!", job.JobNumber);
                email.To = job.ProjectManager.Email;
                email.Job = job;
                email.Send();
            }

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
                    PickupTime = x.PickupTime,
                    ReturnedAssets = x.ReturnedAssets.Select(a => new
                    {
                        Name = a.Asset.Name,
                        Id = a.Id,
                        Quantity = a.Quantity,
                        Kind = a.Asset.Kind,
                        AssetId = a.Asset.Id,
                        Barcode = a.Item.Barcode,
                        AssetCategory = a.Asset.Category.Name,
                        AssetItemId = (a.Item != null ? a.Item.Id : (int?)null),
                        MavoItemNumber = a.Asset.MavoItemNumber,
                        QuantityReturned = a.QuantityPicked,
                        IsDamaged = ((bool?)(a.Item.Condition == AssetCondition.Damaged)) ?? false
                    }),
                    Assets = x.PickedAssets.Select(a => new
                    {
                        Name = a.Asset.Name,
                        Id = a.Id,
                        Quantity = a.Quantity,
                        Kind = a.Asset.Kind,
                        AssetId = a.Asset.Id,
                        Barcode = a.Item.Barcode,
                        AssetCategory = a.Asset.Category.Name,
                        AssetItemId = (a.Item != null ? a.Item.Id : (int?)null),
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
                PickupTime = result.PickupTime,
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
                    Barcode = x.Barcode,
                    AssetCategory = x.AssetCategory,
                    MavoItemNumber = x.MavoItemNumber,
                    IsDamaged = x.IsDamaged

                }).OrderByDescending(x => Asset.SortableMavoItemNumber(x.MavoItemNumber)).ToList(),
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
                    Barcode = x.Barcode,
                    AssetCategory = x.AssetCategory,
                    MavoItemNumber = x.MavoItemNumber
                }).OrderByDescending(x => Asset.SortableMavoItemNumber(x.MavoItemNumber)).ToList()
            });
        }
    }
}
