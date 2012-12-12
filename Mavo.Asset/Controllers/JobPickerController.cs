using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Mavo.Assets.Models;
using Mavo.Assets.Models.ViewModel;

namespace Mavo.Assets.Controllers
{
    public partial class JobPickerController : BaseController
    {
        private readonly AssetContext Context;
        /// <summary>
        /// Initializes a new instance of the JobPickerController class.
        /// </summary>
        public JobPickerController(AssetContext context)
        {
            Context = context;
        }
        [HttpPost]
        public virtual ActionResult Index(int id, IList<JobAsset> assets)
        {
            Job job = Context.Jobs.FirstOrDefault(x => x.Id == id);
            job.Status = JobStatus.Started;

            var pickedAssets = assets.Select(x => new PickedAsset()
            {
                Asset = Context.Assets.FirstOrDefault(a => a.Id == x.Id),
                Job = job,
                Picked = DateTime.Now,
                Quantity = x.QuantityTaken.Value
            });
            foreach (var item in pickedAssets)
                Context.PickedAssets.Add(item);

            Context.SaveChanges();

            return View();
        }
        public virtual ActionResult Index(int id)
        {
            var result = Context.Jobs.Where(x=>x.Id == id).Select(x =>
                new
                {
                    JobId = x.Id,
                    JobName = x.Name,
                    JobNumber = x.JobNumber,
                    ManagerFirstName = x.ProjectManager.FirstName,
                    ManagerLastName = x.ProjectManager.LastName,
                    Customer = x.Customer.Name,
                    JobSite = x.JobSiteName,
                    ForemanFirstName = x.Foreman.FirstName,
                    ForemanLastName = x.Foreman.LastName,
                    Assets = x.Assets.Select(a => new
                    {
                        Name = a.Asset.Name,
                        Id = a.Id,
                        Quantity = a.Quantity,
                        Kind = a.Asset.Kind,
                    })
                }).First();
            return View(new PickAJobModel()
            {
                JobId = result.JobId,
                JobName = result.JobName,
                JobNumber = result.JobNumber,
                Manager = String.Format("{0} {1}", result.ManagerFirstName, result.ManagerLastName),
                JobSite = result.JobSite,
                Foreman = String.Format("{0} {1}", result.ForemanFirstName, result.ForemanLastName),
                Customer = result.Customer,
                Assets = result.Assets.Select(x => new JobAsset()
                {
                    Name = x.Name,
                    Id = x.Id,
                    QuantityNeeded = x.Quantity,
                    QuantityTaken = x.Quantity,
                    Kind = x.Kind
                }).ToList()
            });
        }
    }
}
