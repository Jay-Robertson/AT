using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Mavo.Assets.Models.ViewModel
{
    public class EditJobPostModel
    {
        public int? Id { get; set; }

        [Required(ErrorMessage = "Job Name is Required")]
        public string Name { get; set; }

        [Required]
        public string JobNumber { get; set; }
        [Required]
        public string JobSiteName { get; set; }

        [Required]
        public Address Address { get; set; }

        public string ContractNumber { get; set; }
        public decimal? ContractAmount { get; set; }
        public string Description { get; set; }
        public string ForemanNote { get; set; }

        public int? CustomerId { get; set; }
        public int? ForemanId { get; set; }
        public int? ProjectManagerId { get; set; }
        public JobStatus Status { get; set; }
        public DateTime PickupTime { get; set; }
        public DateTime ContractDate { get; set; }
        public DateTime EstimatedCompletionDate { get; set; }

        public int? TemplateId { get; set; }

        public IList<Template> Templates { get; set; }

        public bool IsAddon { get; set; }

        public string PickedUpByStr { get; set; }

        public DateTime? PickStarted { get; set; }

        public DateTime? ReturnStarted { get; set; }
        public string ReturnedByStr { get; set; }

        public IList<PickedAsset> PickedAssets { get; set; }
    }
}