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
    public partial class AssetController : Controller
    {
        private AssetContext db = new AssetContext();

        /// <summary>
        /// Initializes a new instance of the AssetController class.
        /// </summary>
        public AssetController()
        {
            db.Configuration.LazyLoadingEnabled = true;
        }

        public virtual ActionResult AssetPickerForTemplate(int id)
        {
            ViewBag.IsForJob = false;
            ViewBag.JobId = id;
            ViewBag.Assets = db.TemplateAssets.Include(x=>x.Asset).FirstOrDefault(x => x.Id == id);
            return PartialView("_AssetPicker", db.AssetCategories.ToList());
        }

        public virtual ActionResult AssetPickerForJob(int id)
        {
            ViewBag.IsForJob = true;
            ViewBag.JobId = id;
            ViewBag.Assets = db.Jobs.Include(x=>x.Assets).Include("Assets.Asset").FirstOrDefault(x => x.Id == id).Assets;
            return PartialView("_AssetPicker", db.AssetCategories.ToList());
        }

        public virtual ActionResult AssetPickerDetail(int id)
        {
            List<Asset> assets = db.Assets.Where(x => x.Category.Id == id).ToList();
            return PartialView("_AssetPickerDetail", assets);
        }
        [HttpPost]
        public virtual ActionResult RemoveAsset(int id, int jobId)
        {
            var job = db.Jobs.Include(x => x.Assets).Include("Assets.Asset").FirstOrDefault(x => x.Id == jobId);
            job.Assets.RemoveAt(job.Assets.ToList().FindIndex(x => x.Id == id));
            db.SaveChanges();
            return null;
        }
        [HttpPost]
        public virtual ActionResult AddAsset(int id, int jobId)
        {
            Asset asset = db.Assets.FirstOrDefault(x => x.Id == id);
            var job = db.Jobs.Include(x => x.Assets).Include("Assets.Asset").FirstOrDefault(x => x.Id == jobId);
            if (job.Assets != null && job.Assets.Any(x => x.Asset.Id == id))
                return null;

            if (job.Assets == null)
                job.Assets = new List<AssetWithQuantity>();

            AssetWithQuantity newAssetWithQuantity = new AssetWithQuantity() { Asset = asset };
            job.Assets.Add(newAssetWithQuantity);

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
        public virtual ActionResult Index()
        {

            return View(db.Assets.ToList());
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
            ViewBag.Assets = db.Assets.ToList();
            ViewBag.AssetCategories = db.AssetCategories.OrderBy(x => x.Name).ToList();
            return View("Edit");
        }


        //
        // GET: /Asset/Edit/5

        public virtual ActionResult Edit(int id = 0)
        {
            ViewBag.Assets = db.Assets.ToList();
            ViewBag.AssetCategories = db.AssetCategories.OrderBy(x => x.Name).ToList();
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