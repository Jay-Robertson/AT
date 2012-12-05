namespace Mavo.Assets.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class JobAddedForman : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Jobs", "Foreman_Id", c => c.Int());
            AddForeignKey("dbo.Jobs", "Foreman_Id", "dbo.Users", "Id");
            CreateIndex("dbo.Jobs", "Foreman_Id");
        }
        
        public override void Down()
        {
            DropIndex("dbo.Jobs", new[] { "Foreman_Id" });
            DropForeignKey("dbo.Jobs", "Foreman_Id", "dbo.Users");
            DropColumn("dbo.Jobs", "Foreman_Id");
        }
    }
}
