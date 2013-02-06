namespace Mavo.Assets.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangeDataType : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Jobs", "InvoiceDetail_Retainage", c => c.Double());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Jobs", "InvoiceDetail_Retainage", c => c.Decimal(precision: 18, scale: 2));
        }
    }
}
