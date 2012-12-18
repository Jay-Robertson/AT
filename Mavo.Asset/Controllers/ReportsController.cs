﻿using System;
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
            List<Job> startedJobs = Context.Jobs.Where(x => x.Status == JobStatus.Started).ToList();
            JobReportViewModel result = new JobReportViewModel()
                {
                    ReadyToPick = Context.Jobs.Where(x => x.Status == JobStatus.ReadyToPick).ToList().Where(x => x.PickupTime.Date == DateTime.Today).ToList(),
                    ReadyToReturn = startedJobs.Where(x => x.EstimatedCompletionDate.Date == DateTime.Today).ToList(),
                    PickedToday = Context.Jobs.Where(x => x.PickupTime >= DateTime.Today).ToList(),
                    ReturnedToday = Context.Jobs.Where(x => x.ReturnCompleted.HasValue && x.ReturnCompleted.Value >= DateTime.Today).ToList(),
                };
            return View(result);
        }
    }
}
