namespace Mavo.Assets.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RetainageToNumberic2 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Jobs", "InvoiceDetail_Retainage", c => c.Int());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Jobs", "InvoiceDetail_Retainage", c => c.Double());
        }
    }
}
