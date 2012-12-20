using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Mavo.Assets.Models;
using Mavo.Assets.Models.ViewModel;

namespace Mavo.Assets.Controllers
{
    public partial class JobReturnerController : BaseController
    {
        //
        private readonly AssetContext Context;
        public JobReturnerController(AssetContext context)
        {
            Context = context;
        }
        [HttpPost]
        public virtual JsonResult StartReturning(int id)
        {
            Job job = Context.Jobs.FirstOrDefault(x => x.Id == id);
            job.ReturnStarted = DateTime.Now;
            job.Status = JobStatus.BeingReturned;
            Context.SaveChanges();
            return Json(job.ReturnStarted.ToString());
        }
        [HttpPost]
        public virtual ActionResult Index(int id, IList<JobAsset> assets)
        {
            Job job = Context.Jobs.Include("PickedAssets").Include("PickedAssets.Item").Include("PickedAssets.Asset").FirstOrDefault(x => x.Id == id);
            job.Status = JobStatus.Completed;
            job.ReturnCompleted = DateTime.Now;

            foreach (PickedAsset pickedAsset in job.PickedAssets)
            {
                Asset asset = Context.Assets.FirstOrDefault(x => x.Id == pickedAsset.Asset.Id);
                if (asset.Kind == AssetKind.Consumable || asset.Kind == AssetKind.NotSerialized && asset.Inventory.HasValue)
                    asset.Inventory += assets.FirstOrDefault(x => x.AssetId == asset.Id).QuantityTaken;
                else if (asset.Kind == AssetKind.Serialized)
                {
                    pickedAsset.Item.Status = InventoryStatus.In;
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
                    ReturnStarted = x.ReturnStarted,
                    Assets = x.PickedAssets.Select(a => new
                    {
                        Name = a.Asset.Name,
                        Id = a.Id,
                        Quantity = a.Quantity,
                        Kind = a.Asset.Kind,
                        AssetId = a.Asset.Id,
                        Serial = a.Barcode
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
                    QuantityNeeded = x.Quantity,
                    QuantityTaken = x.Quantity,
                    Kind = x.Kind,
                    Barcode = x.Serial
                }).ToList()
            });
        }
    }
}
