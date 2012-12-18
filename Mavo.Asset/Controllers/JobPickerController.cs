using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Mavo.Assets.Models;
using Mavo.Assets.Models.ViewModel;

namespace Mavo.Assets.Controllers
{
    public partial class JobPickerController : BaseController
    {
        private readonly AssetContext Context;
        /// <summary>
        /// Initializes a new instance of the JobPickerController class.
        /// </summary>
        public JobPickerController(AssetContext context)
        {
            Context = context;
        }
        [HttpPost]
        public virtual ActionResult Index(int id, IList<JobAsset> assets)
        {
            Job job = Context.Jobs.FirstOrDefault(x => x.Id == id);
            job.Status = JobStatus.Started;
            job.PickCompleted = DateTime.Now;
            job.PickedAssets = new List<PickedAsset>();

            if (assets != null)
            {
                var pickedAssets = assets.Select(x => new PickedAsset()
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
                        asset.Inventory = Convert.ToInt32(Math.Max((decimal)(asset.Inventory - pickedAsset.Quantity), 0m));
                    else if (asset.Kind == AssetKind.Serialized)
                    {
                        pickedAsset.Item.Status = InventoryStatus.Out;
                    }

                    job.PickedAssets.Add(pickedAsset);
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
                    Assets = x.Assets.Select(a => new
                    {
                        Name = a.Asset.Name,
                        Id = a.Id,
                        Quantity = a.Quantity,
                        Kind = a.Asset.Kind,
                        AssetId = a.Asset.Id,
                        NotEnoughQuantity = a.Quantity > a.Asset.Inventory,
                        QuantityAvailable = a.Asset.Inventory
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
