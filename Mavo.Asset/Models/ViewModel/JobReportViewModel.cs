using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Mavo.Assets.Models.ViewModel
{
    public class JobReportViewModel
    {/// <summary>
        /// Initializes a new instance of the JobReportViewModel class.
        /// </summary>
        public JobReportViewModel()
        {
            ReadyToPick = new List<Job>();
            ReadyToReturn = new List<Job>();
            BeingPicked = new List<Job>();
            BeingReturned = new List<Job>();
            AlreadyPicked = new List<Job>();
            AlreadyReturned = new List<Job>();
        }
        public List<Job> ReadyToPick { get; set; }
        public List<Job> ReadyToReturn { get; set; }
        public List<Job> BeingPicked { get; set; }
        public List<Job> BeingReturned { get; set; }
        public List<Job> AlreadyPicked { get; set; }
        public List<Job> AlreadyReturned { get; set; }
    }
}