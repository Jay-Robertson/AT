using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
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

            AssetContext assetContext = ServiceLocator.Current.GetInstance<AssetContext>();

            ViewBag.Templates = assetContext.Templates.Include("Assets").Include("Assets.Asset").ToList();
            ViewBag.JobsReadyToPickForNav = assetContext.Jobs.Where(x => x.Status == JobStatus.ReadyToPick || x.Status == JobStatus.BeingPicked).ToList().GroupBy(x => x.PickupTime.Date).OrderBy(x => x.Key);
            ViewBag.JobsReadyForReturn = assetContext.Jobs.Where(x => !(x is JobAddon) && (x.Status == JobStatus.Started || x.Status == JobStatus.BeingReturned) && x.PickCompleted.HasValue).ToList().GroupBy(x => x.PickCompleted.Value.Date).OrderBy(x => x.Key);

            User currentUser = ServiceLocator.Current.GetInstance<ICurrentUserService>().GetCurrent();
            if (currentUser != null)
            {
                if (currentUser.Disabled)
                {
                    WebSecurity.InitializeDatabaseConnection("DefaultConnection", "Users", "Id", "Email", autoCreateTables: true);
                    WebSecurity.Logout();
                    Response.Redirect("~/");
                }
                ViewBag.CurrentUserRole = currentUser.Role;
                ViewBag.AllUsers = assetContext.Users.ToList();
            }




            base.OnActionExecuting(filterContext);
        }
    }
}
