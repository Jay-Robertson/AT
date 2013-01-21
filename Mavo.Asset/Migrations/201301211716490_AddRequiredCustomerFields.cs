namespace Mavo.Assets.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddRequiredCustomerFields : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Jobs", "Address_Address1", c => c.String(nullable: false));
            AlterColumn("dbo.Jobs", "Address_City", c => c.String(nullable: false));
            AlterColumn("dbo.Jobs", "Address_State", c => c.String(nullable: false));
            AlterColumn("dbo.Jobs", "Address_ZipCode", c => c.String(nullable: false));
            AlterColumn("dbo.Customers", "Address_Address1", c => c.String(nullable: false));
            AlterColumn("dbo.Customers", "Address_City", c => c.String(nullable: false));
            AlterColumn("dbo.Customers", "Address_State", c => c.String(nullable: false));
            AlterColumn("dbo.Customers", "Address_ZipCode", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Customers", "Address_ZipCode", c => c.String());
            AlterColumn("dbo.Customers", "Address_State", c => c.String());
            AlterColumn("dbo.Customers", "Address_City", c => c.String());
            AlterColumn("dbo.Customers", "Address_Address1", c => c.String());
            AlterColumn("dbo.Jobs", "Address_ZipCode", c => c.String());
            AlterColumn("dbo.Jobs", "Address_State", c => c.String());
            AlterColumn("dbo.Jobs", "Address_City", c => c.String());
            AlterColumn("dbo.Jobs", "Address_Address1", c => c.String());
        }
    }
}
