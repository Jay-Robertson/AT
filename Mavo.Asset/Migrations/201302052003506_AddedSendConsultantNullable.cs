namespace Mavo.Assets.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedSendConsultantNullable : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Jobs", "InvoiceDetail_SendConsultant", c => c.Int());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Jobs", "InvoiceDetail_SendConsultant", c => c.Int(nullable: false));
        }
    }
}
