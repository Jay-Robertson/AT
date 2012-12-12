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

            AssetContext assetContext =  ServiceLocator.Current.GetInstance<AssetContext>();
            ViewBag.Templates = assetContext.Templates.ToList();
            ViewBag.JobsReadyToPickForNav = assetContext.Jobs.Where(x=>x.Status == JobStatus.ReadyToPick).ToList().GroupBy(x=>x.PickupTime.Date).OrderBy(x=>x.Key);
            base.OnActionExecuting(filterContext);
        }
    }
}
