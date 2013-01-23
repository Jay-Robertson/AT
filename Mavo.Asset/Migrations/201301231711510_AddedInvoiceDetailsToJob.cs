namespace Mavo.Assets.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedInvoiceDetailsToJob : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Jobs", "InvoiceDetail_Attention", c => c.String());
            AddColumn("dbo.Jobs", "InvoiceDetail_SendInvoiceTo", c => c.String());
            AddColumn("dbo.Jobs", "InvoiceDetail_InvoiceAddress", c => c.String());
            AddColumn("dbo.Jobs", "InvoiceDetail_InvoiceInstructions", c => c.String());
            AddColumn("dbo.Jobs", "InvoiceDetail_SendInvoiceApprovalTo", c => c.String());
            AddColumn("dbo.Jobs", "InvoiceDetail_IssueInvoiceToOwner", c => c.String());
            AddColumn("dbo.Jobs", "InvoiceDetail_Comments", c => c.String());
            AddColumn("dbo.Jobs", "InvoiceDetail_SpecialForms", c => c.Int(nullable: false));
            AddColumn("dbo.Jobs", "InvoiceDetail_CopyAddress", c => c.String());
            AddColumn("dbo.Jobs", "InvoiceDetail_CopyAttention", c => c.String());
            AddColumn("dbo.Jobs", "InvoiceDetail_CopyTo", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Jobs", "InvoiceDetail_CopyTo");
            DropColumn("dbo.Jobs", "InvoiceDetail_CopyAttention");
            DropColumn("dbo.Jobs", "InvoiceDetail_CopyAddress");
            DropColumn("dbo.Jobs", "InvoiceDetail_SpecialForms");
            DropColumn("dbo.Jobs", "InvoiceDetail_Comments");
            DropColumn("dbo.Jobs", "InvoiceDetail_IssueInvoiceToOwner");
            DropColumn("dbo.Jobs", "InvoiceDetail_SendInvoiceApprovalTo");
            DropColumn("dbo.Jobs", "InvoiceDetail_InvoiceInstructions");
            DropColumn("dbo.Jobs", "InvoiceDetail_InvoiceAddress");
            DropColumn("dbo.Jobs", "InvoiceDetail_SendInvoiceTo");
            DropColumn("dbo.Jobs", "InvoiceDetail_Attention");
        }
    }
}
