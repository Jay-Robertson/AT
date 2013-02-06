using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Mavo.Assets.Attributes;
using Mavo.Assets.Models;
using Mavo.Assets.Models.ViewModel;
using WebMatrix.WebData;

namespace Mavo.Assets.Controllers
{
    [AuthorizeUser(UserRole = UserRole.Administrator)]
    public partial class UserManagementController : BaseController
    {
        private AssetContext db;
        public UserManagementController(AssetContext db)
        {
            this.db = db;
        }
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            ViewBag.Users = db.Users.ToList();
            base.OnActionExecuting(filterContext);
        }
        //
        // GET: /UserManagement/

        public virtual ActionResult Index()
        {
            return View(db.Users.ToList());
        }

        //
        // GET: /UserManagement/Details/5

        public virtual ActionResult Details(int id = 0)
        {
            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        //
        // GET: /UserManagement/Create

        public virtual ActionResult Create()
        {
            return View();
        }

        //
        // POST: /UserManagement/Create

        [HttpPost]
        public virtual ActionResult Create(User user)
        {
            if (ModelState.IsValid)
            {
                db.Users.Add(user);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(user);
        }
        public ActionResult ToggleStatus(int id)
        {
            User user = db.Users.Find(id);
            user.Disabled = !user.Disabled;
            db.SaveChanges();
            return RedirectToAction(MVC.UserManagement.Edit(id));
        }
        public ActionResult UpdatePassword(SetPasswordModel model)
        {
            string userId = db.Users.Find(model.Id).Email;
            string token = WebSecurity.GeneratePasswordResetToken(userId);
            WebSecurity.ResetPassword(token, model.Password);
            return RedirectToAction(MVC.UserManagement.Edit(model.Id));
        }
        //
        // GET: /UserManagement/Edit/5

        public virtual ActionResult Edit(int id = 0)
        {
            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        //
        // POST: /UserManagement/Edit/5

        [HttpPost]
        public virtual ActionResult Edit(User user)
        {
            if (ModelState.IsValid)
            {
                if (!String.IsNullOrEmpty(Request.Form["Role"]))
                {
                    var values = Request.Form["Role"].Split(',');
                    user.Role = (UserRole)values.Aggregate(0, (acc, v) => acc |= Convert.ToInt32(v), acc => acc);
                }

                var entry = db.Entry<User>(user);

                if (entry.State == EntityState.Detached)
                {
                    var set = db.Set<User>();
                    User attachedEntity = set.Find(user.Id);  // You need to have access to key

                    if (attachedEntity != null)
                    {
                        var attachedEntry = db.Entry(attachedEntity);
                        attachedEntry.CurrentValues.SetValues(user);
                    }
                    else
                    {
                        entry.State = EntityState.Modified; // This should attach entity
                    }
                }



                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(user);
        }

        //
        // GET: /UserManagement/Delete/5

        public virtual ActionResult Delete(int id = 0)
        {
            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        //
        // POST: /UserManagement/Delete/5

        [HttpPost, ActionName("Delete")]
        public virtual ActionResult DeleteConfirmed(int id)
        {
            User user = db.Users.Find(id);
            db.Users.Remove(user);
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