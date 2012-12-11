namespace Mavo.Assets.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AssetUnitOfMeasure : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Assets", "UnitOfMeasure", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Assets", "UnitOfMeasure");
        }
    }
}
