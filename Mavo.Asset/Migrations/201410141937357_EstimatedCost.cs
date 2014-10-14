namespace Mavo.Assets.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class EstimatedCost : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Jobs", "EstimatedCost", c => c.Decimal(precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Jobs", "EstimatedCost");
        }
    }
}
