using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Mavo.Assets.Models.ViewModel
{
    public class LeftNavViewModel
    {
        public Job Job { get; set; }

        public IEnumerable<IGrouping<JobStatus, Models.Job>> Jobs { get; set; }
    }
}