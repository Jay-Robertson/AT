namespace Mavo.Assets.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class whatup2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AssetItems", "Status", c => c.Int(nullable: false));
            DropColumn("dbo.Assets", "Status");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Assets", "Status", c => c.Int(nullable: false));
            DropColumn("dbo.AssetItems", "Status");
        }
    }
}
