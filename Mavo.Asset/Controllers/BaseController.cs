using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web;
using Mavo.Assets.Filters;
using Mavo.Assets.Models;
using Mavo.Assets.Services;
using Microsoft.Practices.ServiceLocation;
using WebMatrix.WebData;

namespace Mavo.Assets.Controllers
{
    [ValidateInput(false)]
    [InitializeSimpleMembership]
    public partial class BaseController : Controller
    {
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {

            AssetContext assetContext = new AssetContext();

            ViewBag.Templates = assetContext.Templates.Include("Assets").Include("Assets.Asset").OrderBy(x => x.Name).ToList();
            ViewBag.JobsReadyToPickForNav = assetContext.Jobs
                .Where(x => x.Status == JobStatus.ReadyToPick || x.Status == JobStatus.BeingPicked).ToList()
                .GroupBy(x => x.EstimatedCompletionDate.Date)
                .OrderBy(x => x.Key);

            ViewBag.JobsReadyForReturn = assetContext.Jobs.Where(x => !(x is JobAddon) && (x.Status == JobStatus.Started || x.Status == JobStatus.BeingReturned) && x.PickCompleted.HasValue).ToList().GroupBy(x => x.PickCompleted.Value.Date).OrderBy(x => x.Key);

            User currentUser = assetContext.Users.FirstOrDefault(x => x.Email == HttpContext.User.Identity.Name);
            if (currentUser != null)
            {
                if (currentUser.Disabled)
                {
                    WebSecurity.InitializeDatabaseConnection("DefaultConnection", "Users", "Id", "Email", autoCreateTables: true);
                    WebSecurity.Logout();
                    Response.Redirect("~/");
                }
                ViewBag.CurrentUserRole = currentUser.Role;
                ViewBag.AllUsers = assetContext.Users.OrderBy(x => x.LastName).ThenBy(x=>x.FirstName).ToList();
            }




            base.OnActionExecuting(filterContext);
        }
    }
}
