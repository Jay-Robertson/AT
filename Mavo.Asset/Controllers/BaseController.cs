using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Mavo.Assets.Models;
using Microsoft.Practices.ServiceLocation;

namespace Mavo.Assets.Controllers
{
    public partial class BaseController : Controller
    {
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            AssetContext assetContext =  new AssetContext();

            ViewBag.Templates = assetContext.Templates.ToList();
            ViewBag.JobsReadyToPickForNav = assetContext.Jobs.Where(x => x.Status == JobStatus.ReadyToPick || x.Status == JobStatus.BeingPicked).ToList().GroupBy(x => x.PickupTime.Date).OrderBy(x => x.Key);
            ViewBag.JobsReadyForReturn = assetContext.Jobs.Where(x => !(x is JobAddon) &&  (x.Status == JobStatus.Started || x.Status == JobStatus.BeingReturned) && x.PickCompleted.HasValue).ToList().GroupBy(x => x.PickCompleted.Value.Date).OrderBy(x => x.Key);

            base.OnActionExecuting(filterContext);
        }
    }
}
