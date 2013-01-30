namespace Mavo.Assets.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addedstatus : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Users", "Disabled", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Users", "Disabled");
        }
    }
}
