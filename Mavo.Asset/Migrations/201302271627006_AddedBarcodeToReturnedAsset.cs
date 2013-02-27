namespace Mavo.Assets.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedBarcodeToReturnedAsset : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ReturnedAsset", "Barcode", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ReturnedAsset", "Barcode");
        }
    }
}
