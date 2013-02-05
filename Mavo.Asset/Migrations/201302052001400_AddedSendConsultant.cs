namespace Mavo.Assets.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedSendConsultant : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Jobs", "InvoiceDetail_SendConsultant", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Jobs", "InvoiceDetail_SendConsultant");
        }
    }
}
