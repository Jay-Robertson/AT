namespace Mavo.Assets.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MovedSomeSummaryItems : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Jobs", "SendInvoiceTo", c => c.String());
            AddColumn("dbo.Jobs", "Attention", c => c.String());
            AddColumn("dbo.Jobs", "InvoiceAddress", c => c.String());
            AddColumn("dbo.Jobs", "InvoiceInstructions", c => c.String());
            AddColumn("dbo.Jobs", "SendFinalReportTo", c => c.String());
            AddColumn("dbo.Jobs", "FinalReportAddress", c => c.String());
            AddColumn("dbo.Jobs", "FinalReportPhoneNumber", c => c.String());
            AddColumn("dbo.Jobs", "Notifiable", c => c.Int(nullable: false));
            AddColumn("dbo.Jobs", "BillingType", c => c.Int(nullable: false));
            AddColumn("dbo.Jobs", "SupervisorsNeeded", c => c.Int());
            AddColumn("dbo.Jobs", "WorkersNeeded", c => c.Int());
            DropColumn("dbo.Jobs", "Summary_Notifiable");
            DropColumn("dbo.Jobs", "Summary_BillingType");
            DropColumn("dbo.Jobs", "Summary_SupervisorsNeeded");
            DropColumn("dbo.Jobs", "Summary_WorkersNeeded");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Jobs", "Summary_WorkersNeeded", c => c.Int());
            AddColumn("dbo.Jobs", "Summary_SupervisorsNeeded", c => c.Int());
            AddColumn("dbo.Jobs", "Summary_BillingType", c => c.Int(nullable: false));
            AddColumn("dbo.Jobs", "Summary_Notifiable", c => c.Int(nullable: false));
            DropColumn("dbo.Jobs", "WorkersNeeded");
            DropColumn("dbo.Jobs", "SupervisorsNeeded");
            DropColumn("dbo.Jobs", "BillingType");
            DropColumn("dbo.Jobs", "Notifiable");
            DropColumn("dbo.Jobs", "FinalReportPhoneNumber");
            DropColumn("dbo.Jobs", "FinalReportAddress");
            DropColumn("dbo.Jobs", "SendFinalReportTo");
            DropColumn("dbo.Jobs", "InvoiceInstructions");
            DropColumn("dbo.Jobs", "InvoiceAddress");
            DropColumn("dbo.Jobs", "Attention");
            DropColumn("dbo.Jobs", "SendInvoiceTo");
        }
    }
}
