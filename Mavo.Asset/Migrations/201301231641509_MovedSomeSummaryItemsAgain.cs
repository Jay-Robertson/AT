namespace Mavo.Assets.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MovedSomeSummaryItemsAgain : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Jobs", "ScopeOfWork", c => c.String());
            DropColumn("dbo.Jobs", "Summary_ScopeOfWork");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Jobs", "Summary_ScopeOfWork", c => c.String());
            DropColumn("dbo.Jobs", "ScopeOfWork");
        }
    }
}
