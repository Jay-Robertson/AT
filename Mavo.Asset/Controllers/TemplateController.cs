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
            ViewBag.Templates = ctx.Templates.Include("Assets").Include("Assets.Asset").ToList();
            Template template = ctx.Templates.FirstOrDefault(x=>x.Id == id);
            ViewBag.Assets = template.Assets;
            return View(template);
        }

        //
        // POST: /Template/Edit/5
        [HttpPost]
        public virtual ActionResult Edit(int id, Template template)
        {
            if (template == null)
                return View();

            if (id == 0)
                ctx.Templates.Add(template);
            else
            {
                var toSave = ctx.Templates.FirstOrDefault(x => x.Id == id);
                toSave.Name = template.Name;
            }

            ctx.SaveChanges();

            return RedirectToAction(MVC.Template.Edit(template.Id));
        }
    }
}
