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
using System.Text;
using Mavo.Assets.Services;
using System.Web.UI;

namespace Mavo.Assets.Controllers
{
    [Authorize]
    [OutputCache(Location = OutputCacheLocation.None, NoStore = true)]
    public partial class AssetController : BaseController
    {
        private readonly IAssetActivityManager AssetActivity;
        private readonly IAssetPicker AssetPicker;
        private AssetContext db;

        /// <summary>
        /// Initializes a new instance of the AssetController class.
        /// </summary>
        public AssetController(AssetContext db, IAssetPicker assetPicker, IAssetActivityManager activityManager)
        {
            AssetActivity = activityManager;
            AssetPicker = assetPicker;
            this.db = db;
        }
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            ViewBag.AssetCategories = new AssetContext().AssetCategories.OrderBy(x => x.Name).ToList();
            base.OnActionExecuting(filterContext);
        }
        [HttpPost]
        public virtual ActionResult RetireItem(int id)
        {
            var item = db.AssetItems.Include(x => x.Asset).FirstOrDefault(x => x.Id == id);
            item.Condition = AssetCondition.Retired;

            AssetActivity.Add(AssetAction.Retire, item.Asset, item);

            db.SaveChanges();
            return RedirectToAction(MVC.Asset.ItemReview());
        }
        [HttpPost]
        public virtual ActionResult RepairItem(int id)
        {
            var item = db.AssetItems.Include(x => x.Asset).FirstOrDefault(x => x.Id == id);
            item.Condition = AssetCondition.Good;

            AssetActivity.Add(AssetAction.Repair, item.Asset, item);

            db.SaveChanges();
            return RedirectToAction(MVC.Asset.ItemReview());
        }
        public virtual ActionResult ItemReview()
        {
            return View(db.AssetItems.Include("Asset").Where(x => x.Condition == AssetCondition.Damaged).ToList());
        }

        public virtual ActionResult AssetPickerForTemplate(int? id = null)
        {
            if (!id.HasValue)
                return null;
            ViewBag.IsForJob = false;
            ViewBag.TemplateId = id;
            ViewBag.Assets = db.TemplateAssets.Where(x => x.Template.Id == id.Value).ToList();
            ViewBag.Lock = false;
            return PartialView("_AssetPicker", db.AssetCategories.ToList());
        }

        [HttpGet]
        public virtual JsonResult IsAssetItemAvailable()
        {
            string barcode = Request.QueryString[Request.QueryString.AllKeys.First(p => p.ToLower().Contains("barcode"))];
        
            bool serialExists = db.AssetItems.Any(x => x.Barcode == barcode);
            bool isInStock = db.AssetItems.Any(x => x.Barcode == barcode && x.Status == InventoryStatus.In);
            bool isInGoodCondition = db.AssetItems.Any(x => x.Barcode == barcode && x.Condition == AssetCondition.Good);
            IList<string> errors = new List<string>();
            if (!serialExists)
                errors.Add("Serial number is not inventoried");
            else if (!isInStock)
                errors.Add("Item is out on a job");
            else if (!isInGoodCondition)
                errors.Add("Item is retired/damaged");
            if (errors.Count == 0)
                return Json(true, JsonRequestBehavior.AllowGet);
            else
                return Json(string.Join(", ", errors), JsonRequestBehavior.AllowGet);
        }
        public virtual ActionResult AssetPickerForJob(int id)
        {
            AssetContext outOfRequestDb = new AssetContext();
            Job job = outOfRequestDb.Jobs.Include("Assets").Include("Assets.Asset").FirstOrDefault(x => x.Id == id);
            ViewBag.IsForJob = true;
            ViewBag.JobId = id;
            ViewBag.Assets = job.Status == JobStatus.New ? job.Assets : job.PickedAssets == null ? new List<AssetWithQuantity>() : job.PickedAssets.Select(x => (AssetWithQuantity)x).ToList();
            ViewBag.Lock = job.Status != JobStatus.New;
            return PartialView("_AssetPicker", outOfRequestDb.AssetCategories.ToList());
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

            return PartialView("_AssetRow", AssetPicker.Add(id, jobId, templateId));
        }

        [HttpPost]
        public virtual ActionResult UpdateQuantity(int id, int quantity)
        {
            AssetPicker.IncreaseQuantity(id, quantity);
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
        public virtual ActionResult Index(int? categoryId = null, AssetKind? kind = null)
        {
            if (categoryId.HasValue || kind.HasValue)
                return Index(new AssetSearchResult() { CategoryId = categoryId, Kind = kind });
            return View();
        }
        [HttpPost]
        public virtual ActionResult Index(AssetSearchResult search)
        {
            var query = db.Assets.AsQueryable();

            if (search.Kind.HasValue)
                query = query.Where(x => x.Kind == search.Kind);

            if (search.CategoryId.HasValue)
                query = query.Where(x => x.Category.Id == search.CategoryId);
            if (!String.IsNullOrEmpty(search.SearchString))
                query = query.Where(x => x.Barcode.Contains(search.SearchString)
                    || x.Name.Contains(search.SearchString)
                    || x.Category.Name.Contains(search.SearchString)
                    );

            search.Results = query.Select(x => new AssetSearchResult()
            {
                Name = x.Name,
                Category = x.Category.Name,
                Kind = x.Kind,
                CategoryId = x.Category.Id,
                AssetId = x.Id,
                Quantity = x.Kind == AssetKind.Serialized ? x.Items.Count() : x.Inventory,
                AssetItems = x.Items
            }).ToList();
            return View(search);
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
                Asset asset = db.Assets.FirstOrDefault(x => x.Id == scan.AssetId);
                AssetItem assetItem = new AssetItem()
                {
                    Asset = asset,
                    Barcode = scan.Barcode,
                    Condition = scan.Condition,
                    PurchaseDate = scan.PurchaseDate,
                    WarrantyExpiration = scan.WarrantyExpiration,
                    SerialNumber = scan.SerialNumber,
                    PurchasePrice = scan.PurchasePrice,
                    ModelNumber = scan.ModelNumber,
                    Manufacturer = scan.Manufacturer,
                };

                db.AssetItems.Add(assetItem);
                db.SaveChanges();

                AssetActivity.Add(AssetAction.Scanned, asset, assetItem);

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
                {
                    context.Assets.Add(assetToSave);
                    AssetActivity.Add(AssetAction.Create, assetToSave);
                }
                else
                    AssetActivity.Add(AssetAction.Edit, assetToSave);

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