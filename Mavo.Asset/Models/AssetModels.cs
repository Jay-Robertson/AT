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
        public AssetContext() : base("DefaultConnection")
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Asset> Assets { get; set; }
        public DbSet<AssetItem> AssetItems { get; set; }
        public DbSet<Template> Templates { get; set; }
        public DbSet<TemplateAsset> TemplateAssets { get; set; }

        public DbSet<Job> Jobs { get; set; }
        public DbSet<Customer> Customers { get; set; }
    }

    public enum AssetKind
    {
        Consumable,     // asset is not barcoded and not expected to return from a job
        Serialized,     // asset is ndividually barcoded an
        NotSerialized,  // asset is durable and expected to return from job, but not individually tracked
    }

    [Table("Assets")]
    public class Asset
    {
        [Key, DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Serial { get; set; }
        public string Name { get; set; }
        public AssetKind Kind { get; set; }
        public int Inventory { get; set; }
    }

    [Table("AssetItems")]
    public class AssetItem
    {
        [Key, DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Serial { get; set; }
        public Asset Type { get; set; }
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

    [Table("Jobs")]
    public class Job
    {
        [Key, DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required(ErrorMessage="Job Name is Required")]
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


        public enum JobStatus
        {
            New,
            Started,
            Completed
        }

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

