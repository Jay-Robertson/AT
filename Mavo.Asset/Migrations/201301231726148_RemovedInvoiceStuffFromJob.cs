namespace Mavo.Assets.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemovedInvoiceStuffFromJob : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Jobs", "SendInvoiceTo");
            DropColumn("dbo.Jobs", "Attention");
            DropColumn("dbo.Jobs", "InvoiceAddress");
            DropColumn("dbo.Jobs", "InvoiceInstructions");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Jobs", "InvoiceInstructions", c => c.String());
            AddColumn("dbo.Jobs", "InvoiceAddress", c => c.String());
            AddColumn("dbo.Jobs", "Attention", c => c.String());
            AddColumn("dbo.Jobs", "SendInvoiceTo", c => c.String());
        }
    }
}
