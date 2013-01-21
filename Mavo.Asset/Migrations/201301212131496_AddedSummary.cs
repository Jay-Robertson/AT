namespace Mavo.Assets.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedSummary : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Jobs", "Summary_FilledOutBy", c => c.String());
            AddColumn("dbo.Jobs", "Summary_Notifiable", c => c.Int(nullable: false));
            AddColumn("dbo.Jobs", "Summary_BillingType", c => c.Int(nullable: false));
            AddColumn("dbo.Jobs", "Summary_ShiftHours", c => c.String());
            AddColumn("dbo.Jobs", "Summary_SupervisorsNeeded", c => c.Int());
            AddColumn("dbo.Jobs", "Summary_WorkersNeeded", c => c.Int());
            AddColumn("dbo.Jobs", "Summary_Comments", c => c.String());
            AddColumn("dbo.Jobs", "Summary_ScopeOfWork", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Jobs", "Summary_ScopeOfWork");
            DropColumn("dbo.Jobs", "Summary_Comments");
            DropColumn("dbo.Jobs", "Summary_WorkersNeeded");
            DropColumn("dbo.Jobs", "Summary_SupervisorsNeeded");
            DropColumn("dbo.Jobs", "Summary_ShiftHours");
            DropColumn("dbo.Jobs", "Summary_BillingType");
            DropColumn("dbo.Jobs", "Summary_Notifiable");
            DropColumn("dbo.Jobs", "Summary_FilledOutBy");
        }
    }
}
