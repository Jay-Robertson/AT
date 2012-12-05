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
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Assets",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Serial = c.String(),
                        Name = c.String(),
                        Kind = c.Int(nullable: false),
                        Inventory = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.AssetItems",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Serial = c.String(),
                        Type_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Assets", t => t.Type_Id)
                .Index(t => t.Type_Id);
            
            CreateTable(
                "dbo.Template",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.TemplatesAssets",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Quantity = c.Int(nullable: false),
                        Template_Id = c.Int(),
                        Asset_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Template", t => t.Template_Id)
                .ForeignKey("dbo.Assets", t => t.Asset_Id)
                .Index(t => t.Template_Id)
                .Index(t => t.Asset_Id);
            
        }
        
        public override void Down()
        {
            DropIndex("dbo.TemplatesAssets", new[] { "Asset_Id" });
            DropIndex("dbo.TemplatesAssets", new[] { "Template_Id" });
            DropIndex("dbo.AssetItems", new[] { "Type_Id" });
            DropForeignKey("dbo.TemplatesAssets", "Asset_Id", "dbo.Assets");
            DropForeignKey("dbo.TemplatesAssets", "Template_Id", "dbo.Template");
            DropForeignKey("dbo.AssetItems", "Type_Id", "dbo.Assets");
            DropTable("dbo.TemplatesAssets");
            DropTable("dbo.Template");
            DropTable("dbo.AssetItems");
            DropTable("dbo.Assets");
            DropTable("dbo.Users");
        }
    }
}
