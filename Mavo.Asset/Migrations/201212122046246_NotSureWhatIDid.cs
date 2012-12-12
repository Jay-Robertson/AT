namespace Mavo.Assets.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class NotSureWhatIDid : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.PickedAsset",
                c => new
                    {
                        Id = c.Int(nullable: false),
                        Job_Id = c.Int(),
                        Item_Id = c.Int(),
                        Picked = c.DateTime(nullable: false),
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
            DropForeignKey("dbo.ReturnedAsset", "Item_Id", "dbo.AssetItems");
            DropForeignKey("dbo.ReturnedAsset", "Job_Id", "dbo.Jobs");
            DropForeignKey("dbo.ReturnedAsset", "Id", "dbo.AssetsWithQuantity");
            DropForeignKey("dbo.PickedAsset", "Item_Id", "dbo.AssetItems");
            DropForeignKey("dbo.PickedAsset", "Job_Id", "dbo.Jobs");
            DropForeignKey("dbo.PickedAsset", "Id", "dbo.AssetsWithQuantity");
            DropTable("dbo.ReturnedAsset");
            DropTable("dbo.PickedAsset");
        }
    }
}
