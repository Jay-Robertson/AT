namespace Mavo.Assets.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangesForContractor : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Jobs", "InvoiceDetail_Consultant_Address1", c => c.String());
            AddColumn("dbo.Jobs", "InvoiceDetail_Consultant_Address2", c => c.String());
            AddColumn("dbo.Jobs", "InvoiceDetail_Consultant_City", c => c.String());
            AddColumn("dbo.Jobs", "InvoiceDetail_Consultant_State", c => c.String());
            AddColumn("dbo.Jobs", "InvoiceDetail_Consultant_ZipCode", c => c.String());
            AddColumn("dbo.Jobs", "ConsultantName", c => c.String());
            AddColumn("dbo.Jobs", "ConsultantContact", c => c.String());
            AddColumn("dbo.Jobs", "ConsultantContactNumber", c => c.String());
            AddColumn("dbo.Jobs", "ConsultantEmail", c => c.String());
            DropColumn("dbo.Jobs", "InvoiceDetail_SendInvoiceTo_Address1");
            DropColumn("dbo.Jobs", "InvoiceDetail_SendInvoiceTo_Address2");
            DropColumn("dbo.Jobs", "InvoiceDetail_SendInvoiceTo_City");
            DropColumn("dbo.Jobs", "InvoiceDetail_SendInvoiceTo_State");
            DropColumn("dbo.Jobs", "InvoiceDetail_SendInvoiceTo_ZipCode");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Jobs", "InvoiceDetail_SendInvoiceTo_ZipCode", c => c.String());
            AddColumn("dbo.Jobs", "InvoiceDetail_SendInvoiceTo_State", c => c.String());
            AddColumn("dbo.Jobs", "InvoiceDetail_SendInvoiceTo_City", c => c.String());
            AddColumn("dbo.Jobs", "InvoiceDetail_SendInvoiceTo_Address2", c => c.String());
            AddColumn("dbo.Jobs", "InvoiceDetail_SendInvoiceTo_Address1", c => c.String());
            DropColumn("dbo.Jobs", "ConsultantEmail");
            DropColumn("dbo.Jobs", "ConsultantContactNumber");
            DropColumn("dbo.Jobs", "ConsultantContact");
            DropColumn("dbo.Jobs", "ConsultantName");
            DropColumn("dbo.Jobs", "InvoiceDetail_Consultant_ZipCode");
            DropColumn("dbo.Jobs", "InvoiceDetail_Consultant_State");
            DropColumn("dbo.Jobs", "InvoiceDetail_Consultant_City");
            DropColumn("dbo.Jobs", "InvoiceDetail_Consultant_Address2");
            DropColumn("dbo.Jobs", "InvoiceDetail_Consultant_Address1");
        }
    }
}
