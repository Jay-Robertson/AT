using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Mavo.Assets.Models;
using Mavo.Assets.Models.ViewModel;
using AutoMapper;

namespace Mavo.Assets.Controllers
{
    public partial class AssetController : BaseController
    {
        private AssetContext db;

        /// <summary>
        /// Initializes a new instance of the AssetController class.
        /// </summary>
        public AssetController(AssetContext db)
        {
            this.db = db;
        }
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            ViewBag.AssetCategories = db.AssetCategories.OrderBy(x => x.Name).ToList();
            base.OnActionExecuting(filterContext);
        }
        public virtual ActionResult AssetPickerForTemplate(int? id = null)
        {
            if (!id.HasValue)
                return null;
            ViewBag.IsForJob = false;
            ViewBag.TemplateId = id;
            return PartialView("_AssetPicker", db.AssetCategories.ToList());
        }

        public virtual ActionResult AssetPickerForJob(int id)
        {
            ViewBag.IsForJob = true;
            ViewBag.JobId = id;
            return PartialView("_AssetPicker", db.AssetCategories.ToList());
        }

        public virtual ActionResult AssetPickerDetail(int id)
        {
            List<Asset> assets = db.Assets.Where(x => x.Category.Id == id).ToList();
            return PartialView("_AssetPickerDetail", assets);
        }
        [HttpPost]
        public virtual ActionResult RemoveAsset(int id, int? jobId = null, int? templateId = null)
        {
            if (jobId.HasValue)
            {
                var job = db.Jobs.Include(x => x.Assets).Include("Assets.Asset").FirstOrDefault(x => x.Id == jobId);
                job.Assets.RemoveAt(job.Assets.ToList().FindIndex(x => x.Id == id));
            }
            else if (templateId.HasValue)
            {
                var template = db.Templates.Include(x => x.Assets).Include("Assets.Asset").FirstOrDefault(x => x.Id == templateId);
                template.Assets.RemoveAt(template.Assets.ToList().FindIndex(x => x.Id == id));
            }
            db.SaveChanges();
            return null;
        }
        [HttpPost]
        public virtual ActionResult AddAsset(int id, int? jobId = null, int? templateId = null)
        {
       
            Asset asset = db.Assets.FirstOrDefault(x => x.Id == id);
            AssetWithQuantity newAssetWithQuantity = new AssetWithQuantity() { Asset = asset, Quantity = 1 };
            if (jobId.HasValue)
            {
              
                var job = db.Jobs.Include(x => x.Assets).Include("Assets.Asset").FirstOrDefault(x => x.Id == jobId);
                if (job.Assets != null && job.Assets.Any(x => x.Asset.Id == id))
                {
                    var assetToIncrease = job.Assets.FirstOrDefault(x => x.Asset.Id == id);
                    assetToIncrease.Quantity++;
                    newAssetWithQuantity.Quantity = assetToIncrease.Quantity;
                }
                else
                {
                    if (job.Assets == null)
                        job.Assets = new List<AssetWithQuantity>();

                    job.Assets.Add(newAssetWithQuantity);
                }
            }
            else if (templateId.HasValue)
            {
                newAssetWithQuantity = new TemplateAsset() { Asset = asset, Quantity = 1 }; ;

                var template = db.Templates.Include(x => x.Assets).Include("Assets.Asset").FirstOrDefault(x => x.Id == templateId);
                if (template.Assets != null && template.Assets.Any(x => x.Asset.Id == id))
                {
                    var assetToIncrease = template.Assets.FirstOrDefault(x => x.Asset.Id == id);
                    assetToIncrease.Quantity++;
                    newAssetWithQuantity.Quantity = assetToIncrease.Quantity;
                }
                else
                {
                    if (template.Assets == null)
                        template.Assets = new List<TemplateAsset>();

                    template.Assets.Add((TemplateAsset)newAssetWithQuantity);
                }
            }


            db.SaveChanges();

            return PartialView("_AssetRow", newAssetWithQuantity);
        }

        [HttpPost]
        public virtual ActionResult UpdateQuantity(int id, int quantity)
        {
            var asset = db.JobAssets.FirstOrDefault(x => x.Id == id);
            asset.Quantity = quantity;
            db.SaveChanges();
            return null;
        }

        [HttpPost]
        public virtual ActionResult UpdateQuantityForTemplate(int id, int quantity)
        {
            var asset = db.TemplateAssets.FirstOrDefault(x => x.Id == id);
            asset.Quantity = quantity;
            db.SaveChanges();
            return null;
        }
        public virtual ActionResult Index()
        {

            return View();
        }

        //
        // GET: /Asset/Details/5

        public virtual ActionResult Details(int id = 0)
        {
            Asset asset = db.Assets.Find(id);
            if (asset == null)
            {
                return HttpNotFound();
            }
            return View(asset);
        }

        //
        // GET: /Asset/Create

        public virtual ActionResult Create()
        {
            return View("Edit");
        }


        //
        // GET: /Asset/Edit/5

        public virtual ActionResult Edit(int id = 0)
        {
            ViewBag.AssetItems = db.AssetItems.Where(x => x.Asset.Id == id).ToList();
            Asset asset = db.Assets.Find(id);
            if (asset == null)
            {
                return HttpNotFound();
            }
            return View(asset);
        }

        //
        // POST: /Asset/Edit/5

        public virtual ActionResult Scan()
        {
            ViewBag.Assets = db.Assets.ToList();
            return View();
        }

        [HttpPost]
        public virtual ActionResult ScanItem(AssetScanPostModel scan)
        {
            if (ModelState.IsValid)
            {
                AssetItem assetItem = new AssetItem()
                {
                    Asset = db.Assets.FirstOrDefault(x => x.Id == scan.AssetId),
                    Barcode = scan.Barcode,
                    Condition = scan.Condition,
                    PurchaseDate = scan.PurchaseDate,
                    WarrantyExpiration = scan.WarrantyExpiration,
                    SerialNumber = scan.SerialNumber,
                    PurchasePrice = scan.PurchasePrice
                };
                db.AssetItems.Add(assetItem);
                db.SaveChanges();
            }
            return RedirectToAction("Scan");
        }

        [HttpPost]
        public virtual ActionResult Edit(AssetPostModel asset)
        {
            if (ModelState.IsValid)
            {
                AssetContext context = new AssetContext();
                var assetToSave = context.Assets.FirstOrDefault(x => x.Id == asset.Id);
                assetToSave = Mapper.Map<AssetPostModel, Asset>(asset, assetToSave);
                if (asset.CategoryId.HasValue)
                    assetToSave.Category = context.AssetCategories.FirstOrDefault(x => x.Id == asset.CategoryId.Value);

                if (!asset.Id.HasValue)
                    context.Assets.Add(assetToSave);

                context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(asset);
        }


        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}