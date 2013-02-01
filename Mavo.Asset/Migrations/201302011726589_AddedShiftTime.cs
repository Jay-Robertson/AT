namespace Mavo.Assets.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedShiftTime : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Jobs", "ShiftStartDate", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Jobs", "ShiftStartDate");
        }
    }
}
