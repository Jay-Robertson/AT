namespace Mavo.Assets.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemovedSiteName : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Jobs", "JobSiteName");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Jobs", "JobSiteName", c => c.String(nullable: false));
        }
    }
}
