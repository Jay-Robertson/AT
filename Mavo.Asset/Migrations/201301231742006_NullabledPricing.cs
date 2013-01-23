namespace Mavo.Assets.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class NullabledPricing : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Jobs", "InvoiceDetail_NetInvoiceAmount", c => c.Decimal(precision: 18, scale: 2));
            AlterColumn("dbo.Jobs", "InvoiceDetail_Retainage", c => c.Decimal(precision: 18, scale: 2));
            AlterColumn("dbo.Jobs", "InvoiceDetail_WithholdPercentage", c => c.Decimal(precision: 18, scale: 2));
            AlterColumn("dbo.Jobs", "InvoiceDetail_GrossInvoiceAmount", c => c.Decimal(precision: 18, scale: 2));
            AlterColumn("dbo.Jobs", "InvoiceDetail_TotalAmountDue", c => c.Decimal(precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Jobs", "InvoiceDetail_TotalAmountDue", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("dbo.Jobs", "InvoiceDetail_GrossInvoiceAmount", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("dbo.Jobs", "InvoiceDetail_WithholdPercentage", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("dbo.Jobs", "InvoiceDetail_Retainage", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("dbo.Jobs", "InvoiceDetail_NetInvoiceAmount", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
    }
}
