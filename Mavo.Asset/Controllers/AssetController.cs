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

        //
        // GET: /Asset/

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
            Asset asset = db.Assets.Find(id);
            if (asset == null)
            {
                return HttpNotFound();
            }
            return View(asset);
        }

        //
        // POST: /Asset/Edit/5

        public ActionResult Scan()
        {
            ViewBag.Assets = db.Assets.ToList();
            return View();
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