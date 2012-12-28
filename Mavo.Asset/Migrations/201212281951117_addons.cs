namespace Mavo.Assets.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addons : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.AssetsWithQuantity", name: "Job_Id", newName: "Job_Id2");
            AddColumn("dbo.Jobs", "Discriminator", c => c.String(nullable: false, maxLength: 128));
            AddColumn("dbo.Jobs", "ParentJob_Id", c => c.Int());
            AddForeignKey("dbo.Jobs", "ParentJob_Id", "dbo.Jobs", "Id");
            CreateIndex("dbo.Jobs", "ParentJob_Id");
        }
        
        public override void Down()
        {
            DropIndex("dbo.Jobs", new[] { "ParentJob_Id" });
            DropForeignKey("dbo.Jobs", "ParentJob_Id", "dbo.Jobs");
            DropColumn("dbo.Jobs", "ParentJob_Id");
            DropColumn("dbo.Jobs", "Discriminator");
            RenameColumn(table: "dbo.AssetsWithQuantity", name: "Job_Id2", newName: "Job_Id");
        }
    }
}
