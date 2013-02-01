namespace Mavo.Assets.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangeAddressToAddress2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Jobs", "FinalReportAddress_Address1", c => c.String());
            AddColumn("dbo.Jobs", "FinalReportAddress_Address2", c => c.String());
            AddColumn("dbo.Jobs", "FinalReportAddress_City", c => c.String());
            AddColumn("dbo.Jobs", "FinalReportAddress_State", c => c.String());
            AddColumn("dbo.Jobs", "FinalReportAddress_ZipCode", c => c.String());
            AddColumn("dbo.Jobs", "InvoiceDetail_InvoiceAddress_Address1", c => c.String());
            AddColumn("dbo.Jobs", "InvoiceDetail_InvoiceAddress_Address2", c => c.String());
            AddColumn("dbo.Jobs", "InvoiceDetail_InvoiceAddress_City", c => c.String());
            AddColumn("dbo.Jobs", "InvoiceDetail_InvoiceAddress_State", c => c.String());
            AddColumn("dbo.Jobs", "InvoiceDetail_InvoiceAddress_ZipCode", c => c.String());
            AddColumn("dbo.Jobs", "InvoiceDetail_CopyAddress_Address1", c => c.String());
            AddColumn("dbo.Jobs", "InvoiceDetail_CopyAddress_Address2", c => c.String());
            AddColumn("dbo.Jobs", "InvoiceDetail_CopyAddress_City", c => c.String());
            AddColumn("dbo.Jobs", "InvoiceDetail_CopyAddress_State", c => c.String());
            AddColumn("dbo.Jobs", "InvoiceDetail_CopyAddress_ZipCode", c => c.String());
            DropColumn("dbo.Jobs", "FinalReportAddress");
            DropColumn("dbo.Jobs", "InvoiceDetail_InvoiceAddress");
            DropColumn("dbo.Jobs", "InvoiceDetail_CopyAddress");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Jobs", "InvoiceDetail_CopyAddress", c => c.String());
            AddColumn("dbo.Jobs", "InvoiceDetail_InvoiceAddress", c => c.String());
            AddColumn("dbo.Jobs", "FinalReportAddress", c => c.String());
            DropColumn("dbo.Jobs", "InvoiceDetail_CopyAddress_ZipCode");
            DropColumn("dbo.Jobs", "InvoiceDetail_CopyAddress_State");
            DropColumn("dbo.Jobs", "InvoiceDetail_CopyAddress_City");
            DropColumn("dbo.Jobs", "InvoiceDetail_CopyAddress_Address2");
            DropColumn("dbo.Jobs", "InvoiceDetail_CopyAddress_Address1");
            DropColumn("dbo.Jobs", "InvoiceDetail_InvoiceAddress_ZipCode");
            DropColumn("dbo.Jobs", "InvoiceDetail_InvoiceAddress_State");
            DropColumn("dbo.Jobs", "InvoiceDetail_InvoiceAddress_City");
            DropColumn("dbo.Jobs", "InvoiceDetail_InvoiceAddress_Address2");
            DropColumn("dbo.Jobs", "InvoiceDetail_InvoiceAddress_Address1");
            DropColumn("dbo.Jobs", "FinalReportAddress_ZipCode");
            DropColumn("dbo.Jobs", "FinalReportAddress_State");
            DropColumn("dbo.Jobs", "FinalReportAddress_City");
            DropColumn("dbo.Jobs", "FinalReportAddress_Address2");
            DropColumn("dbo.Jobs", "FinalReportAddress_Address1");
        }
    }
}
