namespace Mavo.Assets.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedInvoiceDetails : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Jobs", "JobContact", c => c.String());
            AddColumn("dbo.Jobs", "JobContactPhone", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Jobs", "JobContactPhone");
            DropColumn("dbo.Jobs", "JobContact");
        }
    }
}
