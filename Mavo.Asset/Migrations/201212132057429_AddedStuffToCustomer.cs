namespace Mavo.Assets.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedStuffToCustomer : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Customers", "CustomerNumber", c => c.String());
            AddColumn("dbo.Customers", "ContactName", c => c.String());
            AddColumn("dbo.Customers", "Address_Address1", c => c.String(nullable: false));
            AddColumn("dbo.Customers", "Address_Address2", c => c.String());
            AddColumn("dbo.Customers", "Address_City", c => c.String(nullable: false));
            AddColumn("dbo.Customers", "Address_State", c => c.String(nullable: false));
            AddColumn("dbo.Customers", "Address_ZipCode", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Customers", "Address_ZipCode");
            DropColumn("dbo.Customers", "Address_State");
            DropColumn("dbo.Customers", "Address_City");
            DropColumn("dbo.Customers", "Address_Address2");
            DropColumn("dbo.Customers", "Address_Address1");
            DropColumn("dbo.Customers", "ContactName");
            DropColumn("dbo.Customers", "CustomerNumber");
        }
    }
}
