using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.Web.Mvc;
using Mavo.Assets.Models;

namespace Mavo.Assets.Controllers
{
    [Authorize]
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
            return View("Edit");
        }

        //
        // GET: /Template/Edit/5
        public virtual ActionResult Edit(int id)
        {
            Template template = ctx.Templates.Include(x=>x.Assets).FirstOrDefault(x=>x.Id == id);
            ViewBag.Assets = template.Assets;
            return View(template);
        }

        [HttpPost]
        public virtual ActionResult Clone(int id)
        {
            var clone = new Template();
            var source = ctx.Templates.Include("Assets").Include("Assets.Asset").First(x => x.Id == id);
            clone.Name = source.Name + " [Clone]";
            clone.Master = false;
            clone.Assets = source.Assets.Select(x => new TemplateAsset() { Asset = x.Asset, Quantity = x.Quantity, Template = clone }).ToList();
            ctx.Templates.Add(clone);
            ctx.SaveChanges();

            return RedirectToAction(MVC.Template.Edit(clone.Id));
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
                var currentUserRole = (UserRole)ViewBag.CurrentUserRole;
                if (toSave.Master && !(currentUserRole == UserRole.Administrator))
                {
                    throw new HttpException(403, "Only administrators may modify master templates.");
                }
                toSave.Name = template.Name;
                toSave.Master = template.Master;
            }

            ctx.SaveChanges();

            return RedirectToAction(MVC.Template.Edit(template.Id));
        }
    }
}
