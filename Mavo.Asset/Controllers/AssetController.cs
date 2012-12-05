using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Mavo.Assets.Models;

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
            return View();
        }

        //
        // POST: /Asset/Create

        [HttpPost]
        public virtual ActionResult Create(Asset asset)
        {
            if (ModelState.IsValid)
            {
                db.Assets.Add(asset);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(asset);
        }

        //
        // GET: /Asset/Edit/5

        public virtual ActionResult Edit(int id = 0)
        {
            Asset asset = db.Assets.Find(id);
            if (asset == null)
            {
                return HttpNotFound();
            }
            return View(asset);
        }

        //
        // POST: /Asset/Edit/5

        [HttpPost]
        public virtual ActionResult Edit(Asset asset)
        {
            if (ModelState.IsValid)
            {
                db.Entry(asset).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(asset);
        }

        //
        // GET: /Asset/Delete/5

        public virtual ActionResult Delete(int id = 0)
        {
            Asset asset = db.Assets.Find(id);
            if (asset == null)
            {
                return HttpNotFound();
            }
            return View(asset);
        }

        //
        // POST: /Asset/Delete/5

        [HttpPost, ActionName("Delete")]
        public virtual ActionResult DeleteConfirmed(int id)
        {
            Asset asset = db.Assets.Find(id);
            db.Assets.Remove(asset);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}