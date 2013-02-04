namespace Mavo.Assets.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ShiftHours : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Jobs", "InvoiceDetail_ShiftHours", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Jobs", "InvoiceDetail_ShiftHours");
        }
    }
}
