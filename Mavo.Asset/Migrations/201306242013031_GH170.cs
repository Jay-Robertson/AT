namespace Mavo.Assets.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class GH170 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Jobs", "Labels", c => c.String());
            AddColumn("dbo.Jobs", "Drums", c => c.String());
            AddColumn("dbo.Jobs", "Bags", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Jobs", "Bags");
            DropColumn("dbo.Jobs", "Drums");
            DropColumn("dbo.Jobs", "Labels");
        }
    }
}
