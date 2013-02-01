using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Mavo.Assets.Models.ViewModel
{
    public class SearchResult
    {
        public string SearchString { get; set; }

        public int? Id { get; set; }
        public string JobNumber { get; set; }
        public string Customer { get; set; }
        public int? CustomerId { get; set; }
        public DateTime? ShipDate { get; set; }
        public DateTime? ReturnDate { get; set; }
        public JobStatus? Status { get; set; }
        public string ProjectManager { get; set; }
        public int? ProjectManagerId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }


        [Display(Name="Site Name")]
        public string Name { get; set; }

        public IList<SearchResult> Results { get; set; }

        public bool IsAddon { get; set; }
    }
}