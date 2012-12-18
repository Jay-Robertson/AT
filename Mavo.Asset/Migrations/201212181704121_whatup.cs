namespace Mavo.Assets.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class whatup : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Jobs", "PickStarted");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Jobs", "PickStarted", c => c.DateTime());
        }
    }
}
