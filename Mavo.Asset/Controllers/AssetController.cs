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
using Mavo.Assets.Attributes;

namespace Mavo.Assets.Controllers
{
    [Authorize]
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
            ViewBag.AssetCategories = db.AssetCategories.OrderBy(x => x.Name).ToList();
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
            ViewBag.Assets = db.TemplateAssets.Include(x => x.Asset).Include(x => x.Asset.Items).Where(x => x.Template.Id == id.Value).ToList();
            ViewBag.Lock = false;
            return PartialView("_AssetPicker", db.AssetCategories.OrderBy(x => x.Name).ToList());
        }

        [HttpGet]
        public virtual JsonResult IsAssetItemAvailable()
        {
            string barcode = Request.QueryString[Request.QueryString.AllKeys.First(p => p.ToLower().Contains("barcode"))];
            int assetId = int.Parse(Request.QueryString[Request.QueryString.AllKeys.First(p => p.ToLower().Contains("assetid"))]);

            bool serialExists = db.AssetItems.Any(x => x.Barcode == barcode);
            bool isInStock = db.AssetItems.Any(x => x.Barcode == barcode && x.Status == InventoryStatus.In);
            bool isInGoodCondition = db.AssetItems.Any(x => x.Barcode == barcode && x.Condition == AssetCondition.Good);
            bool existsWithinAsset = db.AssetItems.Any(x => x.Barcode == barcode && x.Asset.Id == assetId);
            IList<string> errors = new List<string>();
            if (!serialExists)
                errors.Add("Serial number is not inventoried");
            else if (!isInStock)
                errors.Add("Item is out on a job");
            else if (!isInGoodCondition)
                errors.Add("Item is retired/damaged");
            else if (!existsWithinAsset)
                errors.Add(String.Format("This is not a {0}.", db.Assets.First(x => x.Id == assetId).Name));

            if (errors.Count == 0)
                return Json(true, JsonRequestBehavior.AllowGet);
            else
                return Json(string.Join(", ", errors), JsonRequestBehavior.AllowGet);
        }

        public ActionResult UnlockedAssetPickerForJob(int id)
        {
            return this.AssetPickerForJob(id, true);
        }

        public virtual ActionResult AssetPickerForJob(int id, bool? unlock = false)
        {
            AssetContext outOfRequestDb = new AssetContext();
            Job job = outOfRequestDb.Jobs.Include("Assets").Include("Assets.Asset").FirstOrDefault(x => x.Id == id);
            ViewBag.IsForJob = true;
            ViewBag.JobId = id;
            ViewBag.Assets = job.Assets.OrderBy(x => Asset.SortableMavoItemNumber(x.Asset.MavoItemNumber));
            ViewBag.Lock = (job.Status != JobStatus.New && job.Status != JobStatus.ReadyToPick);
            if (unlock.HasValue && unlock.Value == true)
            {
                ViewBag.Lock = false;
            }
            return PartialView("_AssetPicker", outOfRequestDb.AssetCategories.OrderBy(x => x.Name).ToList());
        }

        public virtual ActionResult GetAssetDetail(string id, IList<int> availableAssets, int? jobId = null)
        {
            AssetItem assetItem = db.AssetItems.Include(x => x.Asset).Where(x => x.Barcode == id).FirstOrDefault();
            if (assetItem == null)
                return Json(new { success = false, reason = String.Format("{0} does not exist in inventory", id) });
            if (!availableAssets.Contains(assetItem.Asset.Id))
                return Json(new { success = false, reason = String.Format("You scanned a {0} item. This has not been marked to be picked.", assetItem.Asset.Name) });
            if (assetItem.Condition != AssetCondition.Good)
                return Json(new { success = false, reason = String.Format("{0} is {1}", assetItem.Asset.Name, assetItem.Condition) });

            if (!jobId.HasValue && assetItem.Status == InventoryStatus.Out)
                return Json(new { success = false, reason = String.Format("How could you be scanning this? It's out on a job.", assetItem.Asset.Name, assetItem.Condition) });

            return Json(new { success = true, assetId = assetItem.Asset.Id, assetItemId = assetItem.Id, barcode = id }, JsonRequestBehavior.AllowGet);
        }

        public virtual ActionResult GetAssetRow(int id, int jobId, int index)
        {
            var assetItem = db.Jobs.Where(x => x.Id == jobId).SelectMany(x => x.Assets).Where(x => x.Asset.Id == id).Select(a => new JobAsset()
                            {
                                Name = a.Asset.Name,
                                Id = a.Id,
                                QuantityNeeded = a.Quantity - a.QuantityPicked,
                                Kind = a.Asset.Kind,
                                AssetId = a.Asset.Id,
                                NotEnoughQuantity = a.Quantity > (a.Asset.Inventory ?? 0),
                                QuantityAvailable = a.Asset.Inventory,
                                AssetCategory = a.Asset.Category.Name,
                                MavoItemNumber = a.Asset.MavoItemNumber
                            }).First();
            ViewData["index"] = index;
            return PartialView(MVC.JobPicker.Views._TabletAssetRow, assetItem);
        }

        public virtual ActionResult AssetPickerDetail(int id)
        {
            List<Asset> assets = db.Assets.Where(x => x.Category.Id == id).OrderBy(x => x.Name).ToList();
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

        [HttpGet]
        public ActionResult Search(string q)
        {
            return Json(
                db.Assets
                    .Where(x =>
                        x.Name.Contains(q) ||
                        x.Barcode.Contains(q) ||
                        x.MavoItemNumber.Contains(q) ||
                        x.Category.Name.Contains(q))
                    .Take(16)
                    .Select(x => new {
                        id = x.Id,
                        number = x.MavoItemNumber,
                        name = "[" + x.MavoItemNumber + "] "+ x.Name + (String.IsNullOrEmpty(x.Barcode) ? "" : " (" + x.Barcode + ")")
                    })
                    .ToList()
                    .OrderBy(x => Convert.ToInt32(x.number)),
                JsonRequestBehavior.AllowGet
            );
        }


        [HttpPost]
        public virtual ActionResult Index(AssetSearchResult search)
        {
            // do an initial search for a specific barcode
            if (!String.IsNullOrWhiteSpace(search.SearchString))
            {
                var i = db.AssetItems.FirstOrDefault(x => x.Barcode == search.SearchString.Trim());
                if (null != i)
                {
                    return RedirectToAction(MVC.Asset.Scan(i.Id));
                }
            }

            var query = db.Assets.AsQueryable();
            if (search.Kind.HasValue)
            {
                query = query.Where(x => x.Kind == search.Kind);
            }
            if (search.CategoryId.HasValue)
            {
                query = query.Where(x => x.Category.Id == search.CategoryId);
            }
            if (!String.IsNullOrEmpty(search.SearchString))
            {
                query = query.Where(x =>
                    x.Barcode.Contains(search.SearchString) ||
                    x.Items.Any(i =>
                        i.Barcode.Contains(search.SearchString)) ||
                        x.Name.Contains(search.SearchString) ||
                        x.Category.Name.Contains(search.SearchString) ||
                        x.MavoItemNumber.Contains(search.SearchString
                    )
                );
            }
            search.Results = query.Select(x => new AssetSearchResult()
            {
                MavoItemNumber = x.MavoItemNumber,
                Name = x.Name,
                Category = x.Category.Name,
                Kind = x.Kind,
                CategoryId = x.Category.Id,
                AssetId = x.Id,
                Quantity = x.Kind == AssetKind.Serialized ? x.Items.Count() : x.Inventory,
                AssetItems = x.Items
            }).ToList();

            if (search.Results.Count == 1)
            {
                return RedirectToAction("Edit", new { id = search.Results[0].AssetId });
            }
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

        public virtual ActionResult Scan(int? id = null)
        {
            List<AssetCategory> categories = db.AssetCategories.OrderBy(x => x.Name).ToList();
            ViewBag.AssetCategories = categories;
            ViewBag.Assets = db.Assets.ToList();
            if (id.HasValue)
            {
                // editing existing item
                var assetItem = db.AssetItems.First(x => x.Id == id.Value);
                AssetScanPostModel model = new AssetScanPostModel
                {
                    AssetId = assetItem.Asset.Id,
                    Barcode = assetItem.Barcode,
                    Condition = assetItem.Condition,
                    Status = assetItem.Status,
                    Id = assetItem.Id,
                    Manufacturer = assetItem.Manufacturer,
                    ModelNumber = assetItem.ModelNumber,
                    PurchaseDate = assetItem.PurchaseDate,
                    PurchasePrice = assetItem.PurchasePrice,
                    SerialNumber = assetItem.SerialNumber,
                    WarrantyExpiration = assetItem.WarrantyExpiration,
                    AssetCategoryId = assetItem.Asset.Category.Id
                };
                ViewBag.AssetsForDropDown = db.Assets.Where(x => x.Category.Id == model.AssetCategoryId).ToList();
                return View(model);
            }

            // scanning new item
            var currentCategoryId = categories.First().Id;
            ViewBag.AssetsForDropDown = db.Assets.Where(x => x.Category.Id == currentCategoryId).ToList();
            ViewBag.CurrentCategoryId = currentCategoryId;
            return View();
        }

        [AuthorizeUser(UserRole = UserRole.Administrator)]
        public virtual ActionResult FoundItem(int id)
        {
            var assetItem = db.AssetItems.First(x => x.Id == id);
            assetItem.Status = InventoryStatus.In;
            db.SaveChanges();
            return RedirectToAction("Scan", new { id = id });
        }

        [HttpPost]
        public virtual ActionResult ScanItem(AssetScanPostModel scan)
        {
            if (ModelState.IsValid)
            {
                Asset asset = db.Assets.FirstOrDefault(x => x.Id == scan.AssetId);
                AssetItem assetItem = null;
                if (scan.Id.HasValue)
                {
                    assetItem = db.AssetItems.Single(x => x.Id == scan.Id);
                    assetItem.Asset = asset;
                    assetItem.Barcode = scan.Barcode;
                    assetItem.Condition = scan.Condition;
                    assetItem.PurchaseDate = scan.PurchaseDate;
                    assetItem.WarrantyExpiration = scan.WarrantyExpiration;
                    assetItem.SerialNumber = scan.SerialNumber;
                    assetItem.PurchasePrice = scan.PurchasePrice;
                    assetItem.ModelNumber = scan.ModelNumber;
                    assetItem.Manufacturer = scan.Manufacturer;
                    db.SaveChanges();
                    AssetActivity.Add(AssetAction.Edit, asset, assetItem);
                    return RedirectToAction(MVC.Asset.Edit(asset.Id));
                }
                
                assetItem = new AssetItem()
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