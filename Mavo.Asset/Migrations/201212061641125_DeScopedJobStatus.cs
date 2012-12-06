namespace Mavo.Assets.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DeScopedJobStatus : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Jobs", "Status", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Jobs", "Status");
        }
    }
}
