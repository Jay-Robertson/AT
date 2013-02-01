namespace Mavo.Assets.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangeAddressToAddress : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Jobs", "InvoiceDetail_SendInvoiceTo_Address1", c => c.String());
            AddColumn("dbo.Jobs", "InvoiceDetail_SendInvoiceTo_Address2", c => c.String());
            AddColumn("dbo.Jobs", "InvoiceDetail_SendInvoiceTo_City", c => c.String());
            AddColumn("dbo.Jobs", "InvoiceDetail_SendInvoiceTo_State", c => c.String());
            AddColumn("dbo.Jobs", "InvoiceDetail_SendInvoiceTo_ZipCode", c => c.String());
            DropColumn("dbo.Jobs", "InvoiceDetail_SendInvoiceTo");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Jobs", "InvoiceDetail_SendInvoiceTo", c => c.String());
            DropColumn("dbo.Jobs", "InvoiceDetail_SendInvoiceTo_ZipCode");
            DropColumn("dbo.Jobs", "InvoiceDetail_SendInvoiceTo_State");
            DropColumn("dbo.Jobs", "InvoiceDetail_SendInvoiceTo_City");
            DropColumn("dbo.Jobs", "InvoiceDetail_SendInvoiceTo_Address2");
            DropColumn("dbo.Jobs", "InvoiceDetail_SendInvoiceTo_Address1");
        }
    }
}
