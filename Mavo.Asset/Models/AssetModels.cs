using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Globalization;
using System.Web.Security;
using Mavo.Assets.Models.ViewModel;

namespace Mavo.Assets.Models
{
    public class AssetContext : DbContext
    {
        public AssetContext()
            : base("DefaultConnection")
        {
        }

        public DbSet<JobAddon> JobAddons { get; set; }
        public DbSet<ReturnedAsset> ReturnedAssets { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<AssetCategory> AssetCategories { get; set; }

        public DbSet<AssetWithQuantity> JobAssets { get; set; }

        public DbSet<Asset> Assets { get; set; }
        public DbSet<AssetItem> AssetItems { get; set; }
        public DbSet<AssetActivity> AssetActivity { get; set; }
        public DbSet<Template> Templates { get; set; }
        public DbSet<TemplateAsset> TemplateAssets { get; set; }

        public DbSet<PickedAsset> PickedAssets { get; set; }
        public DbSet<Job> Jobs { get; set; }
        public DbSet<Customer> Customers { get; set; }

        public AssetItem Lookup(string barcode)
        {
            if (String.IsNullOrWhiteSpace(barcode))
            {
                return null;
            }
            barcode = barcode.Trim();
            return this.AssetItems.Include(x => x.Asset).SingleOrDefault(x => x.Barcode == barcode);
        }
    }

    public enum InventoryStatus
    {
        In,
        Out
    }

    public enum AssetKind
    {
        Consumable,     // asset is not barcoded and not expected to return from a job
        Serialized,     // asset is individually barcoded
        NotSerialized,  // asset is durable and expected to return from job, but not individually tracked
    }

    public enum AssetCondition
    {
        Good,           // asset is functional and in circulation
        Damaged,        // asset is damaged and awaiting repairs, out of circulation for now
        Retired         // asset could not be repaired and has been removed from circulation
    }

    [Table("AssetCategory")]
    public class AssetCategory
    {
        [Key, DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }
        public int SortOrder { get; set; }
    }

    [Table("Assets")]
    public class Asset
    {
        [Key, DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string MavoItemNumber { get; set; }
        public string Barcode { get; set; }          // mavo barcode value
        public AssetCategory Category { get; set; }
        [Display(Name="Item Name")]
        public string Name { get; set; }
        public string UnitOfMeasure { get; set; }    // valid on Consumable and NonSerialized assets

        // warehousing data
        public AssetKind Kind { get; set; }
        public int? Inventory { get; set; }          // valid on Consumable and NotSerialized assets

        public List<AssetItem> Items { get; set; }

        public static int? SortableMavoItemNumber(string mavoItemNumber)
        {
            int i;
            if (int.TryParse(mavoItemNumber, out i)) return i;
            return null;
        }
    }

    [Table("AssetItems")]
    public class AssetItem
    {
        [Key, DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public Asset Asset { get; set; }
        public string Barcode { get; set; }                 // mavo barcode value
        public AssetCondition Condition { get; set; }

        // purchasing/warranty data
        public string Manufacturer { get; set; }
        public string ModelNumber { get; set; }
        public string SerialNumber { get; set; }            // manufacturer's serial number
        public DateTime? PurchaseDate { get; set; }
        public decimal? PurchasePrice { get; set; }
        public DateTime? WarrantyExpiration { get; set; }

        public InventoryStatus Status { get; set; }

        public int? AssetId { get { return Asset == null ? null : (int?)Asset.Id; } }
        public string AssetName { get { return Asset == null ? null : Asset.Name; } }
        public AssetKind? AssetKind { get { return Asset == null ? null : (AssetKind?)Asset.Kind; } }
        public int? AssetInventory { get { return Asset == null ? null : Asset.Inventory; } }
        public string AssetMavoItemNumber { get { return Asset == null ? null : Asset.MavoItemNumber; } }
        public AssetCategory AssetCategory { get { return Asset == null ? null : Asset.Category; } }
        public string AssetCategoryName { get { return this.AssetCategory == null ? null : AssetCategory.Name; } }
    }

    public enum AssetAction
    {
        Create,
        Edit,
        Pick,
        Return,
        Repair,
        Retire,
        Scanned,
        Damaged,
        TransferTo,
        TransferFrom
    }

    [Table("AssetActivity")]
    public class AssetActivity
    {
        [Key, DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public Asset Asset { get; set; }
        public AssetItem Item { get; set; }
        public AssetAction Action { get; set; }
        public DateTime Date { get; set; }
        public User User { get; set; }
        public Job Job { get; set; }                // associated job for Pick and Return actions
    }

    [Table("Template")]
    public class Template
    {
        [Key, DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public IList<TemplateAsset> Assets { get; set; }
    }

    [Table("AssetsWithQuantity")]
    public class AssetWithQuantity
    {
        [Key, DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public Asset Asset { get; set; }
        public int Quantity { get; set; }

        public int QuantityPicked { get; set; }

        public int? AssetId { get { return Asset == null ? null : (int?)Asset.Id; } }
        public string AssetName { get { return Asset == null ? null : Asset.Name;  } }
        public AssetKind? AssetKind { get { return Asset == null ? null : (AssetKind?)Asset.Kind; } }
        public int? AssetInventory { get { return Asset == null ? null : Asset.Inventory; } }
        public string AssetMavoItemNumber { get { return Asset == null ? null : Asset.MavoItemNumber; } }
        public AssetCategory AssetCategory { get { return Asset == null ? null : Asset.Category; } }
        public string AssetCategoryName { get { return this.AssetCategory == null ? null : AssetCategory.Name; } }
        public string AssetUnitOfMeasure { get { return Asset == null ? null : (Asset.UnitOfMeasure ?? "EACH"); } }

        public int SortableAssetMavoItemNumber
        {
            get
            {
                int n;
                if (int.TryParse(this.AssetMavoItemNumber, out n)) return n;
                return 0;
            }
        }
    }

    [Table("PickedAsset")]
    public class PickedAsset : AssetWithQuantity
    {
        public Job Job { get; set; }
        public DateTime Picked { get; set; }
        public AssetItem Item { get; set; }

        public string Barcode { get; set; }
    }

    [Table("ReturnedAsset")]
    public class ReturnedAsset : AssetWithQuantity
    {
        public Job Job { get; set; }
        public DateTime Returned { get; set; }
        public AssetItem Item { get; set; }

        public string Barcode { get; set; }

    }

    [Table("TemplatesAssets")]
    public class TemplateAsset : AssetWithQuantity
    {
        public Template Template { get; set; }
    }

    public enum JobStatus
    {
        New,
        ReadyToPick,
        BeingPicked,
        Started,
        BeingReturned,
        Completed
    }
    public class JobAddon : Job
    {
        public Job ParentJob { get; set; }
        public bool? IsPicked { get; set; }
    }
    [Table("Jobs")]
    public class Job
    {
        public Job()
        {
            FinalReportAddress = new Address();
        }
        [Key, DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required(ErrorMessage = "Site Name is Required")]
        [Display(Name="Job Site Name")]
        public string Name { get; set; }

        [Required]
        public string JobNumber { get; set; }

        [Required]
        public Address Address { get; set; }

        public string ContractNumber { get; set; }
        public decimal? EstimatedCost { get; set; }
        public decimal? EstimatedContractAmount { get; set; }
        public decimal? ContractAmount { get; set; }
        public string Description { get; set; }
        public string ForemanNote { get; set; }

        public Customer Customer { get; set; }
        public DateTime? PickStarted { get; set; }
        public User ProjectManager { get; set; }
        public JobStatus Status { get; set; }
        public DateTime PickupTime { get; set; }
        public DateTime ContractDate { get; set; }
        public DateTime EstimatedCompletionDate { get; set; }

        public User Foreman { get; set; }

        public IList<AssetWithQuantity> Assets { get; set; }

        public DateTime? ReturnedDate { get; set; }

        public User PickedBy { get; set; }
        public DateTime? PickCompleted { get; set; }
        public User ReturnedBy { get; set; }
        public DateTime? ReturnStarted { get; set; }
        public DateTime? ReturnCompleted { get; set; }

        public List<PickedAsset> PickedAssets { get; set; }

        public List<ReturnedAsset> ReturnedAssets { get; set; }

        public User SubmittedBy { get; set; }
        public DateTime CreatedDate { get; set; }

        public Summary Summary { get; set; }

        public string SendFinalReportTo { get; set; }

        public Address FinalReportAddress { get; set; }

        public string FinalReportPhoneNumber { get; set; }

        public YesNo Notifiable { get; set; }

        public BillingType BillingType { get; set; }

        public int? SupervisorsNeeded { get; set; }

        public int? WorkersNeeded { get; set; }

        public DateTime? ShiftStartDate { get; set; }


        public string ScopeOfWork { get; set; }

        public string JobContact { get; set; }

        public string JobContactPhone { get; set; }

        public InvoiceDetails InvoiceDetail { get; set; }


        public string ConsultantName { get; set; }
        public string ConsultantContact { get; set; }
        public string ConsultantContactNumber { get; set; }
        public string ConsultantEmail { get; set; }

        public string Labels { get; set; }
        public string Drums { get; set; }
        public string Bags { get; set; }

        public int GetQuantityPicked(Asset asset)
        {
            return this.PickedAssets.Where(x => x.Asset.Id == asset.Id).Sum(x => x.Quantity);
        }

        public List<NotPickedItem> GetItemsNotPicked()
        {
            var result = new List<NotPickedItem>();
            foreach (var requestedAsset in this.Assets)
            {
                var picked = 0;
                var requested = requestedAsset.Quantity;
                if (requestedAsset.Asset.Kind == AssetKind.Serialized)
                {
                    picked = this.PickedAssets.Count(x => x.Asset.Id == requestedAsset.Asset.Id);
                }
                else
                {
                    var pickedAsset = this.PickedAssets.FirstOrDefault(x => x.Asset.Id == requestedAsset.Asset.Id);
                    if (pickedAsset != null)
                    {
                        picked = pickedAsset.Quantity;
                    }
                }
                if (requested > picked)
                {
                    result.Add(new NotPickedItem
                    {
                        Asset = requestedAsset.Asset,
                        AssetId = requestedAsset.Asset.Id,
                        AssetName = requestedAsset.Asset.Name,
                        AssetNumber = requestedAsset.Asset.MavoItemNumber,
                        Picked = picked,
                        Requested = requested
                    });
                }
            }
            return result;
        }
    }

    [ComplexType]
    public class InvoiceDetails
    {
        public InvoiceDetails()
        {
            this.CopyAddress = new Address();
            this.InvoiceAddress = new Address();
        }
        public string Attention { get; set; }

        [Display(Name="Consultant Address")]
        public Address Consultant { get; set; }
        public Address InvoiceAddress { get; set; }

        public string InvoiceInstructions { get; set; }

        public string SendInvoiceApprovalTo { get; set; }

        public string IssueInvoiceToOwner { get; set; }

        public string Comments { get; set; }

        [Display(Name="Special Forms to Use")]
        public SpecialForms SpecialForms { get; set; }

        public Address CopyAddress { get; set; }

        public string CopyAttention { get; set; }

        public string CopyTo { get; set; }

        public decimal? NetInvoiceAmount { get; set; }

        [Display(Name="Retainage %")]
        [Range(0,100)]
        public int? Retainage { get; set; }

        public decimal? WithholdPercentage { get; set; }

        public decimal? GrossInvoiceAmount { get; set; }

        public decimal? TotalAmountDue { get; set; }

        public int? ShiftHours { get; set; }

        public SendConsultant SendConsultant { get; set; }

        public SendCustomer SendCustomer { get; set; }
    }
    [Flags]
    public enum SpecialForms
    {
        AIADocuments = 0x1,
        AsPerAttached = 0x2,
        SubmitCertifiedPayrollWithBilling = 0x4,
        StateForms = 0x8,
        Other = 0x10,
        ManifestOrFinalReportNeeded = 0x20
    }

    [ComplexType]
    public class Summary
    {
        public string FilledOutBy { get; set; }

        public int? ShiftHours { get; set; }
    
        public string Comments { get; set; }

    }
    [ComplexType]
    public class Address
    {
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }

    }

    [Table("Customers")]
    public class Customer
    {
        [Key, DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Name { get; set; }
        public string CustomerNumber { get; set; }
        public string ContactName { get; set; }
        public string PhoneNumber { get; set; }
        [Display(Name = "Customer Address")]
        public Address Address { get; set; }
    }
}
    
