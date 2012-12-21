using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Mavo.Assets.Models;
using Mavo.Assets.Models.ViewModel;

namespace Mavo.Assets.Controllers
{
    [Authorize]
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
        private List<Job> GetReadyToPick()
        {
            return Context.Jobs.Include("ProjectManager").Include("PickedBy").Include("ReturnedBy").Where(x => x.Status == JobStatus.ReadyToPick).ToList().Where(x => x.PickupTime.Date == DateTime.Today).ToList();
        }
        private List<Job> GetReadyToReturn()
        {
            var tomorrow = DateTime.Today.AddDays(1).AddSeconds(-1);
            return Context.Jobs.Include("ProjectManager").Include("PickedBy").Include("ReturnedBy").Where(x => x.Status == JobStatus.Started && x.EstimatedCompletionDate >= DateTime.Today && x.EstimatedCompletionDate < tomorrow).ToList();
        }
        private List<Job> GetAlreadyPicked()
        {
            return Context.Jobs.Include("ProjectManager").Include("PickedBy").Include("ReturnedBy").Where(x => x.PickCompleted.HasValue && x.PickCompleted >= DateTime.Today).ToList();
        }
        private List<Job> GetAlreadyReturned()
        {
            return Context.Jobs.Include("ProjectManager").Include("PickedBy").Include("ReturnedBy").Where(x => x.ReturnCompleted.HasValue && x.ReturnCompleted.Value >= DateTime.Today).ToList();
        }
        private List<Job> GetBeingPicked()
        {
            return Context.Jobs.Include("ProjectManager").Include("PickedBy").Include("ReturnedBy").Where(x => x.Status == JobStatus.BeingPicked).OrderBy(x => x.PickStarted).ToList();
        }
        private List<Job> GetBeingReturned()
        {
            return Context.Jobs.Include("ProjectManager").Include("PickedBy").Include("ReturnedBy").Where(x => x.Status == JobStatus.BeingReturned).OrderBy(x => x.ReturnStarted).ToList();
        }
        public virtual ActionResult Jobs()
        {
            JobReportViewModel result = new JobReportViewModel()
                {
                    ReadyToPick = GetReadyToPick(),
                    ReadyToReturn = GetReadyToReturn(),
                    AlreadyPicked = GetAlreadyPicked(),
                    AlreadyReturned = GetAlreadyReturned(),
                    BeingPicked = GetBeingPicked(),
                    BeingReturned = GetBeingReturned()
                };
            return View(result);
        }
    }
}
