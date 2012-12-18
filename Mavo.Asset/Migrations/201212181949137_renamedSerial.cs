namespace Mavo.Assets.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class renamedSerial : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PickedAsset", "Barcode", c => c.String());
            DropColumn("dbo.PickedAsset", "Serial");
        }
        
        public override void Down()
        {
            AddColumn("dbo.PickedAsset", "Serial", c => c.String());
            DropColumn("dbo.PickedAsset", "Barcode");
        }
    }
}
