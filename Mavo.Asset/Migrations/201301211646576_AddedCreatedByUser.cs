namespace Mavo.Assets.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedCreatedByUser : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Jobs", "CreatedDate", c => c.DateTime(nullable: false));
            AddColumn("dbo.Jobs", "SubmittedBy_Id", c => c.Int());
            AddForeignKey("dbo.Jobs", "SubmittedBy_Id", "dbo.Users", "Id");
            CreateIndex("dbo.Jobs", "SubmittedBy_Id");
        }
        
        public override void Down()
        {
            DropIndex("dbo.Jobs", new[] { "SubmittedBy_Id" });
            DropForeignKey("dbo.Jobs", "SubmittedBy_Id", "dbo.Users");
            DropColumn("dbo.Jobs", "SubmittedBy_Id");
            DropColumn("dbo.Jobs", "CreatedDate");
        }
    }
}
