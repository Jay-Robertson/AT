using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Mavo.Assets.Models;
using Mavo.Assets.Models.ViewModel;

namespace Mavo.Assets.Controllers
{
    public partial class ReportsController : BaseController
    {
        private readonly AssetContext Context;
        public ReportsController(AssetContext context)
        {
            Context = context;
        }
        public virtual ActionResult LateJobs()
        {
            return View(Context.Jobs.Where(x => x.EstimatedCompletionDate < DateTime.Today && x.Status == JobStatus.Started).ToList());
        }
        public virtual ActionResult Jobs()
        {
            var tomorrow = DateTime.Today.AddDays(1).AddSeconds(-1);
            JobReportViewModel result = new JobReportViewModel()
                {
                    ReadyToPick = Context.Jobs.Include("ProjectManager").Where(x => x.Status == JobStatus.ReadyToPick).ToList().Where(x => x.PickupTime.Date == DateTime.Today).ToList(),
                    ReadyToReturn = Context.Jobs.Where(x => x.Status == JobStatus.Started && x.EstimatedCompletionDate >= DateTime.Today && x.EstimatedCompletionDate < tomorrow).ToList(),
                    AlreadyPicked = Context.Jobs.Where(x => x.PickCompleted.HasValue && x.PickCompleted >= DateTime.Today).ToList(),
                    AlreadyReturned = Context.Jobs.Where(x => x.ReturnCompleted.HasValue && x.ReturnCompleted.Value >= DateTime.Today).ToList(),
                    BeingPicked = Context.Jobs.Where(x => x.Status == JobStatus.BeingPicked).OrderBy(x => x.PickStarted).ToList(),
                    BeingReturned = Context.Jobs.Where(x=>x.Status == JobStatus.BeingReturned).OrderBy(x=>x.ReturnStarted).ToList()
                };
            return View(result);
        }
    }
}
