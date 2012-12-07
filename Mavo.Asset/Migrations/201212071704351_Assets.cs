namespace Mavo.Assets.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Assets : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.AssetItems", "Type_Id", "dbo.Assets");
            DropIndex("dbo.AssetItems", new[] { "Type_Id" });
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
                "dbo.AssetActivity",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Action = c.Int(nullable: false),
                        Date = c.DateTime(nullable: false),
                        Asset_Id = c.Int(),
                        Item_Id = c.Int(),
                        User_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Assets", t => t.Asset_Id)
                .ForeignKey("dbo.AssetItems", t => t.Item_Id)
                .ForeignKey("dbo.Users", t => t.User_Id)
                .Index(t => t.Asset_Id)
                .Index(t => t.Item_Id)
                .Index(t => t.User_Id);
            
            AddColumn("dbo.Assets", "Barcode", c => c.String());
            AddColumn("dbo.Assets", "Manufacturer", c => c.String());
            AddColumn("dbo.Assets", "ModelNumber", c => c.String());
            AddColumn("dbo.Assets", "UPC", c => c.String());
            AddColumn("dbo.Assets", "Category_Id", c => c.Int());
            AddColumn("dbo.AssetItems", "Barcode", c => c.String());
            AddColumn("dbo.AssetItems", "Condition", c => c.Int(nullable: false));
            AddColumn("dbo.AssetItems", "SerialNumber", c => c.String());
            AddColumn("dbo.AssetItems", "PurchaseDate", c => c.DateTime());
            AddColumn("dbo.AssetItems", "PurchasePrice", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("dbo.AssetItems", "WarrantyExpiration", c => c.DateTime());
            AddColumn("dbo.AssetItems", "Asset_Id", c => c.Int());
            AlterColumn("dbo.Assets", "Inventory", c => c.Int());
            AddForeignKey("dbo.Assets", "Category_Id", "dbo.AssetCategory", "Id");
            AddForeignKey("dbo.AssetItems", "Asset_Id", "dbo.Assets", "Id");
            CreateIndex("dbo.Assets", "Category_Id");
            CreateIndex("dbo.AssetItems", "Asset_Id");
            DropColumn("dbo.Assets", "Serial");
            DropColumn("dbo.AssetItems", "Serial");
            DropColumn("dbo.AssetItems", "Type_Id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.AssetItems", "Type_Id", c => c.Int());
            AddColumn("dbo.AssetItems", "Serial", c => c.String());
            AddColumn("dbo.Assets", "Serial", c => c.String());
            DropIndex("dbo.AssetActivity", new[] { "User_Id" });
            DropIndex("dbo.AssetActivity", new[] { "Item_Id" });
            DropIndex("dbo.AssetActivity", new[] { "Asset_Id" });
            DropIndex("dbo.AssetItems", new[] { "Asset_Id" });
            DropIndex("dbo.Assets", new[] { "Category_Id" });
            DropForeignKey("dbo.AssetActivity", "User_Id", "dbo.Users");
            DropForeignKey("dbo.AssetActivity", "Item_Id", "dbo.AssetItems");
            DropForeignKey("dbo.AssetActivity", "Asset_Id", "dbo.Assets");
            DropForeignKey("dbo.AssetItems", "Asset_Id", "dbo.Assets");
            DropForeignKey("dbo.Assets", "Category_Id", "dbo.AssetCategory");
            AlterColumn("dbo.Assets", "Inventory", c => c.Int(nullable: false));
            DropColumn("dbo.AssetItems", "Asset_Id");
            DropColumn("dbo.AssetItems", "WarrantyExpiration");
            DropColumn("dbo.AssetItems", "PurchasePrice");
            DropColumn("dbo.AssetItems", "PurchaseDate");
            DropColumn("dbo.AssetItems", "SerialNumber");
            DropColumn("dbo.AssetItems", "Condition");
            DropColumn("dbo.AssetItems", "Barcode");
            DropColumn("dbo.Assets", "Category_Id");
            DropColumn("dbo.Assets", "UPC");
            DropColumn("dbo.Assets", "ModelNumber");
            DropColumn("dbo.Assets", "Manufacturer");
            DropColumn("dbo.Assets", "Barcode");
            DropTable("dbo.AssetActivity");
            DropTable("dbo.AssetCategory");
            CreateIndex("dbo.AssetItems", "Type_Id");
            AddForeignKey("dbo.AssetItems", "Type_Id", "dbo.Assets", "Id");
        }
    }
}
