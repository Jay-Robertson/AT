namespace Mavo.Assets.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class JobAddedJobName : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Jobs", "Name", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Jobs", "Name");
        }
    }
}
