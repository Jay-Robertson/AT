namespace Mavo.Assets.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedQtyPicked : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AssetsWithQuantity", "QuantityPicked", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.AssetsWithQuantity", "QuantityPicked");
        }
    }
}
