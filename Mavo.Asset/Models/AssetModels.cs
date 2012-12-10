using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Globalization;
using System.Web.Security;

namespace Mavo.Assets.Models
{
    public class AssetContext : DbContext
    {
        public AssetContext()
            : base("DefaultConnection")
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<AssetCategory> AssetCategories { get; set; }
        public DbSet<Asset> Assets { get; set; }
        public DbSet<AssetItem> AssetItems { get; set; }
        public DbSet<AssetActivity> AssetActivity { get; set; }
        public DbSet<Template> Templates { get; set; }
        public DbSet<TemplateAsset> TemplateAssets { get; set; }

        public DbSet<Job> Jobs { get; set; }
        public DbSet<Customer> Customers { get; set; }
    }

    public enum AssetKind
    {
        Consumable,     // asset is not barcoded and not expected to return from a job
        Serialized,     // asset is ndividually barcoded
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
        public string Barcode { get; set; }         // mavo barcode value
        public AssetCategory Category { get; set; }
        public string Name { get; set; }

        // warehousing data
        public AssetKind Kind { get; set; }
        public int? Inventory { get; set; }          // only valid for Consumable and NotSerialized assets

        // manufactuer/model/vendor data
        public string Manufacturer { get; set; }
        public string ModelNumber { get; set; }
        public string UPC { get; set; }

        public List<AssetItem> Items { get; set; }
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
        public string SerialNumber { get; set; }            // manufacturer's serial number
        public DateTime? PurchaseDate { get; set; }
        public decimal? PurchasePrice { get; set; }
        public DateTime? WarrantyExpiration { get; set; }
    }

    public enum AssetAction
    {
        Create,
        Edit,
        Pick,
        Transfer,
        Return,
        Repair,
        Retire
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
    }

    [Table("Template")]
    public class Template
    {
        [Key, DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Name { get; set; }
        public IList<TemplateAsset> Assets { get; set; }
    }

    [Table("TemplatesAssets")]
    public class TemplateAsset
    {
        [Key, DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public Template Template { get; set; }
        public Asset Asset { get; set; }
        public int Quantity { get; set; }
    }

    public enum JobStatus
    {
        New,
        Started,
        Completed,
        ReadyToPick
    }

    [Table("Jobs")]
    public class Job
    {
        [Key, DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

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

        public Customer Customer { get; set; }
        public User ProjectManager { get; set; }
        public JobStatus Status { get; set; }
        public DateTime PickupTime { get; set; }
        public DateTime ContractDate { get; set; }
        public DateTime EstimatedCompletionDate { get; set; }

        public User Foreman { get; set; }
    }

    [ComplexType]
    public class Address
    {
        [Required]
        public string Address1 { get; set; }

        public string Address2 { get; set; }
        [Required]
        public string City { get; set; }
        [Required]
        public string State { get; set; }
        [Required]
        public string ZipCode { get; set; }

    }

    [Table("Customers")]
    public class Customer
    {
        [Key, DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }
    }
}

