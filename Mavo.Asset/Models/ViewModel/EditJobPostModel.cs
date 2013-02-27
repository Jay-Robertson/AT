using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Mavo.Assets.Models.ViewModel
{
    public class EditJobPostModel
    {
        /// <summary>
        /// Initializes a new instance of the EditJobPostModel class.
        /// </summary>
        public EditJobPostModel()
        {
            Summary = new Summary();
        }
        public int? Id { get; set; }

        [Range(0, 24)]
        public int? ShiftHours { get; set; }

        [Required(ErrorMessage = "Job Site is Required")]
        [Display(Name = "Job Site Name")]
        public string Name { get; set; }

        [Required]
        public string JobNumber { get; set; }


        [Required]
        [Display(Name = "Job Site")]
        public Address Address { get; set; }

        [Display(Name = "Contract/PO#")]
        public string ContractNumber { get; set; }

        public decimal? ContractAmount { get; set; }
        public string Description { get; set; }
        public string ForemanNote { get; set; }





        public Customer Customer { get; set; }

        public int? CustomerId { get; set; }

        [Display(Name = "Foreman")]
        public int? ForemanId { get; set; }
        public int? ProjectManagerId { get; set; }
        public string ProjectManager { get; set; }
        public JobStatus Status { get; set; }
        [Display(Name = "Warehouse Pickup Time")]
        public DateTime PickupTime { get; set; }
        [Display(Name = "Start Date/Time")]
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

        public User SubmittedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public Summary Summary { get; set; }

        public InvoiceDetails InvoiceDetail { get; set; }

        public string SendFinalReportTo { get; set; }

        public string FinalReportPhoneNumber { get; set; }

        public YesNo Notifiable { get; set; }

        [Display(Name = "Contract or T&M")]
        public BillingType BillingType { get; set; }

        public int? SupervisorsNeeded { get; set; }

        public int? WorkersNeeded { get; set; }

        public string ScopeOfWork { get; set; }


        public string JobContact { get; set; }

        public string JobContactPhone { get; set; }

        public DateTime? ShiftStartDate { get; set; }


        public string ConsultantName { get; set; }
        public string ConsultantContact { get; set; }
        public string ConsultantContactNumber { get; set; }
        public string ConsultantEmail { get; set; }

    }



}