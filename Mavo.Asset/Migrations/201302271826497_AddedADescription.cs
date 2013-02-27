namespace Mavo.Assets.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedADescription : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Users", "EmployeeId", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Users", "EmployeeId", c => c.String(nullable: false));
        }
    }
}
