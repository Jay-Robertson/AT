namespace Mavo.Assets.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedPricing : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Jobs", "InvoiceDetail_NetInvoiceAmount", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.Jobs", "InvoiceDetail_Retainage", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.Jobs", "InvoiceDetail_WithholdPercentage", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.Jobs", "InvoiceDetail_GrossInvoiceAmount", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.Jobs", "InvoiceDetail_TotalAmountDue", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Jobs", "InvoiceDetail_TotalAmountDue");
            DropColumn("dbo.Jobs", "InvoiceDetail_GrossInvoiceAmount");
            DropColumn("dbo.Jobs", "InvoiceDetail_WithholdPercentage");
            DropColumn("dbo.Jobs", "InvoiceDetail_Retainage");
            DropColumn("dbo.Jobs", "InvoiceDetail_NetInvoiceAmount");
        }
    }
}
