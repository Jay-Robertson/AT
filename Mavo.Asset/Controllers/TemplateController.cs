using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Mavo.Assets.Models;

namespace Mavo.Assets.Controllers
{
    public class TemplateController : Controller
    {
        AssetContext ctx;
        //
        // GET: /Template/
        /// <summary>
        /// Initializes a new instance of the TemplateController class.
        /// </summary>
        public TemplateController()
        {
            ctx = new AssetContext();
        }
        public ActionResult Index()
        {
            return View(ctx.Templates.ToList());
        }

        //
        // GET: /Template/Create

        public ActionResult Create()
        {
            ViewBag.Templates = ctx.Templates.ToList();
            return View("Edit");
        }

        //
        // POST: /Template/Create

        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /Template/Edit/5

        public ActionResult Edit(int id)
        {
            ViewBag.Templates = ctx.Templates.ToList();
            return View(ctx.Templates.FirstOrDefault(x=>x.Id == id));
        }

        //
        // POST: /Template/Edit/5

        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /Template/Delete/5

        public ActionResult Delete(int id)
        {
            return View();
        }

        //
        // POST: /Template/Delete/5

        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
