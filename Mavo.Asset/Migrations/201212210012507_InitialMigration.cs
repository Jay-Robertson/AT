namespace Mavo.Assets.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialMigration : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        EmployeeId = c.String(),
                        Email = c.String(),
                        FirstName = c.String(),
                        LastName = c.String(),
                        Role = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.AssetCategory",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Description = c.String(),
                        SortOrder = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.AssetsWithQuantity",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Quantity = c.Int(nullable: false),
                        Asset_Id = c.Int(),
                        Job_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Assets", t => t.Asset_Id)
                .ForeignKey("dbo.Jobs", t => t.Job_Id)
                .Index(t => t.Asset_Id)
                .Index(t => t.Job_Id);
            
            CreateTable(
                "dbo.Assets",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        MavoItemNumber = c.String(),
                        Barcode = c.String(),
                        Name = c.String(),
                        UnitOfMeasure = c.String(),
                        Kind = c.Int(nullable: false),
                        Inventory = c.Int(),
                        Category_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AssetCategory", t => t.Category_Id)
                .Index(t => t.Category_Id);
            
            CreateTable(
                "dbo.AssetItems",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Barcode = c.String(),
                        Condition = c.Int(nullable: false),
                        Manufacturer = c.String(),
                        ModelNumber = c.String(),
                        SerialNumber = c.String(),
                        PurchaseDate = c.DateTime(),
                        PurchasePrice = c.Decimal(precision: 18, scale: 2),
                        WarrantyExpiration = c.DateTime(),
                        Status = c.Int(nullable: false),
                        Asset_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Assets", t => t.Asset_Id)
                .Index(t => t.Asset_Id);
            
            CreateTable(
                "dbo.Jobs",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        JobNumber = c.String(nullable: false),
                        JobSiteName = c.String(nullable: false),
                        Address_Address1 = c.String(nullable: false),
                        Address_Address2 = c.String(),
                        Address_City = c.String(nullable: false),
                        Address_State = c.String(nullable: false),
                        Address_ZipCode = c.String(nullable: false),
                        ContractNumber = c.String(),
                        ContractAmount = c.Decimal(precision: 18, scale: 2),
                        Description = c.String(),
                        ForemanNote = c.String(),
                        PickStarted = c.DateTime(),
                        Status = c.Int(nullable: false),
                        PickupTime = c.DateTime(nullable: false),
                        ContractDate = c.DateTime(nullable: false),
                        EstimatedCompletionDate = c.DateTime(nullable: false),
                        ReturnedDate = c.DateTime(),
                        PickCompleted = c.DateTime(),
                        ReturnStarted = c.DateTime(),
                        ReturnCompleted = c.DateTime(),
                        Customer_Id = c.Int(),
                        ProjectManager_Id = c.Int(),
                        Foreman_Id = c.Int(),
                        PickedBy_Id = c.Int(),
                        ReturnedBy_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Customers", t => t.Customer_Id)
                .ForeignKey("dbo.Users", t => t.ProjectManager_Id)
                .ForeignKey("dbo.Users", t => t.Foreman_Id)
                .ForeignKey("dbo.Users", t => t.PickedBy_Id)
                .ForeignKey("dbo.Users", t => t.ReturnedBy_Id)
                .Index(t => t.Customer_Id)
                .Index(t => t.ProjectManager_Id)
                .Index(t => t.Foreman_Id)
                .Index(t => t.PickedBy_Id)
                .Index(t => t.ReturnedBy_Id);
            
            CreateTable(
                "dbo.Customers",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        CustomerNumber = c.String(),
                        ContactName = c.String(),
                        PhoneNumber = c.String(),
                        Address_Address1 = c.String(nullable: false),
                        Address_Address2 = c.String(),
                        Address_City = c.String(nullable: false),
                        Address_State = c.String(nullable: false),
                        Address_ZipCode = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Template",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.AssetActivity",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Action = c.Int(nullable: false),
                        Date = c.DateTime(nullable: false),
                        Asset_Id = c.Int(),
                        Item_Id = c.Int(),
                        User_Id = c.Int(),
                        Job_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Assets", t => t.Asset_Id)
                .ForeignKey("dbo.AssetItems", t => t.Item_Id)
                .ForeignKey("dbo.Users", t => t.User_Id)
                .ForeignKey("dbo.Jobs", t => t.Job_Id)
                .Index(t => t.Asset_Id)
                .Index(t => t.Item_Id)
                .Index(t => t.User_Id)
                .Index(t => t.Job_Id);
            
            CreateTable(
                "dbo.TemplatesAssets",
                c => new
                    {
                        Id = c.Int(nullable: false),
                        Template_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AssetsWithQuantity", t => t.Id)
                .ForeignKey("dbo.Template", t => t.Template_Id)
                .Index(t => t.Id)
                .Index(t => t.Template_Id);
            
            CreateTable(
                "dbo.PickedAsset",
                c => new
                    {
                        Id = c.Int(nullable: false),
                        Job_Id = c.Int(),
                        Item_Id = c.Int(),
                        Picked = c.DateTime(nullable: false),
                        Barcode = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AssetsWithQuantity", t => t.Id)
                .ForeignKey("dbo.Jobs", t => t.Job_Id)
                .ForeignKey("dbo.AssetItems", t => t.Item_Id)
                .Index(t => t.Id)
                .Index(t => t.Job_Id)
                .Index(t => t.Item_Id);
            
            CreateTable(
                "dbo.ReturnedAsset",
                c => new
                    {
                        Id = c.Int(nullable: false),
                        Job_Id = c.Int(),
                        Item_Id = c.Int(),
                        Returned = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AssetsWithQuantity", t => t.Id)
                .ForeignKey("dbo.Jobs", t => t.Job_Id)
                .ForeignKey("dbo.AssetItems", t => t.Item_Id)
                .Index(t => t.Id)
                .Index(t => t.Job_Id)
                .Index(t => t.Item_Id);
            
        }
        
        public override void Down()
        {
            DropIndex("dbo.ReturnedAsset", new[] { "Item_Id" });
            DropIndex("dbo.ReturnedAsset", new[] { "Job_Id" });
            DropIndex("dbo.ReturnedAsset", new[] { "Id" });
            DropIndex("dbo.PickedAsset", new[] { "Item_Id" });
            DropIndex("dbo.PickedAsset", new[] { "Job_Id" });
            DropIndex("dbo.PickedAsset", new[] { "Id" });
            DropIndex("dbo.TemplatesAssets", new[] { "Template_Id" });
            DropIndex("dbo.TemplatesAssets", new[] { "Id" });
            DropIndex("dbo.AssetActivity", new[] { "Job_Id" });
            DropIndex("dbo.AssetActivity", new[] { "User_Id" });
            DropIndex("dbo.AssetActivity", new[] { "Item_Id" });
            DropIndex("dbo.AssetActivity", new[] { "Asset_Id" });
            DropIndex("dbo.Jobs", new[] { "ReturnedBy_Id" });
            DropIndex("dbo.Jobs", new[] { "PickedBy_Id" });
            DropIndex("dbo.Jobs", new[] { "Foreman_Id" });
            DropIndex("dbo.Jobs", new[] { "ProjectManager_Id" });
            DropIndex("dbo.Jobs", new[] { "Customer_Id" });
            DropIndex("dbo.AssetItems", new[] { "Asset_Id" });
            DropIndex("dbo.Assets", new[] { "Category_Id" });
            DropIndex("dbo.AssetsWithQuantity", new[] { "Job_Id" });
            DropIndex("dbo.AssetsWithQuantity", new[] { "Asset_Id" });
            DropForeignKey("dbo.ReturnedAsset", "Item_Id", "dbo.AssetItems");
            DropForeignKey("dbo.ReturnedAsset", "Job_Id", "dbo.Jobs");
            DropForeignKey("dbo.ReturnedAsset", "Id", "dbo.AssetsWithQuantity");
            DropForeignKey("dbo.PickedAsset", "Item_Id", "dbo.AssetItems");
            DropForeignKey("dbo.PickedAsset", "Job_Id", "dbo.Jobs");
            DropForeignKey("dbo.PickedAsset", "Id", "dbo.AssetsWithQuantity");
            DropForeignKey("dbo.TemplatesAssets", "Template_Id", "dbo.Template");
            DropForeignKey("dbo.TemplatesAssets", "Id", "dbo.AssetsWithQuantity");
            DropForeignKey("dbo.AssetActivity", "Job_Id", "dbo.Jobs");
            DropForeignKey("dbo.AssetActivity", "User_Id", "dbo.Users");
            DropForeignKey("dbo.AssetActivity", "Item_Id", "dbo.AssetItems");
            DropForeignKey("dbo.AssetActivity", "Asset_Id", "dbo.Assets");
            DropForeignKey("dbo.Jobs", "ReturnedBy_Id", "dbo.Users");
            DropForeignKey("dbo.Jobs", "PickedBy_Id", "dbo.Users");
            DropForeignKey("dbo.Jobs", "Foreman_Id", "dbo.Users");
            DropForeignKey("dbo.Jobs", "ProjectManager_Id", "dbo.Users");
            DropForeignKey("dbo.Jobs", "Customer_Id", "dbo.Customers");
            DropForeignKey("dbo.AssetItems", "Asset_Id", "dbo.Assets");
            DropForeignKey("dbo.Assets", "Category_Id", "dbo.AssetCategory");
            DropForeignKey("dbo.AssetsWithQuantity", "Job_Id", "dbo.Jobs");
            DropForeignKey("dbo.AssetsWithQuantity", "Asset_Id", "dbo.Assets");
            DropTable("dbo.ReturnedAsset");
            DropTable("dbo.PickedAsset");
            DropTable("dbo.TemplatesAssets");
            DropTable("dbo.AssetActivity");
            DropTable("dbo.Template");
            DropTable("dbo.Customers");
            DropTable("dbo.Jobs");
            DropTable("dbo.AssetItems");
            DropTable("dbo.Assets");
            DropTable("dbo.AssetsWithQuantity");
            DropTable("dbo.AssetCategory");
            DropTable("dbo.Users");
        }
    }
}
