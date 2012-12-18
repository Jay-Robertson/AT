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
            PickedToday = new List<Job>();
            ReturnedToday = new List<Job>();
        }
        public List<Job> ReadyToPick { get; set; }
        public List<Job> ReadyToReturn { get; set; }
        public List<Job> BeingPicked { get; set; }
        public List<Job> BeingReturned { get; set; }
        public List<Job> PickedToday { get; set; }
        public List<Job> ReturnedToday { get; set; }
    }
}