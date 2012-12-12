using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Mavo.Assets.Models.ViewModel
{
    public class PickAJobModel
    {
        /// <summary>
        /// Initializes a new instance of the PickAJobModel class.
        /// </summary>
        public PickAJobModel()
        {
            Assets = new List<JobAsset>();
        }
        public string JobName { get; set; }

        public string JobNumber { get; set; }

        public string Manager { get; set; }

        public string Customer { get; set; }

        public string JobSite { get; set; }

        public string Foreman { get; set; }

        public IList<JobAsset> Assets { get; set; }

        public int JobId { get; set; }
    }
    public class JobAsset
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Status { get; set; }

        public int? QuantityTaken { get; set; }

        public int? QuantityNeeded { get; set; }
    }
}