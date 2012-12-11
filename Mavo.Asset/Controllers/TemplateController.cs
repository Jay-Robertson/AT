using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Mavo.Assets.Models;

namespace Mavo.Assets.Controllers
{
    public partial class TemplateController : BaseController
    {
        AssetContext ctx;
        //
        // GET: /Template/
        /// <summary>
        /// Initializes a new instance of the TemplateController class.
        /// </summary>
        public TemplateController(AssetContext ctx)
        {
            this.ctx = ctx;
        }

        public virtual ActionResult Index()
        {
            return View(ctx.Templates.ToList());
        }

        //
        // GET: /Template/Create
        public virtual ActionResult Create()
        {
            ViewBag.Templates = ctx.Templates.ToList();
            return View("Edit");
        }

        //
        // GET: /Template/Edit/5
        public virtual ActionResult Edit(int id)
        {
            ViewBag.Templates = ctx.Templates.ToList();
            return View(ctx.Templates.FirstOrDefault(x=>x.Id == id));
        }

        //
        // POST: /Template/Edit/5
        [HttpPost]
        public virtual ActionResult Edit(int id, FormCollection collection)
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
    }
}
