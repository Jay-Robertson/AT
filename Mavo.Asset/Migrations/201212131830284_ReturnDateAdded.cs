namespace Mavo.Assets.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ReturnDateAdded : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Jobs", "ReturnedDate", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Jobs", "ReturnedDate");
        }
    }
}
