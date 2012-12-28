namespace Mavo.Assets.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addons2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Jobs", "IsPicked", c => c.Boolean());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Jobs", "IsPicked");
        }
    }
}
