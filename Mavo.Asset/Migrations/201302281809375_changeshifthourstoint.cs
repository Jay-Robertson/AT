namespace Mavo.Assets.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class changeshifthourstoint : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Jobs", "Summary_ShiftHours", c => c.Int());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Jobs", "Summary_ShiftHours", c => c.String());
        }
    }
}
