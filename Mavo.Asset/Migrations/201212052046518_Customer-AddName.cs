namespace Mavo.Assets.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CustomerAddName : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Customers", "Name", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Customers", "Name");
        }
    }
}
