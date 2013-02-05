namespace Mavo.Assets.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedSendCustomer : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Jobs", "InvoiceDetail_SendCustomer", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Jobs", "InvoiceDetail_SendCustomer");
        }
    }
}
